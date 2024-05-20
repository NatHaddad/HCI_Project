from deepface import Deepface


dfs = Deepface.analyze(
            img_path="nat.png", actions=["emotion"], enforce_detection = False 
            )
if len(dfs[0]["dominant_emotion"]) > 0:
    first_result = dfs[0]
    e = first_result["dominant _emotion"]
print(e)