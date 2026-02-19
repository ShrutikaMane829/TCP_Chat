using System.Net.Sockets;
using System.Text;
using System.Text.Json;

// Login
Console.Write("Username: ");
var user = Console.ReadLine();
Console.Write("Password: ");
var pass = Console.ReadLine();

// Connect
var client = new TcpClient();
await client.ConnectAsync("127.0.0.1", 5000);
var stream = client.GetStream();

// Send login request
await Send(new Msg { Type = "LOGIN_REQ", Username = user, Password = pass });

// Receive messages
_ = Task.Run(async () =>
{
    var buffer = new byte[4096];
    while (true)
    {
        try
        {
            var bytes = await stream.ReadAsync(buffer);
            if (bytes == 0) break;

            var json = Encoding.UTF8.GetString(buffer, 0, bytes);
            var msg = JsonSerializer.Deserialize<Msg>(json);

            if (msg?.Type == "LOGIN_RESP")
            {
                if (msg.Ok)
                {
                    Console.WriteLine($"\n✓ Logged in as {user}");
                    Console.WriteLine("Commands: dm <user> <msg> | multi <u1,u2> <msg> | broadcast <msg> | quit");
                    Console.Write("> ");
                }
                else
                {
                    Console.WriteLine($"\n✗ Login failed: {msg.Reason}");
                    Environment.Exit(1);
                }
            }
            else if (msg?.Type == "DM")
            {
                Console.WriteLine($"\n[DM from {msg.From}]: {msg.Message}");
                Console.Write("> ");
            }
            else if (msg?.Type == "MULTI")
            {
                Console.WriteLine($"\n[MULTI from {msg.From}]: {msg.Message}");
                Console.Write("> ");
            }
            else if (msg?.Type == "BROADCAST")
            {
                Console.WriteLine($"\n[BROADCAST from {msg.From}]: {msg.Message}");
                Console.Write("> ");
            }
        }
        catch { break; }
    }
});

// Send messages (REPL)
while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (string.IsNullOrEmpty(input)) continue;

    var parts = input.Split(' ', 3);

    if (parts[0] == "dm" && parts.Length >= 3)
    {
        await Send(new Msg { Type = "DM", To = parts[1], Message = parts[2] });
    }
    else if (parts[0] == "multi" && parts.Length >= 3)
    {
        await Send(new Msg { Type = "MULTI", ToList = parts[1].Split(',').ToList(), Message = parts[2] });
    }
    else if (parts[0] == "broadcast" && parts.Length >= 2)
    {
        await Send(new Msg { Type = "BROADCAST", Message = parts[1] });
    }
    else if (parts[0] == "quit")
    {
        break;
    }
}

client.Close();

async Task Send(Msg msg)
{
    var json = JsonSerializer.Serialize(msg);
    var bytes = Encoding.UTF8.GetBytes(json);
    await stream.WriteAsync(bytes);
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
