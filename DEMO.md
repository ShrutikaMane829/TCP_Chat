# TCP Chat Application Demo Script

## Prerequisites
- .NET 8 SDK installed
- 3 terminal windows

## Step 1: Start the Server
```bash
cd Server
dotnet run
```
Expected output: `[SERVER] Started on port 5000`

## Step 2: Start Client 1 (Alice)
Open new terminal:
```bash
cd Client
dotnet run
```
- Username: `alice`
- Password: `pass1`

Expected: `✓ Logged in as alice`

## Step 3: Start Client 2 (Bob)
Open new terminal:
```bash
cd Client
dotnet run
```
- Username: `bob`
- Password: `pass2`

Expected: `✓ Logged in as bob`

## Step 4: Test DM (Direct Message)
In Alice's terminal:
```
> dm bob Hello Bob, this is a direct message!
```

In Bob's terminal, you should see:
```
[DM from alice]: Hello Bob, this is a direct message!
```

## Step 5: Test MULTI (Multi-user Message)
Start Client 3 (Charlie) in another terminal with username `charlie` and password `pass3`.

In Alice's terminal:
```
> multi bob,charlie Hey team, this is a group message!
```

Both Bob and Charlie should receive:
```
[MULTI from alice]: Hey team, this is a group message!
```

## Step 6: Test BROADCAST
In Bob's terminal:
```
> broadcast Important announcement to everyone!
```

All other connected users (Alice and Charlie) should receive:
```
[BROADCAST from bob]: Important announcement to everyone!
```

## Step 7: Test Invalid Login
Start a new client and try:
- Username: `invalid`
- Password: `wrong`

Expected: `✗ Login failed: Invalid credentials`

## Step 8: Check Server Metrics
In the server terminal, you'll see periodic metrics:
```
[METRICS] Online: 3, Msgs/sec: 0.60, Avg Latency: 2.34ms
```

## Step 9: Check Audit Logs
Server terminal shows audit logs for all activities:
```
[HH:mm:ss] LOGIN: alice
[HH:mm:ss] DM: alice -> bob, 67b
[HH:mm:ss] MULTI: alice -> [bob,charlie], 78b
[HH:mm:ss] BROADCAST: bob, 65b
[HH:mm:ss] LOGOUT: alice
```

## Step 10: Exit
In any client terminal:
```
> quit
```

## All Packet Types Demonstrated
1. ✓ LOGIN_REQ / LOGIN_RESP
2. ✓ DM (Direct Message)
3. ✓ MULTI (Multi-user Message)
4. ✓ BROADCAST (Broadcast to all)

## Error Handling Tested
- Invalid credentials
- Malformed JSON (auto-disconnect)
- Duplicate login prevention
- Graceful disconnect
