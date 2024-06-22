from ultralytics import YOLO

def train():
    model = YOLO("yolov8n.pt")
    model.train(data="config.yaml", epochs=100, imgsz=640)

if __name__ == '__main__':
    train()
