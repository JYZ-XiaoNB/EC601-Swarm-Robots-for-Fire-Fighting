# Description
This repo contains the code for the Firefighting Swarm Drone project (FireFlys). This code is use to run the Waveshare Thermal Camera and RealSense D435 cameras that have both been integrated into a Raspberry Pi 5. We trained a model  to detect whether fire is present or not. The model is stored in project/model/fire_model_new.keras.


## Running instructions
To run the files, login to the raspberry pi and open 2 terminals. In both terminals login into the virtual environment
source ~/thermalenv/bin/activate

In terminal 1 start the thermal camera
python run_thermal_camera.py

In terminal 2, start the real sense camera
python run_program.py

The thermal camera program will open 1 window with the thermal camera output. The realsense camera will open 2 windows for the RGB camera and the depth camera

