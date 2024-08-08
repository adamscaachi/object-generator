# object-generator

YOLO (You Only Look Once) models excel at detecting objects quickly, making them useful for applications that require object detection in real-time. However, achieving a good performance in a customised domain often relies on the model being fine-tuned to adapt to new objects, contexts, and variations not seen in the pre-training. This involves having access to a dataset with many labelled examples that are representative of the new domain, however such a dataset may not be easy or even possible to acquire. To circumvent this issue, the use of a 3D game engine to generate training data similar (but not identical) to the target domain is explored. 

## Generator

Multiple copies of an object's model are instantiated at random positions and angles in 3D space. The vertices of each object are obtained from their model's mesh and the 2D coordinates of each vertex's position on the screen is calculated. Taking the minimum and maximum values of each coordinate results in bounding boxes that encapsulate each object. An image of the scene is captured and a file containing the bounding box of each object in that image is saved. The scene is then reset and the process is repeated until the desired amount of data is obtained.

https://github.com/adamscaachi/object-generator/assets/44872869/195ce008-e1f3-40d2-a8ed-46e34b6a926e

## Experiments

Three experiments are conducted to investigate how synthetic data affects the performance of the model:
- a) Training with real data only (7 training images, 2 validation images).
- b) Training with synthetic data only (80 training images, 20 validation images).
- c) Training with the combined real and synthetic data used in the previous experiments (87 training images, 22 validation images).
  
All models are then evaluated quantitatively on a single testing image, and qualitatively on a video with 326 frames. 

## Results

The precision, recall, and F1 score evaluated using the testing image are plotted below for each of the model training strategies.

![metrics](https://github.com/user-attachments/assets/0f45c141-6b36-4142-a2d6-ac315a43ee0a)

The model trained with only real data has high precision but low recall across a range of confidence thresholds, showing that while it rarely misclassifies objects it does struggle to detect all of them. Conversely, the model trained with only synthetic data has high recall but low precision across the range of confidence thresholds, showing that while it succeeds at detecting a large proportion of the objects present it also makes additional erroneous detections. Both of these models have a range of confidence thresholds where the F1 score is reasonably good, however this range is quite small and deploying these models at such fine-tuned values is unlikely to generalise well to unseen data. The model trained with the real and synthetic data combined has both high precision and recall across the range of confidence thresholds, leading to a more robust F1 score that is less sensitive to confidence threshold adjustments.

The mean average precision (mAP) of each model evaluated at intersection over union (IoU) thresholds of 50% and 75% are shown below. In addition, the mAP evaluated at a range of IoU thresholds between 50% and 95% is also provided for each model.

<img src="https://github.com/user-attachments/assets/059217ac-d6e9-4357-ad7f-fc1273edebc8" width="400"/>

All models perform well with the relatively lenient criterion of 50% overlap, but when increasing the strictness of this criterion to 75% only the model trained using the combined dataset retains its mAP. Furthermore, when considering the model's performance over a range of detection strictness, the combined data training strategy also outperforms the others.

## Conclusion

A 3D game engine was used to generate synthetic data for training an object detection model. Using a combination of real and synthetic data resulted in a model with a more robust F1 score and a higher mAP compared to models trained using only real or only synthetic data. This was achieved despite the visible domain gap between the synthetic images and real images. With this approach, a reliable model was produced using only 10 manually labelled images. 

A demonstration of object detection using the model trained on the combined real and synthetic data is shown below.

https://github.com/adamscaachi/object-generator/assets/44872869/6828bdb4-22a5-4456-9f6f-cb77b0eb21cd
