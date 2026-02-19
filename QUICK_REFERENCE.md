# Quick Reference Guide

## Login Credentials

**User 1:**
- Username: `raj`
- Password: `pass1`

**User 2:**
- Username: `priya`
- Password: `pass2`

## How to Run

1. Open `TCPChat.sln` in Visual Studio
2. Press **F5**
3. Login in Client window as **raj/pass1**
4. Login in Client2 window as **priya/pass2**

## Basic Commands

### Send Direct Message (DM)
```
> dm priya Hello!
```

### Send to Multiple Users
```
> multi raj,priya Team message!
```

### Broadcast to Everyone
```
> broadcast Important announcement!
```

### Exit
```
> quit
```

## Example Chat

**Raj's window:**
```
> dm priya Hi Priya, how are you?
```

**Priya's window:**
```
[DM from raj]: Hi Priya, how are you?
> dm raj I'm good, thanks!
```

**Raj's window:**
```
[DM from raj]: I'm good, thanks!
> broadcast Meeting at 3pm
```

**Priya's window:**
```
[BROADCAST from raj]: Meeting at 3pm
```

## That's It!
Simple chat application with direct messaging, multi-user messaging, and broadcast features.
