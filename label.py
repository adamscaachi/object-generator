import glob
from ultralytics import YOLO

def generate_pseudo_labels():
    model = YOLO("experiments/b/best.pt")
    image_paths = glob.glob("experiments/d/target/images/*.png")
    for i, image in enumerate(image_paths):
        results = model.predict(image, iou=0.6, conf=0.8)
        label_path = "experiments/d/target/labels/sheep_" + str(i) + ".txt"
        with open(label_path, 'w') as f:
            for r in results:
                boxes = r.boxes.xywhn.cpu().numpy()
                for x, y, w, h in boxes:
                    label = f"0 {x:.6f} {y:.6f} {w:.6f} {h:.6f}"
                    f.write(label + "\n")

if __name__ == '__main__':
    generate_pseudo_labels()