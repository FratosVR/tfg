import socket

UDP_IP = "127.0.0.1"
UDP_PORT = 1234
MESSAGE = b"Hello, World!"

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM) 
sock.bind((UDP_IP, UDP_PORT))

ticks = 0

data = ""
try:
    while(True):
        ticks += 1
        data, addr = sock.recvfrom(1024) # buffer size is 1024 bytes
        print("received message: %s" % data)

        if data != "" and ticks > 90:
            sock.sendto(MESSAGE, addr)
            data = ""
            ticks = 0
except KeyboardInterrupt:
    pass
finally:
    sock.close()