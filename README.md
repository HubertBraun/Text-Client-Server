# Text-Client-Server
## Overwiew
This project implements simple queries protocol between client and server. Client asks for answer of math operation, which are calculating and sends back.
## Description
Data is send via UDP protocol. Text protocol, which is inside look like:
Time - time in which datagram was send
OP - operation
ST - status
NS - sequence number
ID - identify number
ID - session identifier 
The Server stores history of operations and sessions, which can be send after seting history flag.
