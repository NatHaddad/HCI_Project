import cv2
import numpy as np

# Define the HSV range for black color
lower_black = np.array([0, 0, 0])
upper_black = np.array([180, 255, 50])

# Define the HSV range for white color
lower_white = np.array([0, 0, 200])
upper_white = np.array([180, 30, 255])

cap = cv2.VideoCapture(0)

while True:
    ret, frame = cap.read()

    if not ret:
        break

    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

    # Create masks for black and white colors
    mask_black = cv2.inRange(hsv, lower_black, upper_black)
    mask_white = cv2.inRange(hsv, lower_white, upper_white)

    # Combine the masks
    combined_mask = cv2.bitwise_or(mask_black, mask_white)

    # Apply morphological transformations to remove small noise
    combined_mask = cv2.erode(combined_mask, None, iterations=2)
    combined_mask = cv2.dilate(combined_mask, None, iterations=2)

    # Find contours in the combined mask
    contours, _ = cv2.findContours(
        combined_mask.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_NONE
    )

    if contours:
        # Find the largest contour
        largest_contour = max(contours, key=cv2.contourArea)

        # Get the bounding box coordinates for the largest contour
        x, y, w, h = cv2.boundingRect(largest_contour)

        # Extract the ROI from the HSV image
        roi_hsv = hsv[y : y + h, x : x + w]

        # Calculate the mean HSV value of the ROI
        mean_hsv = cv2.mean(roi_hsv)[:3]  # Only take the first three elements (H, S, V)

        # Determine if the contour is black or white
        if (
            lower_black[0] <= mean_hsv[0] <= upper_black[0]
            and lower_black[1] <= mean_hsv[1] <= upper_black[1]
            and lower_black[2] <= mean_hsv[2] <= upper_black[2]
        ):
            color = "Black"
        elif (
            lower_white[0] <= mean_hsv[0] <= upper_white[0]
            and lower_white[1] <= mean_hsv[1] <= upper_white[1]
            and lower_white[2] <= mean_hsv[2] <= upper_white[2]
        ):
            color = "White"
        else:
            color = "Unknown"

        # Draw the bounding box on the frame
        cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)

        # Put the color text on the frame
        cv2.putText(
            frame, color, (x, y - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.9, (0, 255, 0), 2
        )

    # Display the frame
    cv2.imshow("Black and White Color Tracking", frame)

    # Break the loop if 'q' is pressed
    if cv2.waitKey(1) & 0xFF == ord("q"):
        break

cap.release()
cv2.destroyAllWindows()
