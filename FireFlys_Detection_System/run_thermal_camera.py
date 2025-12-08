import subprocess

#Activate the thermal camera
#Use subprocess because we need to run as root and make it use the env python
#result=subprocess.run(['sudo', '/home/lirifx/thermalenv/bin/python', 'stream_spi.py'], capture_output=True, text=True)

process=subprocess.Popen(['sudo', '/home/lirifx/thermalenv/bin/python', 'stream_spi.py'], stdout=subprocess.PIPE, text=True)
stdout, stderr = process.communicate()