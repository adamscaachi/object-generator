import argparse
from ultralytics import YOLO

def train(experiment_name):
    model = YOLO("yolov8n.pt")
    config_path = f"experiments/{experiment_name}/train.yaml"
    model.train(data=config_path, epochs=100, imgsz=640)

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('experiment_name', type=str, help='Name of the experiment.')
    args = parser.parse_args()
    train(args.experiment_name)
