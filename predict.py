from ultralytics import YOLO

def predict():
    model = YOLO("model.pt")
    results = model.predict(source='sheep.MP4', stream=True, iou=0.6, conf=0.5, save=True, line_width=1, show_labels=False)
    for r in results:
        next(results)

if __name__ == '__main__':
    predict()
