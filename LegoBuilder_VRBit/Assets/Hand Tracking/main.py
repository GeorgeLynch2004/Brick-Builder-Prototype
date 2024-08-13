# Hand Tracking
from cvzone.HandTrackingModule import HandDetector
import cv2
import socket
import numpy as np

# Gesture Recognition modules to be used if ML techniques are worth applying to this project.
#import os
#import mediapipe as mp
#from mediapipe.tasks import python
#from mediapipe.tasks.python import vision

# Camera calibration constants
KNOWN_DISTANCE = 50  # cm
KNOWN_WIDTH = 10  # cm (estimated distance between the base of the index and pinky fingers)
KNOWN_PIXEL_WIDTH = 300  # pixels (to be measured during calibration)

# Calculate distance value for depth
def calculate_distance(pixel_width):
    return (KNOWN_WIDTH * KNOWN_DISTANCE) / pixel_width

# Set up the Video Capture
cap = cv2.VideoCapture(0)
cap.set(3, 1280)
cap.set(4, 720)
success, img = cap.read()
h, w, _ = img.shape
detector = HandDetector(detectionCon=0.8, maxHands=2)

# Set up UDP
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)
    

while True:
    # Get image frame
    success, img = cap.read()
    # Find the hand and its landmarks
    hands, img = detector.findHands(img)  # with draw
    data = []

    if hands:
        # Hand 1
        hand = hands[0]
        lmList = hand["lmList"]  # List of 21 Landmark points
        for lm in lmList:
            data.extend([lm[0], h - lm[1], lm[2]])

        # Calculate hand width (distance between the base of the index and pinky fingers)
        indexBase = lmList[5][:2]  # Index base
        pinkyBase = lmList[17][:2]  # Pinky Base
        pixel_width = np.sqrt((indexBase[0] - pinkyBase[0])**2 + (indexBase[1] - pinkyBase[1])**2)
        
        # Estimate distance
        distance = calculate_distance(pixel_width)
        
        # Format distance
        distance = float(distance)
        
        
        # Add distance to data
        data.append(distance)
        
        # Draw result on image
        cv2.putText(img, f"Distance: {distance:.2f} cm", (10, 30), 
                    cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)

        # Send data
        sock.sendto(str.encode(str(data)), serverAddressPort)

    # Display
    cv2.imshow("Image", img)
    if cv2.waitKey(1) & 0xFF == 27:  # Exit on ESC
        break

cap.release()
cv2.destroyAllWindows()