import mediapipe as mp
import cv2
import socket
import threading
from deepface import DeepFace
import copy
from dollarpy import Recognizer, Template, Point
import pickle
from gaze_tracking import GazeTracking
import csv
import pandas as pd
import numpy as np
import seaborn as sns
import cv2
import matplotlib.pyplot as plt

import speech_recognition as sr
import pyttsx3
r = sr.Recognizer()

def load_recognizer_weights(filename):
    with open(filename, "rb") as f:
        templates = pickle.load(f)
    recognizer = Recognizer(templates)
    return recognizer


loaded_recognizer = load_recognizer_weights("recognizer_weights.pkl")


def receive_data(client_socket):
    while True:
        try:
            data, server_address = client_socket.recvfrom(1024)
            if not data:
                break
            print(f"Received from server: {data.decode('utf-8')}")
        except socket.error as e:
            print(f"Socket error: {e}")
            break


def send_data(client_socket, message, server_address):
    client_socket.sendto(message.encode("utf-8"), server_address)


def mediapipe_detection(image, model):
    image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    image.flags.writeable = False
    results = model.process(image)
    image.flags.writeable = True
    image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
    return image, results


mp_holistic = mp.solutions.holistic
drawing = mp.solutions.drawing_utils
gaze = GazeTracking()
webcam = cv2.VideoCapture(2)


def SpeakText(command):

    # Initialize the engine
    engine = pyttsx3.init()
    engine.say(command)
    engine.runAndWait()


def draw_landmarks(image, results):
    drawing.draw_landmarks(
        image,
        results.right_hand_landmarks,
        mp_holistic.HAND_CONNECTIONS,
        drawing.DrawingSpec(color=(55, 11, 66), thickness=2, circle_radius=2),
        drawing.DrawingSpec(color=(25, 66, 20), thickness=2, circle_radius=2),
    )


def getTheUserName(identity_str):
    extracted_str = ""
    started = False
    for char in identity_str:
        if char == "\\":
            started = True
        elif char == ".":
            break
        elif started:
            extracted_str += char
    return extracted_str


def cameraOne(client_socket, server_address):
    verification = 0
    emotion = 0
    ct_for_emotion = 0
    state = 0
    ct = 0
    ctTwo = 0

    cap = cv2.VideoCapture(2)

    hand_landmarks = []

    with mp_holistic.Holistic(
        min_detection_confidence=0.5, min_tracking_confidence=0.5
    ) as holistic:
        while cap.isOpened():
            ret, frame = cap.read()
            if not ret:
                break

            photo, landmarks = mediapipe_detection(frame, holistic)
            points = []

            if verification == 0:
                if ctTwo > 20:
                    dfs = DeepFace.find(
                        img_path=photo,
                        db_path="H:/HCI/final_test/data_set",
                        enforce_detection=False,
                    )
                    if len(dfs[0]["identity"]) > 0:
                        extracted_str = getTheUserName(dfs[0]["identity"][0])
                        
                        data = ["v: Hello: mr. ", extracted_str]
                        send_data(client_socket, str(data), server_address)
                        verification = 1

            if ct_for_emotion % 20 == 0:
                if emotion == 0:
                    dfs = DeepFace.analyze(
                        img_path=photo, actions=["emotion"], enforce_detection=False
                    )
                    if len(dfs[0]["dominant_emotion"]) > 0:
                        first_result = dfs[0]
                        e = first_result["dominant_emotion"]
                        print(e)

            ctTwo = ctTwo + 1
            if landmarks.right_hand_landmarks:
                PointOne = landmarks.right_hand_landmarks.landmark[0]
                PointTwo = landmarks.right_hand_landmarks.landmark[12]
                des = abs((PointTwo.y * 100) - (PointOne.y * 100))
                for landmark in landmarks.right_hand_landmarks.landmark:
                    points.append(landmark)
                for point in range(len(points)):
                    dollarpy_points = Point(points[point].x, points[point].y)
                    hand_landmarks.append(copy.deepcopy(dollarpy_points))

            draw_landmarks(photo, landmarks)

            if ct == 5:
                if hand_landmarks:
                    result = loaded_recognizer.recognize(hand_landmarks)
                    if des > 25 and result[0] == "open":
                        state = 0
                    elif des < 25 and result[0] == "close":
                        state = 1
                    print(state)
                hand_landmarks = []
                ct = 0

            if landmarks.right_hand_landmarks:
                x = landmarks.right_hand_landmarks.landmark[13].x
                y = landmarks.right_hand_landmarks.landmark[13].y
                data = []
                if state == 0:
                    data = str(["U", x, y])
                else:
                    data = str(["L", x, y])
                send_data(client_socket, data, server_address)
            ct += 1
            ct_for_emotion += 1

            gaze.refresh(photo)
            photo=gaze.annotated_frame()

            left_pupil = str(gaze.pupil_left_coords())
            right_pupil = str(gaze.pupil_right_coords())

            right_pupil=right_pupil.replace("(","")
            right_pupil=right_pupil.replace(")","")
            left_pupil=left_pupil.replace(")","")
            left_pupil=left_pupil.replace("(","")

            with open("output.csv", "a", newline="") as file:
                if left_pupil != "None" and right_pupil != "None":
                    writer = csv.writer(file)
                    writer.writerow(left_pupil.split(","))
                    writer.writerow(right_pupil.split(","))

            cv2.imshow("OpenCV Feed", photo)

            if cv2.waitKey(10) & 0xFF == ord("q"):
                break

    cap.release()
    cv2.destroyAllWindows()


def cameraTwo(client_socket):
    pass


def speechThread(client_socket, server_address):
    while 1:

        # Exception handling to handle
        # exceptions at the runtime
        try:

            # use the microphone as source for input.
            with sr.Microphone() as source2:

                # wait for a second to let the recognizer
                # adjust the energy threshold based on
                # the surrounding noise level
                r.adjust_for_ambient_noise(source2, duration=0.2)

                # listens for the user's input
                audio2 = r.listen(source2)

                # Using google to recognize audio
                MyText = r.recognize_google(audio2)
                MyText = MyText.lower()

                print("Did you say ", MyText)

                data = str(["S", MyText])
                
                
                send_data(client_socket, data, server_address)
                # SpeakText(MyText)

        except sr.RequestError as e:
            print("Could not request results; {0}".format(e))

        except sr.UnknownValueError:
            print("unknown error occurred")




server_address = ("127.0.0.1", 12345)
client_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)



taskOne = threading.Thread(target=cameraOne, args=(client_socket, server_address))



taskSpeech = threading.Thread(target=speechThread, args=(client_socket, server_address))
receive = threading.Thread(target=receive_data, args=(client_socket, server_address))


taskOne.start()
taskSpeech.start()


taskOne.join()


eye_data = pd.read_csv("output.csv", header=None, names=["X", "Y"])

img = cv2.imread("test.png")
img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)

# Create a figure and axis
fig, ax = plt.subplots(figsize=(10, 8))
# Display the background image
ax.imshow(img, extent=[0, img.shape[1], 0, img.shape[0]], origin="upper")
# Create heatmap using kernel density estimation
sns.kdeplot(
    x=eye_data["X"],
    y=eye_data["Y"],
    fill=True,
    cmap="plasma",
    cbar=False,
    ax=ax,
    alpha=0.6,
)  # viridis,RdBu_r,rainbow
# Remove ticks and labels
ax.set_xticks([])
ax.set_yticks([])
ax.set_frame_on(False)
# Save the plot
plt.savefig("heatmap_plot_with_image.png", bbox_inches="tight", pad_inches=0)
# Show the plot


plt.show()

client_socket.close()
