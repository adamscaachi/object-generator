import argparse
from ultralytics import YOLO

def val(experiment_name):
    model_path = f"experiments/{experiment_name}/best.pt"
    config_path = f"experiments/{experiment_name}/train.yaml"
    model = YOLO(model_path)
    metrics = model.val(data=config_path)
    print(metrics.box.map)

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('experiment_name', type=str, help='Name of the experiment.')
    args = parser.parse_args()
    val(args.experiment_name)