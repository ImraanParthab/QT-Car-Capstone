import cv2
import argparse
import numpy as np
from ultralytics import YOLO
import matplotlib.pyplot as plt

def detect_objects(image, model):
    
    # Perform detection
    results = model(image)[0]
    
    # Create a copy of the image for drawing
    annotated_image = image.copy()
    
    # Generate random colours for classes
    np.random.seed(42)  # For consistent colours
    colours = np.random.randint(0, 255, size=(100, 3), dtype=np.uint8)
    
    # Process detections
    boxes = results.boxes

    return boxes, results.names, annotated_image, colours

# is screen renderer
def main(server_url):
    cap = cv2.VideoCapture(server_url)

    model = YOLO('yolov8n.pt')

    if not cap.isOpened():
        print(f"Failed to connect to stream at {server_url}")
        return

    while True:
        ret, frame = cap.read()
        if not ret:
            break

        # runs custom fucntion
        boxes, class_names, annotated_image, colours = detect_objects(frame, model)
        # Process each detected object and apply confidence threshold filtering

        for box in boxes:
            # Get box coordinates
            x1, y1, x2, y2 = map(int, box.xyxy[0])
            
            # Get confidence score
            confidence = float(box.conf[0])
            
            # Only show detections above confidence threshold
            if confidence > 0.2:
                # Get class id and name
                class_id = int(box.cls[0])
                class_name = class_names[class_id]
                
                # Get colour for this class
                colour = colours[class_id % len(colours)].tolist()
                
                # Draw bounding box
                cv2.rectangle(annotated_image, (x1, y1), (x2, y2), colour, 2)

                #lable box
                cv2.putText(annotated_image, class_name, (x1,y1), cv2.FONT_HERSHEY_SIMPLEX, 1, colour, 2)


        cv2.imshow("Webcam Stream", annotated_image)


        if cv2.waitKey(1) & 0xFF == 27:
            break

    cap.release()
    cv2.destroyAllWindows()

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--url', default='http://192.168.1.100:8080/video',
                        help='URL of the webcam stream server')
    args = parser.parse_args()

    main(args.url)