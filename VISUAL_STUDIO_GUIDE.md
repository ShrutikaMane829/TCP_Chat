# Running in Visual Studio - Single Solution

## Step 1: Open Solution
1. Open **Visual Studio**
2. **File** → **Open** → **Project/Solution**
3. Navigate to `D:\IAPCode\TestProject\TCPChat.sln`
4. Click **Open**

You'll see both projects in Solution Explorer:
```
Solution 'TCPChat'
├── Server
└── Client
```

## Step 2: Configure Multiple Startup Projects
1. Right-click on **Solution 'TCPChat'** in Solution Explorer
2. Select **Properties**
3. Go to **Startup Project**
4. Select **Multiple startup projects**
5. Set both projects to **Start**:
   - Server: **Start**
   - Client: **Start**
6. Click **OK**

## Step 3: Run Everything
1. Press **F5** or click **▶ Start**
2. Two console windows will open:
   - Server window (shows logs)
   - Client window (prompts for login)

3. In Client window:
   - Username: **alice**
   - Password: **pass1**

## Step 4: Start Second Client
1. In Visual Studio, right-click **Client** project
2. Select **Debug** → **Start New Instance**
3. New client window opens
4. Login as **bob** / **pass2**

## Step 5: Test
In Alice's window:
```
> dm bob Hello!
```

In Bob's window:
```
> dm alice Hi there!
```

## Quick Commands
- **F5** - Start debugging (both projects)
- **Ctrl+F5** - Start without debugging
- **Shift+F5** - Stop debugging

## Tips
- Server must start first (Visual Studio handles this automatically)
- To start more clients: Right-click Client → Debug → Start New Instance
- Each client runs in separate console window
