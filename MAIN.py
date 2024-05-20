from deepface import DeepFace


def getTheUserName(identity_str):
    extracted_str = ""
    started = False
    for char in identity_str:
        if char == "\":
            started = True
        elif char == ".":
            break
        elif started:
            extracted_str += char
    return extracted_str

dfs = DeepFace.find(
                        img_path="Doha.png",
                        db_path="H:/HCI/final_test/data_set",
                        enforce_detection=False,
                    )
if len(dfs[0]["identity"]) > 0:
    extracted_str = getTheUserName(dfs[0]["identity"][0])

    data = ["v: Hello: mr. ", extracted_str]



