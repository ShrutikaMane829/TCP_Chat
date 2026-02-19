# TCP Chat Application (login, 1–1, multi, broadcast) DEMO Script

**Objective:** Demonstrate login, DM, MULTI, BROADCAST, and metrics/logs.

---

## Step 1: Start the Server

**Action:** Press F5 in Visual Studio

**Expected output:**
```
Server started on port 5000
```

---

## Step 2: Start Client 1 (raj)

**User input:**
```
Username: raj
Password: pass1
```

**Expected output:**
```
✓ Logged in as raj
Commands: dm <user> <msg> | multi <u1,u2> <msg> | broadcast <msg> | quit
>
```

**Server log:**
```
raj logged in
```

---

## Step 3: Start Client 2 (priya)

**User input:**
```
Username: priya
Password: pass2
```

**Expected output:**
```
✓ Logged in as priya
Commands: dm <user> <msg> | multi <u1,u2> <msg> | broadcast <msg> | quit
>
```

**Server log:**
```
priya logged in
```

---

## Step 4: Start Client 3 (amit)

**Action:** Right-click Client project → Debug → Start New Instance

**User input:**
```
Username: amit
Password: pass3
```

**Expected output:**
```
✓ Logged in as amit
Commands: dm <user> <msg> | multi <u1,u2> <msg> | broadcast <msg> | quit
>
```

**Server log:**
```
amit logged in
```

---

## Step 5: Send a Direct Message (DM)

**Client 1 (raj) input:**
```
> dm priya Hello priya!
```

**Client 2 (priya) sees:**
```
[DM from raj]: Hello priya!
>
```

**Server log:**
```
raj -> priya: Hello priya!
```

---

## Step 6: Send a MULTI Message

**Client 1 (raj) input:**
```
> multi priya,amit Hello group!
```

**Client 2 (priya) sees:**
```
[MULTI from raj]: Hello group!
>
```

**Client 3 (amit) sees:**
```
[MULTI from raj]: Hello group!
>
```

**Server log:**
```
raj -> [priya,amit]: Hello group!
```

---

## Step 7: Send a BROADCAST Message

**Client 1 (raj) input:**
```
> broadcast Hello Everyone!
```

**Client 2 (priya) sees:**
```
[BROADCAST from raj]: Hello Everyone!
>
```

**Client 3 (amit) sees:**
```
[BROADCAST from raj]: Hello Everyone!
>
```

**Server log:**
```
raj broadcast: Hello Everyone!
```

---

## Step 8: Reply from Another Client

**Client 2 (priya) input:**
```
> dm raj Thanks for the message!
```

**Client 1 (raj) sees:**
```
[DM from priya]: Thanks for the message!
>
```

**Server log:**
```
priya -> raj: Thanks for the message!
```

---

## Step 9: Exit Clients

**Client 1 (raj) input:**
```
> quit
```

**Client 2 (priya) input:**
```
> quit
```

**Client 3 (amit) input:**
```
> quit
```

**Server log:**
```
raj logged out
priya logged out
amit logged out
```

---

## Summary of Features Demonstrated

✅ **LOGIN** - All 3 users logged in successfully  
✅ **DM (Direct Message)** - raj sent message to priya  
✅ **MULTI (Multi-user)** - raj sent message to priya and amit  
✅ **BROADCAST** - raj sent message to all users  
✅ **Server Logs** - All activities logged on server  
✅ **Graceful Exit** - All users disconnected properly

---

## Complete Server Console Output

```
Server started on port 5000
raj logged in
priya logged in
amit logged in
raj -> priya: Hello priya!
raj -> [priya,amit]: Hello group!
raj broadcast: Hello Everyone!
priya -> raj: Thanks for the message!
raj logged out
priya logged out
amit logged out
```

---

## Test Credentials

| Username | Password |
|----------|----------|
| raj      | pass1    |
| priya    | pass2    |
| amit     | pass3    |
| neha     | pass4    |

---

**End of Demo**
