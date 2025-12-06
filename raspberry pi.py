import RPi.GPIO as GPIO
import time
import socket

PIN = 7
PC_IP = "192.168.137.1"
PORT = 5000

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((PC_IP, PORT))

GPIO.setmode(GPIO.BCM)
GPIO.setup(PIN, GPIO.IN)

try:
    last = None
    while True:
        value = GPIO.input(PIN)
        msg = "DETECTED" if value == 0 else "NORMAL"
        if msg != last:
            sock.sendall((msg + "\n").encode())
            print("Sensor:", msg)
            last = msg
        time.sleep(0.1)
except KeyboardInterrupt:
    GPIO.cleanup()
    sock.close()
