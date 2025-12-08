import pyrealsense2 as rs
import numpy as np
import cv2
from PIL import Image
from keras.models import load_model

#Activate the thermal camera separately

model_file = "/home/lirifx/project/model/fire_model_new.keras"
fire_prediction_threshold=0.6 #if probability is > 0.5 then fire is detected

def make_predictions(model_file, color_image):
    #output="No Fire"
    #Resize image
    input_width=128
    input_height=128
    im=Image.fromarray(color_image, 'RGB')
    im=im.resize((input_width, input_height))
    im_array=np.array(im)
    im_array=np.expand_dims(im_array, axis=0)
    

    #Load model and get prediction
    model = load_model(model_file)
    predictions = model.predict(im_array)

    
    fire_pred=np.argmax(predictions, axis=1)

    #To print only the predictions value
    np.set_printoptions(legacy='1.25')
    delimiter=" "
    #if fire_pred==1:
    if predictions[0][1]>=fire_prediction_threshold:
        str_out=["FIRE DETECTED (Prob", str(predictions[0][1].round(3)),")"]
    else:
        str_out=["No Fire (Prob ",str(predictions[0][0].round(3)), ")"]
    output=delimiter.join(str_out)    
      
    return output



pipe=rs.pipeline()
cfg=rs.config()

cfg.enable_stream(rs.stream.color, 640,480, rs.format.bgr8, 30)
cfg.enable_stream(rs.stream.depth, 640,480, rs.format.z16, 30)

pipe.start(cfg)

while True:
    frame = pipe.wait_for_frames()
    depth_frame = frame.get_depth_frame()
    color_frame = frame.get_color_frame()

    depth_image = np.asanyarray(depth_frame.get_data())
    color_image = np.asanyarray(color_frame.get_data())
    depth_cm = cv2.applyColorMap(cv2.convertScaleAbs(depth_image,
                                     alpha = 0.5), cv2.COLORMAP_JET)

    gray_image = cv2.cvtColor(color_image, cv2.COLOR_BGR2GRAY)

    #Predict if a fire exists
    output=make_predictions(model_file, color_image)

    #Print text box on screen
    str_output=str(output)
    substr="No"
    if substr in output:
        font_color=(0,255,0) #font is green
    else:
        font_color=(0,0,255) #font is red
        
        
    cv2.putText(color_image,
                str_output, #text on video
                (50,50), #bottom left corner of text
                cv2.FONT_HERSHEY_SIMPLEX,  #font
                1.0,
                font_color,
                 2,
                 cv2.LINE_4)
                
    print(output)

    cv2.imshow('rgb camera', color_image)
    cv2.imshow('depth camera', depth_cm)

    if cv2.waitKey(1) == ord('q'):
        print("stopping program")
        break

pipe.stop()
