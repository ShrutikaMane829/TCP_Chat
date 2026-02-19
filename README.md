# TCP Chat Application

## Objective
Implement a small TCP chat server/client with a tiny application protocol supporting login, direct messaging, multi-user messaging, and broadcasting.

## Stack
- **Language**: C# (.NET 8)
- **Protocol**: TCP with JSON
- **Client**: CLI with REPL

## Features Implemented

### Protocol (JSON)
- **LOGIN_REQ** `{username, password}` → **LOGIN_RESP** `{ok|err, reason?}`
- **DM** `{to, msg}` - Direct message to one user
- **MULTI** `{to:[u1,u2,...], msg}` - Message to multiple users
- **BROADCAST** `{msg}` - Message to all users

### Server Features
✅ Session management  
✅ Message routing (DM, MULTI, BROADCAST)  
✅ Basic authentication (static credential list)  
✅ Heartbeat/idle timeout (60 seconds)  
✅ Concurrent client handling  
✅ Graceful disconnect  
✅ Malformed JSON defense (closes session)  
✅ Backpressure handling (per-client send queue with SemaphoreSlim)  
✅ Non-blocking accept loop  

### Client Features
✅ REPL to send packets  
✅ Pretty-print received messages  
✅ All message types supported  

### Metrics/Logs
✅ Online users count  
✅ Messages per second  
✅ Per-message enqueue→deliver latency  
✅ Audit log: timestamp, from, to, type, bytes  

## Credentials

| Username | Password |
|----------|----------|
| raj      | pass1    |
| priya    | pass2    |
| amit     | pass3    |
| neha     | pass4    |

## How to Run

### Open in Visual Studio
1. Open `D:\IAPCode\TestProject\TCPChat.sln`
2. Right-click Solution → Properties → Multiple startup projects
3. Set Server, Client, Client2, Client3 to **Start**
4. Press **F5**

### Login
- **Client**: raj / pass1
- **Client2**: priya / pass2
- **Client3**: amit / pass3

### Commands
```
dm <user> <message>           # Direct message
multi <user1,user2> <message> # Multi-user message
broadcast <message>           # Broadcast to all
quit                          # Exit
```

## Example Usage

**Raj (Client):**
```
> dm priya Hello Priya!
```

**Priya (Client2):**
```
[DM from raj]: Hello Priya!
> dm raj Hi Raj!
```

**Raj (Client):**
```
> multi priya,amit Team meeting at 3pm
```

**Both Priya and Amit receive:**
```
[MULTI from raj]: Team meeting at 3pm
```

**Amit (Client3):**
```
> broadcast Server maintenance tonight
```

**Raj and Priya receive:**
```
[BROADCAST from amit]: Server maintenance tonight
```

## Server Output

```
Server started on port 5000
[14:23:45] raj -> [] [LOGIN] (45 bytes)
[14:23:52] priya -> [] [LOGIN] (47 bytes)
[14:24:01] raj -> priya [DM] (52 bytes)
[14:24:15] raj -> priya,amit [MULTI] (68 bytes)
[14:24:30] amit -> ALL [BROADCAST] (75 bytes)
[METRICS] Online: 3, Msgs/sec: 0.50, Avg Latency: 1.23ms
[DISCONNECT] raj logged out
```

## Architecture

### Server
- **Async TCP Listener**: Non-blocking accept loop
- **Per-Client Handler**: Each client in separate task
- **Session Management**: ConcurrentDictionary for thread-safe access
- **Send Queue**: SemaphoreSlim per client prevents blocking
- **Heartbeat**: 60-second idle timeout
- **Metrics**: Background task reports every 10 seconds
- **Malformed JSON**: Try-catch closes session on invalid JSON

### Client
- **REPL**: Read-Eval-Print Loop for commands
- **Async Receiver**: Background task for incoming messages
- **Pretty Print**: Formatted output with message type indicators

## Constraints Met

✅ **Concurrent clients**: Multiple clients handled simultaneously  
✅ **Graceful disconnect**: Proper cleanup on exit  
✅ **Malformed JSON defense**: Session closed on invalid JSON  
✅ **Backpressure**: Per-client send queue with SemaphoreSlim  
✅ **Non-blocking accept**: Accept loop never blocks  

## Acceptance Criteria

✅ Two terminals can log in  
✅ Exchange DM messages reliably  
✅ Exchange MULTI messages reliably  
✅ Exchange BROADCAST messages reliably  

## Deliverables

✅ Server application  
✅ CLI client application  
✅ Sample credentials (credentials.txt)  
✅ Demo script (DEMO_SCRIPT.txt)  
✅ README (this file)  

## Timebox

✅ Completed within 60 minutes

## Project Structure

```
TestProject/
├── Server/
│   └── Program.cs          # TCP server with all features
├── Client/
│   └── Program.cs          # CLI client
├── Client2/
│   └── Program.cs          # CLI client (copy)
├── Client3/
│   └── Program.cs          # CLI client (copy)
├── TCPChat.sln             # Visual Studio solution
├── credentials.txt         # Sample credentials
├── DEMO_SCRIPT.txt         # Demo script
└── README.md               # This file
```

## Testing

1. Start server (F5 in Visual Studio)
2. Login 3 clients with different credentials
3. Test DM: `dm priya Hello!`
4. Test MULTI: `multi priya,amit Team message!`
5. Test BROADCAST: `broadcast Important news!`
6. Verify server logs show all activities
7. Verify metrics display every 10 seconds

## All Requirements Covered

### Task
✅ Protocol: LOGIN_REQ/LOGIN_RESP, DM, MULTI, BROADCAST  
✅ Server: Sessions, routing, auth, heartbeat/timeout  
✅ Client: REPL, pretty-print  

### Constraints
✅ Concurrent clients  
✅ Graceful disconnect  
✅ Malformed JSON defense  
✅ Backpressure (per-client send queue)  
✅ Non-blocking accept loop  

### Metrics/Logs
✅ Online users  
✅ Msgs/sec  
✅ Enqueue→deliver latency  
✅ Audit log: timestamp, from, to, type, bytes  

### Deliverables
✅ Server + CLI client  
✅ Sample credentials  
✅ Demo script  
✅ README  

### Acceptance
✅ Two terminals can log in and exchange DM/MULTI/BROADCAST messages reliably  

### Timebox
✅ 60 minutes  

## Status: ✅ COMPLETE

All requirements met and ready for submission!
