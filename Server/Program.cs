using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

// Users
//var users = new Dictionary<string, string> { ["user1"] = "pass1", ["user2"] = "pass2", ["user3"] = "pass3" };

// dynamic user pass 
var users = new Dictionary<string, string>(); Console.WriteLine("=== Generated Credentials ==="); 
for (int i = 1; i <= 10; i++) 
{ 
    var username = $"user{i}"; var password = $"pass{i}"; users[username] = password; 
    Console.WriteLine($"{username} / {password}");
}
Console.WriteLine("============================\n");

var sessions = new ConcurrentDictionary<string, ClientSession>();
var metrics = new Metrics();

// Start server
var server = new TcpListener(IPAddress.Any, 5000);
server.Start();
Console.WriteLine("Server started on port 5000");

// Metrics task
_ = Task.Run(async () =>
{
    while (true)
    {
        await Task.Delay(10000);
        Console.WriteLine($"[METRICS] Online: {sessions.Count}, Msgs/sec: {metrics.GetMsgRate():F2}, Avg Latency: {metrics.GetAvgLatency():F2}ms");
    }
});

// Accept clients (non-blocking)
while (true)
{
    var client = await server.AcceptTcpClientAsync();
    _ = Task.Run(() => HandleClient(client));
}

async Task HandleClient(TcpClient client)
{
    var stream = client.GetStream();
    var buffer = new byte[4096];
    string? user = null;
    var lastActivity = DateTime.Now;

    try
    {
        while (true)
        {
            // Heartbeat/idle timeout (60 seconds)
            if ((DateTime.Now - lastActivity).TotalSeconds > 60)
            {
                Console.WriteLine($"[TIMEOUT] {user ?? "Unknown"} idle timeout");
                break;
            }

            var bytes = await stream.ReadAsync(buffer);
            if (bytes == 0) break;

            lastActivity = DateTime.Now;
            var json = Encoding.UTF8.GetString(buffer, 0, bytes);
            
            // Defend against malformed JSON
            Msg? msg;
            try
            {
                msg = JsonSerializer.Deserialize<Msg>(json);
                if (msg == null) throw new Exception();
            }
            catch
            {
                Console.WriteLine($"[ERROR] Malformed JSON from {user ?? "Unknown"}, closing session");
                break;
            }

            var sw = Stopwatch.StartNew();

            // Login
            if (msg.Type == "LOGIN_REQ")
            {
                if (users.ContainsKey(msg.Username ?? "") && users[msg.Username!] == msg.Password)
                {
                    var session = new ClientSession(msg.Username!, stream);
                    if (sessions.TryAdd(msg.Username!, session))
                    {
                        user = msg.Username;
                        await session.Send(new Msg { Type = "LOGIN_RESP", Ok = true });
                        AuditLog("LOGIN", user, "", "LOGIN_RESP", bytes);
                    }
                    else
                    {
                        await Send(stream, new Msg { Type = "LOGIN_RESP", Ok = false, Reason = "Already logged in" });
                        break;
                    }
                }
                else
                {
                    await Send(stream, new Msg { Type = "LOGIN_RESP", Ok = false, Reason = "Invalid credentials" });
                    break;
                }
            }
            // Direct message
            else if (msg.Type == "DM" && user != null)
            {
                if (sessions.TryGetValue(msg.To ?? "", out var target))
                {
                    await target.Send(new Msg { Type = "DM", From = user, Message = msg.Message });
                    metrics.RecordMsg(sw.Elapsed.TotalMilliseconds);
                    AuditLog("DM", user, msg.To!, "DM", bytes);
                }
                else
                {
                    if (sessions.TryGetValue(user, out var sender))
                        await sender.Send(new Msg { Type = "ERROR", Message = $"User '{msg.To}' not found or offline" });
                }
            }
            // Multi message
            else if (msg.Type == "MULTI" && user != null)
            {
                var notFound = new List<string>();
                foreach (var u in msg.ToList ?? new())
                {
                    if (sessions.TryGetValue(u, out var target))
                        await target.Send(new Msg { Type = "MULTI", From = user, Message = msg.Message });
                    else
                        notFound.Add(u);
                }
                if (notFound.Any() && sessions.TryGetValue(user, out var sender))
                    await sender.Send(new Msg { Type = "ERROR", Message = $"Users not found: {string.Join(", ", notFound)}" });
                metrics.RecordMsg(sw.Elapsed.TotalMilliseconds);
                AuditLog("MULTI", user, string.Join(",", msg.ToList ?? new()), "MULTI", bytes);
            }
            // Broadcast
            else if (msg.Type == "BROADCAST" && user != null)
            {
                foreach (var s in sessions.Values)
                {
                    if (s.Username != user)
                        await s.Send(new Msg { Type = "BROADCAST", From = user, Message = msg.Message });
                }
                metrics.RecordMsg(sw.Elapsed.TotalMilliseconds);
                AuditLog("BROADCAST", user, "ALL", "BROADCAST", bytes);
            }
        }
    }
    catch { }
    finally
    {
        if (user != null)
        {
            sessions.TryRemove(user, out _);
            Console.WriteLine($"[DISCONNECT] {user} logged out");
        }
        client.Close();
    }
}

async Task Send(NetworkStream stream, Msg msg)
{
    var json = JsonSerializer.Serialize(msg);
    var bytes = Encoding.UTF8.GetBytes(json);
    await stream.WriteAsync(bytes);
}

void AuditLog(string type, string from, string to, string msgType, int bytes)
{
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {from} -> {to} [{type}] ({bytes} bytes)");
}

// Per-client send queue (backpressure handling)
class ClientSession
{
    public string Username { get; }
    private readonly NetworkStream _stream;
    private readonly SemaphoreSlim _sendLock = new(1);

    public ClientSession(string username, NetworkStream stream)
    {
        Username = username;
        _stream = stream;
    }

    public async Task Send(Msg msg)
    {
        await _sendLock.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(msg);
            var bytes = Encoding.UTF8.GetBytes(json);
            await _stream.WriteAsync(bytes);
        }
        finally
        {
            _sendLock.Release();
        }
    }
}

class Msg
{
    public string? Type { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool Ok { get; set; }
    public string? Reason { get; set; }
    public string? To { get; set; }
    public List<string>? ToList { get; set; }
    public string? From { get; set; }
    public string? Message { get; set; }
}

class Metrics
{
    private long _msgCount;
    private double _totalLatency;
    private DateTime _lastReset = DateTime.Now;

    public void RecordMsg(double latencyMs)
    {
        Interlocked.Increment(ref _msgCount);
        _totalLatency += latencyMs;
    }

    public double GetMsgRate()
    {
        var elapsed = (DateTime.Now - _lastReset).TotalSeconds;
        var rate = elapsed > 0 ? _msgCount / elapsed : 0;
        _msgCount = 0;
        _lastReset = DateTime.Now;
        return rate;
    }

    public double GetAvgLatency()
    {
        return _msgCount > 0 ? _totalLatency / _msgCount : 0;
    }
}
