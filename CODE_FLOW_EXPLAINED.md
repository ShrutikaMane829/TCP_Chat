# TCP Chat Application - Code Flow Explained (Simple Language)

## Overview
This document explains how the code works step-by-step in simple language.

---

## 🖥️ SERVER CODE FLOW

### 1. **Server Starts** (Lines 13-15)
- Server creates a listener on port 5000
- Prints "Server started on port 5000"
- Now waiting for clients to connect

### 2. **Metrics Task Starts** (Lines 18-24)
- A background task runs every 10 seconds
- Shows: How many users online, messages per second, average latency
- Runs forever in the background

### 3. **Accept Loop** (Lines 27-31)
- Server waits for new clients to connect
- When a client connects, creates a new task to handle that client
- Goes back to waiting for more clients (non-blocking)
- This loop never stops

### 4. **HandleClient Method** - Main Logic (Lines 33-131)

#### Step 4.1: Setup (Lines 35-38)
- Gets the network stream to send/receive data
- Creates a buffer to store incoming data
- Sets `user` to null (not logged in yet)
- Records last activity time for timeout

#### Step 4.2: Main Loop (Lines 42-125)
- Keeps running until client disconnects

#### Step 4.3: Heartbeat Check (Lines 44-49)
- Checks if 60 seconds passed without activity
- If yes, disconnects the client (timeout)

#### Step 4.4: Read Data (Lines 51-53)
- Reads data from client
- If 0 bytes, client disconnected, break loop

#### Step 4.5: Parse JSON (Lines 55-68)
- Converts bytes to text (JSON)
- Tries to parse JSON into a message object
- If JSON is malformed/invalid, closes the connection (security)

#### Step 4.6: Handle LOGIN_REQ (Lines 72-92)
**What happens:**
1. Checks if username and password match (from dictionary)
2. If valid:
   - Creates a new session for this user
   - Adds to sessions dictionary
   - Sends LOGIN_RESP with Ok=true
   - Logs the login in audit log
3. If already logged in:
   - Sends error "Already logged in"
   - Disconnects
4. If invalid credentials:
   - Sends error "Invalid credentials"
   - Disconnects

#### Step 4.7: Handle DM (Direct Message) (Lines 94-101)
**What happens:**
1. Checks if user is logged in
2. Finds the target user in sessions
3. If found, sends message to that user only
4. Records metrics (latency)
5. Logs in audit log

#### Step 4.8: Handle MULTI (Multiple Users) (Lines 103-112)
**What happens:**
1. Checks if user is logged in
2. Loops through list of target users
3. For each user found in sessions, sends the message
4. Records metrics
5. Logs in audit log

#### Step 4.9: Handle BROADCAST (All Users) (Lines 114-123)
**What happens:**
1. Checks if user is logged in
2. Loops through ALL sessions
3. Sends message to everyone EXCEPT the sender
4. Records metrics
5. Logs in audit log

#### Step 4.10: Cleanup (Lines 127-135)
- When loop breaks (disconnect/error):
  - Removes user from sessions dictionary
  - Logs disconnect message
  - Closes the client connection

### 5. **Send Method** (Lines 137-142)
- Helper method to send a message
- Converts message object to JSON
- Converts JSON to bytes
- Writes bytes to network stream

### 6. **AuditLog Method** (Lines 144-147)
- Prints log with timestamp, from, to, type, bytes
- Example: `[14:23:45] raj -> priya [DM] (52 bytes)`

### 7. **ClientSession Class** (Lines 150-175)
**Purpose:** Handles per-client send queue (backpressure)

- **SemaphoreSlim**: Ensures only one message sent at a time per client
- **Send Method**:
  1. Waits for lock (if another message is sending, waits)
  2. Converts message to JSON, then bytes
  3. Writes to network stream
  4. Releases lock (next message can send)

### 8. **Msg Class** (Lines 177-188)
- Data structure for all messages
- Contains: Type, Username, Password, Ok, Reason, To, ToList, From, Message

### 9. **Metrics Class** (Lines 190-212)
- Tracks message count and latency
- **RecordMsg**: Increments count, adds latency
- **GetMsgRate**: Calculates messages per second
- **GetAvgLatency**: Calculates average latency

---

## 💻 CLIENT CODE FLOW

### 1. **Login Prompt** (Lines 4-8)
- Asks user to enter username
- Asks user to enter password
- Stores in variables

### 2. **Connect to Server** (Lines 11-13)
- Creates TCP client
- Connects to server at 127.0.0.1:5000
- Gets network stream for communication

### 3. **Send Login Request** (Line 16)
- Creates LOGIN_REQ message with username and password
- Sends to server using Send method

### 4. **Receive Task** (Lines 19-56)
**Background task that runs forever:**

#### Step 4.1: Read Data (Lines 24-26)
- Reads bytes from server
- If 0 bytes, server disconnected, break

#### Step 4.2: Parse JSON (Lines 28-29)
- Converts bytes to text (JSON)
- Deserializes into message object

#### Step 4.3: Handle LOGIN_RESP (Lines 31-42)
- If Ok=true:
  - Prints "✓ Logged in as {username}"
  - Shows available commands
  - Shows prompt ">"
- If Ok=false:
  - Prints "✗ Login failed: {reason}"
  - Exits program

#### Step 4.4: Handle DM (Lines 43-47)
- Prints: `[DM from {sender}]: {message}`
- Shows prompt ">"

#### Step 4.5: Handle MULTI (Lines 48-52)
- Prints: `[MULTI from {sender}]: {message}`
- Shows prompt ">"

#### Step 4.6: Handle BROADCAST (Lines 53-56)
- Prints: `[BROADCAST from {sender}]: {message}`
- Shows prompt ">"

### 5. **REPL Loop** (Lines 59-84)
**Main loop for user input:**

#### Step 5.1: Read Input (Lines 61-63)
- Shows prompt ">"
- Reads user input
- If empty, continue

#### Step 5.2: Parse Command (Line 65)
- Splits input into parts (max 3 parts)
- Example: "dm priya hello" → ["dm", "priya", "hello"]

#### Step 5.3: Handle DM Command (Lines 67-70)
- Format: `dm <user> <message>`
- Creates DM message with To and Message
- Sends to server

#### Step 5.4: Handle MULTI Command (Lines 71-74)
- Format: `multi <user1,user2> <message>`
- Splits user list by comma
- Creates MULTI message with ToList and Message
- Sends to server

#### Step 5.5: Handle BROADCAST Command (Lines 75-78)
- Format: `broadcast <message>`
- Creates BROADCAST message with Message
- Sends to server

#### Step 5.6: Handle QUIT Command (Lines 79-82)
- Breaks the loop
- Closes connection

### 6. **Close Connection** (Line 86)
- Closes TCP client
- Program ends

### 7. **Send Method** (Lines 88-93)
- Helper method to send messages
- Converts message object to JSON
- Converts JSON to bytes
- Writes bytes to network stream

### 8. **Msg Class** (Lines 95-106)
- Same as server's Msg class
- Data structure for all messages

---

## 🔄 COMPLETE FLOW EXAMPLE: Sending a DM

### Client Side:
1. User types: `dm priya Hello!`
2. REPL parses: ["dm", "priya", "Hello!"]
3. Creates Msg: `{Type="DM", To="priya", Message="Hello!"}`
4. Send method converts to JSON: `{"Type":"DM","To":"priya","Message":"Hello!"}`
5. Converts to bytes and sends to server

### Server Side:
1. HandleClient reads bytes from raj's connection
2. Converts bytes to JSON string
3. Deserializes JSON to Msg object
4. Sees Type="DM"
5. Looks up "priya" in sessions dictionary
6. Finds priya's session
7. Calls priya's session.Send with: `{Type="DM", From="raj", Message="Hello!"}`
8. ClientSession.Send waits for lock
9. Converts to JSON, then bytes
10. Writes to priya's network stream
11. Releases lock
12. Records metrics (latency)
13. Logs: `[14:23:45] raj -> priya [DM] (52 bytes)`

### Priya's Client Side:
1. Receive task reads bytes from server
2. Converts to JSON, deserializes to Msg
3. Sees Type="DM"
4. Prints: `[DM from raj]: Hello!`
5. Shows prompt: `>`

---

## 🔑 KEY CONCEPTS

### Async/Await
- Methods run without blocking
- Server can handle multiple clients simultaneously
- Client can receive messages while waiting for user input

### ConcurrentDictionary
- Thread-safe dictionary for sessions
- Multiple clients can be added/removed safely

### SemaphoreSlim
- Prevents multiple messages from being sent at once per client
- Handles backpressure (if client is slow, messages queue)

### Task.Run
- Creates background tasks
- Metrics task runs every 10 seconds
- Each client handled in separate task
- Client receive task runs in background

### Try-Catch
- Protects against malformed JSON
- Handles disconnections gracefully
- Ensures cleanup happens

---

## 📊 METHOD CALL SEQUENCE

### Server Startup:
```
Main
├── TcpListener.Start()
├── Task.Run(Metrics Loop)
└── AcceptTcpClientAsync() Loop
    └── Task.Run(HandleClient)
```

### HandleClient:
```
HandleClient
├── stream.ReadAsync()
├── JsonSerializer.Deserialize()
├── Handle LOGIN_REQ
│   ├── sessions.TryAdd()
│   ├── session.Send()
│   └── AuditLog()
├── Handle DM
│   ├── sessions.TryGetValue()
│   ├── target.Send()
│   ├── metrics.RecordMsg()
│   └── AuditLog()
├── Handle MULTI
│   ├── Loop: sessions.TryGetValue()
│   ├── Loop: target.Send()
│   ├── metrics.RecordMsg()
│   └── AuditLog()
└── Handle BROADCAST
    ├── Loop: sessions.Values
    ├── Loop: session.Send()
    ├── metrics.RecordMsg()
    └── AuditLog()
```

### Client Startup:
```
Main
├── TcpClient.ConnectAsync()
├── Send(LOGIN_REQ)
├── Task.Run(Receive Loop)
│   ├── stream.ReadAsync()
│   ├── JsonSerializer.Deserialize()
│   └── Print message
└── REPL Loop
    ├── Console.ReadLine()
    ├── Parse command
    └── Send(message)
```

---

## 🎯 SUMMARY

**Server:**
1. Starts and listens on port 5000
2. Accepts clients in a loop
3. Each client handled in separate task
4. Validates login, routes messages
5. Tracks metrics, logs everything

**Client:**
1. Connects to server
2. Sends login request
3. Background task receives messages
4. Main loop reads user commands
5. Sends messages based on commands

**Communication:**
- All messages are JSON over TCP
- Server routes messages to correct recipients
- Clients display received messages with formatting
