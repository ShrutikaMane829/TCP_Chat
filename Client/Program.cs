using System.Net.Sockets;
using System.Text;
using System.Text.Json;

// Login loop
TcpClient client;
NetworkStream stream;
string? user;

while (true)
{
    Console.Write("Username: ");
    user = Console.ReadLine();
    Console.Write("Password: ");
    var pass = Console.ReadLine();

    // Connect
    client = new TcpClient();
    await client.ConnectAsync("127.0.0.1", 5000);
    stream = client.GetStream();

    // Send login request
    await Send(new Msg { Type = "LOGIN_REQ", Username = user, Password = pass });

    // Wait for login response
    var buffer = new byte[4096];
    var bytes = await stream.ReadAsync(buffer);
    var json = Encoding.UTF8.GetString(buffer, 0, bytes);
    var loginResp = JsonSerializer.Deserialize<Msg>(json);

    if (loginResp?.Ok == true)
    {
        Console.WriteLine($"\n✓ Logged in as {user}");
        Console.WriteLine("Commands: dm <user> <msg> | multi <u1,u2> <msg> | broadcast <msg> | quit");
        break;
    }
    else
    {
        Console.WriteLine($"✗ Wrong username or password\n");
        client.Close();
    }
}

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

            if (msg?.Type == "DM")
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
            else if (msg?.Type == "ERROR")
            {
                Console.WriteLine($"\n✗ Error: {msg.Message}");
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

    if (parts[0] == "dm")
    {
        if (parts.Length < 3)
            Console.WriteLine("✗ Usage: dm <user> <message>");
        else
            await Send(new Msg { Type = "DM", To = parts[1], Message = parts[2] });
    }
    else if (parts[0] == "multi")
    {
        if (parts.Length < 3)
            Console.WriteLine("✗ Usage: multi <user1,user2> <message>");
        else
            await Send(new Msg { Type = "MULTI", ToList = parts[1].Split(',').ToList(), Message = parts[2] });
    }
    else if (parts[0] == "broadcast")
    {
        if (parts.Length < 2)
            Console.WriteLine("✗ Usage: broadcast <message>");
        else
            await Send(new Msg { Type = "BROADCAST", Message = string.Join(' ', parts.Skip(1)) });
    }
    else if (parts[0] == "quit")
    {
        break;
    }
    else
    {
        Console.WriteLine($"✗ Invalid command '{parts[0]}'. Available commands: dm, multi, broadcast, quit");
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
