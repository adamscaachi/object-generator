from ultralytics import YOLO

def val():
    model = YOLO("model.pt")
    metrics = model.val(data="config.yaml")
    print(metrics.box.map)

if __name__ == '__main__':
    val()