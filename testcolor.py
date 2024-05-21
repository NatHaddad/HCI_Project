# USAGE
# python ball_tracking.py --video ball_tracking_example.mp4
# python ball_tracking.py

# import the necessary packages
from collections import deque
from copy import deepcopy
import numpy as np
import argparse
import imutils
import cv2
import socket
import threading


def send_data(client_socket, message, server_address):
    client_socket.sendto(message.encode("utf-8"), server_address)


def cameraOne(client_socket, server_address):
    # construct the argument parse and parse the arguments
    ap = argparse.ArgumentParser()
    ap.add_argument("-v", "--video", help="path to the (optional) video file")
    ap.add_argument("-b", "--buffer", type=int, default=10, help="max buffer size")
    args = vars(ap.parse_args())

    # define the lower and upper boundaries of the "green"
    # ball in the HSV color space, then initialize the
    # list of tracked points
    # {'lower': (0, 0, 200), 'upper': (180, 55, 255)}
    color_range = {
        "green": {"lower": (35, 50, 50), "upper": (85, 255, 255)},
        "red": {"lower": (170, 100, 100), "upper": (180, 255, 255)},
        "blue": {"lower": (90, 50, 50), "upper": (130, 255, 255)},
        "yellow": {"lower": (20, 100, 100), "upper": (30, 255, 200)},
    }
    tracking_points = {}

    for color in color_range:
        tracking_points[color] = deque(maxlen=args["buffer"])

    # if a video path was not supplied, grab the reference
    # to the webcam
    if not args.get("video", False):
        camera = cv2.VideoCapture(2)

    # otherwise, grab a reference to the video file
    else:
        camera = cv2.VideoCapture(args["video"])

    # keep looping
    while True:
        # grab the current frame
        (grabbed, frame) = camera.read()

        # if we are viewing a video and we did not grab a frame,
        # then we have reached the end of the video
        if args.get("video") and not grabbed:
            break

        # resize the frame, blur it, and convert it to the HSV
        # color space
        frame = imutils.resize(frame, width=600)
        hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

        flag = 0
        flag2 = 0
        mask = cv2.inRange(
            hsv, color_range["green"]["lower"], color_range["green"]["upper"]
        )
        mask = cv2.erode(mask, None, iterations=2)
        mask = cv2.dilate(mask, None, iterations=2)
        cnts = cv2.findContours(mask.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)[-2]

        if len(cnts) > 0:
            c = max(cnts, key=cv2.contourArea)
            ((x, y), radius) = cv2.minEnclosingCircle(c)
            M = cv2.moments(c)
            center = (int(M["m10"] / M["m00"]), int(M["m01"] / M["m00"]))
            if radius > 10:
                cv2.circle(frame, (int(x), int(y)), int(radius), (0, 255, 255), 2)
                cv2.circle(frame, center, 5, (0, 0, 255), -1)
                tracking_points["green"].appendleft(center)
            else:
                tracking_points["green"] = deque(maxlen=args["buffer"])
        for i in range(1, len(tracking_points["green"])):
            if (
                tracking_points["green"][i - 1] is None
                or tracking_points["green"][i] is None
            ):
                continue
            thickness = int(np.sqrt(args["buffer"] / float(i + 1)) * 2.5)
            if len(tracking_points["green"]) > 0:
                cv2.line(
                    frame,
                    tracking_points["green"][i - 1],
                    tracking_points["green"][i],
                    (0, 0, 255),
                    thickness,
                )

                height, width, _ = frame.shape

                x=center[0]/width
                y=center[1]/height

                send_data(client_socket, str(['c',1,x,y]), server_address)

                flag = 1
        if flag == 0:
            mask = cv2.inRange(
                hsv, color_range["red"]["lower"], color_range["red"]["upper"]
            )
            mask = cv2.erode(mask, None, iterations=2)
            mask = cv2.dilate(mask, None, iterations=2)
            cnts = cv2.findContours(
                mask.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE
            )[-2]

            if len(cnts) > 0:
                c = max(cnts, key=cv2.contourArea)
                ((x, y), radius) = cv2.minEnclosingCircle(c)
                M = cv2.moments(c)
                center = (int(M["m10"] / M["m00"]), int(M["m01"] / M["m00"]))
                if radius > 10:
                    cv2.circle(frame, (int(x), int(y)), int(radius), (0, 255, 255), 2)
                    cv2.circle(frame, center, 5, (0, 0, 255), -1)
                    tracking_points["red"].appendleft(center)
                else:
                    tracking_points["red"] = deque(maxlen=args["buffer"])
            for i in range(1, len(tracking_points["red"])):
                if (
                    tracking_points["red"][i - 1] is None
                    or tracking_points["red"][i] is None
                ):
                    continue
                thickness = int(np.sqrt(args["buffer"] / float(i + 1)) * 2.5)
                if len(tracking_points["red"]) > 0:
                    cv2.line(
                        frame,
                        tracking_points["red"][i - 1],
                        tracking_points["red"][i],
                        (0, 0, 255),
                        thickness,
                    )
                    flag = 1
                    height, width, _ = frame.shape
                    x=center[0]/width
                    y=center[1]/height

                    send_data(client_socket, str(['c',0,x,y]), server_address)

                    # x = tracking_points["red"][i - 1] / width
                    # y = tracking_points["red"][i] / height
        if flag == 0:
            mask = cv2.inRange(
                hsv, color_range["blue"]["lower"], color_range["blue"]["upper"]
            )
            mask = cv2.erode(mask, None, iterations=2)
            mask = cv2.dilate(mask, None, iterations=2)
            cnts = cv2.findContours(
                mask.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE
            )[-2]

            if len(cnts) > 0:
                c = max(cnts, key=cv2.contourArea)
                ((x, y), radius) = cv2.minEnclosingCircle(c)
                M = cv2.moments(c)
                center = (int(M["m10"] / M["m00"]), int(M["m01"] / M["m00"]))
                if radius > 10:
                    cv2.circle(frame, (int(x), int(y)), int(radius), (0, 255, 255), 2)
                    cv2.circle(frame, center, 5, (0, 0, 255), -1)
                    tracking_points["blue"].appendleft(center)
                else:
                    tracking_points["blue"] = deque(maxlen=args["buffer"])
            for i in range(1, len(tracking_points["blue"])):
                if (
                    tracking_points["blue"][i - 1] is None
                    or tracking_points["blue"][i] is None
                ):
                    continue
                thickness = int(np.sqrt(args["buffer"] / float(i + 1)) * 2.5)
                if len(tracking_points["blue"]) > 0:
                    cv2.line(
                        frame,
                        tracking_points["blue"][i - 1],
                        tracking_points["blue"][i],
                        (0, 0, 255),
                        thickness,
                    )
                    flag = 1
                    height, width, _ = frame.shape
                    x=center[0]/width
                    y=center[1]/height
                    send_data(client_socket, str(['c',3,x,y]), server_address)

        if flag == 0:
            mask = cv2.inRange(
                hsv, color_range["yellow"]["lower"], color_range["yellow"]["upper"]
            )
            mask = cv2.erode(mask, None, iterations=2)
            mask = cv2.dilate(mask, None, iterations=2)
            cnts = cv2.findContours(
                mask.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE
            )[-2]

            if len(cnts) > 0:
                c = max(cnts, key=cv2.contourArea)
                ((x, y), radius) = cv2.minEnclosingCircle(c)
                M = cv2.moments(c)
                center = (int(M["m10"] / M["m00"]), int(M["m01"] / M["m00"]))
                if radius > 10:
                    cv2.circle(frame, (int(x), int(y)), int(radius), (0, 255, 255), 2)
                    cv2.circle(frame, center, 5, (0, 0, 255), -1)
                    tracking_points["yellow"].appendleft(center)
                else:
                    tracking_points["yellow"] = deque(maxlen=args["buffer"])
            for i in range(1, len(tracking_points["yellow"])):
                if (
                    tracking_points["yellow"][i - 1] is None
                    or tracking_points["yellow"][i] is None
                ):
                    continue
                thickness = int(np.sqrt(args["buffer"] / float(i + 1)) * 2.5)
                if len(tracking_points["yellow"]) > 0:
                    cv2.line(
                        frame,
                        tracking_points["yellow"][i - 1],
                        tracking_points["yellow"][i],
                        (0, 0, 255),
                        thickness,
                    )
                    flag = 1
                    height, width, _ = frame.shape

                    x=center[0]/width
                    y=center[1]/height
                    send_data(client_socket, str(['c',2,x,y]), server_address)

        cv2.imshow("Frame", frame)
        key = cv2.waitKey(1) & 0xFF

        # if the 'q' key is pressed, stop the loop
        if key == ord("q"):
            break

    # cleanup the camera and close any open windows
    camera.release()
    cv2.destroyAllWindows()


server_address = ("127.0.0.1", 12345)
client_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)


taskOne = threading.Thread(target=cameraOne, args=(client_socket, server_address))

taskOne.start()
taskOne.join()
