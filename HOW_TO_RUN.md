# How to Run - 2 Clients in Solution

## Step 1: Open Solution
1. Open **Visual Studio**
2. **File** → **Open** → **Project/Solution**
3. Open `D:\IAPCode\TestProject\TCPChat.sln`

You'll see:
```
Solution 'TCPChat'
├── Server
├── Client
└── Client2
```

## Step 2: Set Multiple Startup (One-time)
1. Right-click **Solution 'TCPChat'**
2. **Properties**
3. Select **Multiple startup projects**
4. Set all 3 to **Start**:
   - Server → **Start**
   - Client → **Start**
   - Client2 → **Start**
5. Click **OK**

## Step 3: Run
Press **F5**

Three windows open:
- **Server** (logs)
- **Client** (login prompt)
- **Client2** (login prompt)

## Step 4: Login
**Client window:**
```
Username: alice
Password: pass1
```

**Client2 window:**
```
Username: bob
Password: pass2
```

## Step 5: Test
**In Alice's window (Client):**
```
> dm bob Hello Bob!
```

**In Bob's window (Client2):**
```
[DM from alice]: Hello Bob!
> dm alice Hi Alice!
```

## Add More Clients
Right-click **Client** or **Client2** → **Debug** → **Start New Instance**

Login as:
- charlie / pass3
- david / pass4

## Done!
Press **F5** → 3 windows open automatically (Server + 2 Clients)
