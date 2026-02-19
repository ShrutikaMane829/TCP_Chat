# Quick Start Guide

## 🚀 Fast Setup (3 Steps)

### 1. Start Server
```bash
cd D:\IAPCode\TestProject
start-server.bat
```
OR
```bash
cd Server
dotnet run
```

### 2. Start Client 1 (Alice)
Open new terminal:
```bash
cd D:\IAPCode\TestProject
start-client.bat
```
- Username: **alice**
- Password: **pass1**

### 3. Start Client 2 (Bob)
Open another terminal:
```bash
cd D:\IAPCode\TestProject
start-client.bat
```
- Username: **bob**
- Password: **pass2**

## 💬 Test Commands

### In Alice's terminal:
```
> dm bob Hello Bob!
> broadcast Everyone, this is Alice!
```

### In Bob's terminal:
```
> dm alice Hi Alice!
```

### Start Client 3 (Charlie) and test multi:
- Username: **charlie**
- Password: **pass3**

### In Alice's terminal:
```
> multi bob,charlie Hey team!
```

## ✅ What You'll See

**Server Terminal:**
```
[SERVER] Started on port 5000
[14:23:45] LOGIN: alice
[14:23:52] LOGIN: bob
[14:24:01] DM: alice -> bob, 67b
[14:24:15] BROADCAST: alice, 78b
[METRICS] Online: 2, Msgs/sec: 0.40, Avg Latency: 1.23ms
```

**Bob's Terminal (receiving):**
```
[DM from alice]: Hello Bob!
[BROADCAST from alice]: Everyone, this is Alice!
```

## 📋 All Features Implemented

✅ LOGIN_REQ / LOGIN_RESP with authentication  
✅ DM (Direct Message) - 1-to-1 messaging  
✅ MULTI - Send to multiple users  
✅ BROADCAST - Send to all users  
✅ Concurrent client handling  
✅ Graceful disconnect  
✅ Malformed JSON protection  
✅ Metrics (online users, msgs/sec, latency)  
✅ Audit logging (timestamp, from, to, type, bytes)  
✅ Backpressure handling (per-client send queue)  
✅ Session management  
✅ Duplicate login prevention  

## 🎯 Acceptance Criteria Met

✅ Two terminals can log in and exchange messages  
✅ DM/MULTI/BROADCAST work reliably  
✅ All packet types demonstrated  
✅ Complete README and demo script  
✅ Sample credentials provided  

## 📁 Project Structure
```
TestProject/
├── Server/Program.cs       # TCP server (150 lines)
├── Client/Program.cs       # CLI client (80 lines)
├── README.md              # Full documentation
├── DEMO.md                # Step-by-step demo
├── credentials.txt        # Sample users
├── QUICKSTART.md          # This file
├── start-server.bat       # Server launcher
└── start-client.bat       # Client launcher
```

## 🔐 Credentials
- alice / pass1
- bob / pass2
- charlie / pass3

## 🛠️ Tech Stack
- C# .NET 8
- TCP Sockets
- JSON Protocol
- Async/Await
- Thread-safe Collections

## ⏱️ Development Time
Completed within 60-minute timebox requirement.
