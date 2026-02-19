# Visual Testing Guide

## Terminal Layout

```
┌─────────────────────────┐  ┌─────────────────────────┐  ┌─────────────────────────┐
│   Terminal 1: SERVER    │  │  Terminal 2: ALICE      │  │   Terminal 3: BOB       │
└─────────────────────────┘  └─────────────────────────┘  └─────────────────────────┘
```

## Step-by-Step Visual Guide

### Step 1: Start Server (Terminal 1)

```
D:\IAPCode\TestProject> start-server.bat

[SERVER] Started on port 5000
```

### Step 2: Alice Logs In (Terminal 2)

```
D:\IAPCode\TestProject> start-client.bat

Username: alice
Password: pass1

✓ Logged in as alice
Commands: dm <user> <msg> | multi <u1,u2> <msg> | broadcast <msg> | quit
>
```

**Server shows:**
```
[14:23:45] LOGIN: alice
```

### Step 3: Bob Logs In (Terminal 3)

```
D:\IAPCode\TestProject> start-client.bat

Username: bob
Password: pass2

✓ Logged in as bob
Commands: dm <user> <msg> | multi <u1,u2> <msg> | broadcast <msg> | quit
>
```

**Server shows:**
```
[14:23:52] LOGIN: bob
```

### Step 4: Alice Sends DM to Bob (Terminal 2)

**Alice types:**
```
> dm bob Hey Bob, how are you?
>
```

**Bob sees (Terminal 3):**
```
[DM from alice]: Hey Bob, how are you?
>
```

**Server shows:**
```
[14:24:01] DM: alice -> bob, 67b
```

### Step 5: Bob Replies (Terminal 3)

**Bob types:**
```
> dm alice I'm good, thanks Alice!
>
```

**Alice sees (Terminal 2):**
```
[DM from bob]: I'm good, thanks Alice!
>
```

**Server shows:**
```
[14:24:15] DM: bob -> alice, 72b
```

### Step 6: Start Charlie (New Terminal 4)

```
D:\IAPCode\TestProject> start-client.bat

Username: charlie
Password: pass3

✓ Logged in as charlie
Commands: dm <user> <msg> | multi <u1,u2> <msg> | broadcast <msg> | quit
>
```

**Server shows:**
```
[14:24:30] LOGIN: charlie
```

### Step 7: Alice Sends Multi Message (Terminal 2)

**Alice types:**
```
> multi bob,charlie Team meeting at 3pm today!
>
```

**Bob sees (Terminal 3):**
```
[MULTI from alice]: Team meeting at 3pm today!
>
```

**Charlie sees (Terminal 4):**
```
[MULTI from alice]: Team meeting at 3pm today!
>
```

**Server shows:**
```
[14:24:45] MULTI: alice -> [bob,charlie], 89b
```

### Step 8: Bob Broadcasts (Terminal 3)

**Bob types:**
```
> broadcast Server maintenance tonight at 10pm
>
```

**Alice sees (Terminal 2):**
```
[BROADCAST from bob]: Server maintenance tonight at 10pm
>
```

**Charlie sees (Terminal 4):**
```
[BROADCAST from bob]: Server maintenance tonight at 10pm
>
```

**Server shows:**
```
[14:25:00] BROADCAST: bob, 95b
```

### Step 9: Server Metrics (Terminal 1)

**Every 5 seconds, server displays:**
```
[METRICS] Online: 3, Msgs/sec: 0.80, Avg Latency: 1.45ms
```

### Step 10: Alice Quits (Terminal 2)

**Alice types:**
```
> quit
```

**Terminal 2 closes**

**Server shows:**
```
[14:25:30] LOGOUT: alice
```

**Next metrics update:**
```
[METRICS] Online: 2, Msgs/sec: 0.60, Avg Latency: 1.23ms
```

## Error Scenarios

### Invalid Login

```
Username: hacker
Password: wrong

✗ Login failed: Invalid credentials
```

**Server shows:**
```
[14:26:00] LOGIN attempt failed: invalid credentials
```

### Duplicate Login

**If Alice tries to login again while already logged in:**
```
Username: alice
Password: pass1

✗ Login failed: Already logged in
```

## Complete Server Log Example

```
[SERVER] Started on port 5000
[14:23:45] LOGIN: alice
[14:23:52] LOGIN: bob
[14:24:01] DM: alice -> bob, 67b
[14:24:15] DM: bob -> alice, 72b
[14:24:30] LOGIN: charlie
[14:24:45] MULTI: alice -> [bob,charlie], 89b
[14:25:00] BROADCAST: bob, 95b
[METRICS] Online: 3, Msgs/sec: 0.80, Avg Latency: 1.45ms
[14:25:30] LOGOUT: alice
[METRICS] Online: 2, Msgs/sec: 0.60, Avg Latency: 1.23ms
[14:26:00] LOGOUT: bob
[14:26:05] LOGOUT: charlie
[METRICS] Online: 0, Msgs/sec: 0.00, Avg Latency: 0.00ms
```

## Quick Command Reference

| Command | Example | Description |
|---------|---------|-------------|
| dm | `dm bob Hello!` | Send direct message to bob |
| multi | `multi bob,charlie Hi team!` | Send to multiple users |
| broadcast | `broadcast Important news!` | Send to all users |
| quit | `quit` | Exit client |

## Message Type Indicators

- `[DM from alice]` - Direct message
- `[MULTI from bob]` - Multi-user message
- `[BROADCAST from charlie]` - Broadcast message

## Success Indicators

✓ Login successful
✓ Message sent (no error)
✓ Message received (displayed with type indicator)
✓ Graceful disconnect

## Error Indicators

✗ Login failed
✗ Invalid command
✗ Connection lost

## Testing Checklist

- [ ] Server starts on port 5000
- [ ] Alice can login
- [ ] Bob can login
- [ ] Alice can send DM to Bob
- [ ] Bob receives DM from Alice
- [ ] Bob can reply to Alice
- [ ] Charlie can login
- [ ] Alice can send MULTI to Bob and Charlie
- [ ] Both Bob and Charlie receive MULTI
- [ ] Bob can BROADCAST
- [ ] Alice and Charlie receive BROADCAST
- [ ] Server shows metrics every 5 seconds
- [ ] Server logs all activities
- [ ] Invalid login is rejected
- [ ] Duplicate login is rejected
- [ ] Quit command works
- [ ] Server shows logout

All features working = ✅ Ready for submission!
