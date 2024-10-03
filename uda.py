import os
import glob
import shutil
from ultralytics import YOLO

def train_source_model(base_dir):
    model = YOLO("yolov8n.pt") # consider using a larger model here
    config_path = os.path.join(base_dir, "source.yaml")
    model.train(data=config_path, epochs=100, imgsz=640)

def get_target_directories(base_dir):
    return {
        "train_images": os.path.join(base_dir, "target/train/images"),
        "val_images": os.path.join(base_dir, "target/val/images"),
        "train_labels": os.path.join(base_dir, "target/train/labels"),
        "val_labels": os.path.join(base_dir, "target/val/labels")
    }

def get_combined_directories(base_dir):
    return {
        "train_images": os.path.join(base_dir, "combined/train/images"),
        "train_labels": os.path.join(base_dir, "combined/train/labels"),
    }

def get_source_directories(base_dir):
    return {
        "train_images": os.path.join(base_dir, "source/train/images"),
        "train_labels": os.path.join(base_dir, "source/train/labels"),
    }

def create_label_directories(label_dirs):  
    for label_dir in label_dirs:
        os.makedirs(label_dir, exist_ok=True)

def create_combined_directories(combined_dirs):
    for combined_dir in combined_dirs:
        os.makedirs(combined_dir, exist_ok=True)

def get_image_paths(image_dirs):
    image_paths = []
    for image_dir in image_dirs:
        image_paths.extend(glob.glob(os.path.join(image_dir, "*.png")))
    return image_paths

def generate_pseudo_labels(base_dir):
    model = YOLO(os.path.join(base_dir, "source.pt"))
    target_dirs = get_target_directories(base_dir)
    create_label_directories([target_dirs["train_labels"], target_dirs["val_labels"]])
    image_paths = get_image_paths([target_dirs["train_images"], target_dirs["val_images"]])
    for image_path in image_paths:
        results = model.predict(image_path, iou=0.6, conf=0.8)
        label_path = image_path.replace("images", "labels").replace(".png", ".txt")
        with open(label_path, 'w') as f:
            for r in results:
                for x, y, w, h in r.boxes.xywhn.cpu().numpy():
                    f.write(f"0 {x:.6f} {y:.6f} {w:.6f} {h:.6f}\n")

def copy_files(src_dir, dst_dir):
    for item in os.listdir(src_dir):
        src_path = os.path.join(src_dir, item)
        dst_path = os.path.join(dst_dir, item)
        shutil.copy2(src_path, dst_path)

def combine_data(base_dir):
    combined_dirs = get_combined_directories(base_dir)
    target_dirs = get_target_directories(base_dir)
    source_dirs = get_source_directories(base_dir)
    create_combined_directories([combined_dirs["train_images"], combined_dirs["train_labels"]])
    copy_files(target_dirs["train_images"], combined_dirs["train_images"])
    copy_files(target_dirs["train_labels"], combined_dirs["train_labels"])
    copy_files(source_dirs["train_images"], combined_dirs["train_images"])
    copy_files(source_dirs["train_labels"], combined_dirs["train_labels"])

def train_target_model(base_dir):
    model = YOLO(os.path.join(base_dir, "source.pt")) # experiment with yolo.pt vs source.pt
    config_path = os.path.join(base_dir, "adapt.yaml")
    model.train(data=config_path, epochs=100, imgsz=640)

if __name__ == '__main__':
    base_dir = "experiments/e"
    #train_source_model(base_dir)
    generate_pseudo_labels(base_dir)
    combine_data(base_dir)
    train_target_model(base_dir)