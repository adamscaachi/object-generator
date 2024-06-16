using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RunManager : MonoBehaviour{

    public GameObject object_prefab;
    public TMP_InputField number_of_images_field;
    public TMP_InputField objects_per_image_field;
    public Toggle draw_boxes_toggle;
    public Button generate_button;
    public Button exit_button;
    public TMP_Text report;
    private bool generatorOn = false;
    private int number_of_images;
    private int objects_per_image;
    private bool draw_boxes;
    private int eventID = 0;
    private int threshold;
    private string project_dir = System.IO.Path.GetDirectoryName(Application.dataPath);
    private string train_image_dir;
    private string train_label_dir;
    private string val_image_dir;
    private string val_label_dir;
    private List<GameObject> instantiated_objects = new List<GameObject>();
    private List<Rect> bounding_boxes = new List<Rect>();

    void Start(){
	train_image_dir = System.IO.Path.Combine(project_dir, @"Output\train\images");
	train_label_dir = System.IO.Path.Combine(project_dir, @"Output\train\labels");
	val_image_dir = System.IO.Path.Combine(project_dir, @"Output\val\images");
	val_label_dir = System.IO.Path.Combine(project_dir, @"Output\val\labels");
	string[] directories = new string[]{train_image_dir, train_label_dir, val_image_dir, val_label_dir};
	foreach (string dir in directories){
	    if (!Directory.Exists(dir)){
	        Directory.CreateDirectory(dir);
	    }
	}
    }

    public void BeginOfRun(){
	number_of_images_field.gameObject.SetActive(false);
	objects_per_image_field.gameObject.SetActive(false);
	draw_boxes_toggle.gameObject.SetActive(false);
	generate_button.gameObject.SetActive(false);
	number_of_images = GetValue(number_of_images_field);
    	threshold = (int)(0.8 * number_of_images);
	objects_per_image = GetValue(objects_per_image_field);
	draw_boxes = draw_boxes_toggle.isOn;
	generatorOn = true;
    }

    void Update(){
	if (generatorOn){
	    Reset();
	    if (eventID < number_of_images){
		InstantiateObjects();
		CalculateBoundingBoxes();
		RecordEvent();
	        eventID++;
	    } else {
	        EndOfRun();
	    }
	}
    }

    void InstantiateObjects(){
	for (int i = 0; i < objects_per_image; i++){
	    Vector3 position = new Vector3(Random.Range(-20f, 20f), 0f, Random.Range(-10f, 10f));
	    Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 359f), 0);
	    GameObject obj = Instantiate(object_prefab, position, rotation);
	    instantiated_objects.Add(obj);
   	}
    }

    void CalculateBoundingBoxes(){
	foreach (GameObject obj in instantiated_objects){
	    MeshFilter mesh_filter = obj.GetComponent<MeshFilter>();
	    Vector3[] vertices = mesh_filter.sharedMesh.vertices;
	    Vector3 center_point = Camera.main.WorldToScreenPoint(obj.transform.position);
	    Vector3 screen_min = center_point;
	    Vector3 screen_max = center_point;
	    foreach (Vector3 vertex in vertices){
	        Vector3 world_point = obj.transform.TransformPoint(vertex);
	        Vector3 screen_point = Camera.main.WorldToScreenPoint(world_point);
	        screen_min = Vector3.Min(screen_min, screen_point);
	        screen_max = Vector3.Max(screen_max, screen_point);
	    }        
	    if (screen_min.x > 0 && screen_min.y > 0 && screen_max.x < Screen.width && screen_max.y < Screen.height){
		float x = screen_min.x;
		float y = Screen.height - screen_max.y;
		float w = screen_max.x - screen_min.x;
		float h = screen_max.y - screen_min.y;
                Rect bounding_box = new Rect(x, y, w, h);
	        bounding_boxes.Add(bounding_box);
	    } else {
		Destroy(obj);
	    }
	}
    }

    void RecordEvent(){
	string image_name = "datapoint_" + eventID.ToString() + ".png";
	string label_name = "datapoint_" + eventID.ToString() + ".txt";
	string image_path, label_path;
	if (eventID < threshold){
	    image_path = System.IO.Path.Combine(train_image_dir, image_name);
	    label_path = System.IO.Path.Combine(train_label_dir, label_name);
	} else {
	    image_path = System.IO.Path.Combine(val_image_dir, image_name);
	    label_path = System.IO.Path.Combine(val_label_dir, label_name);
	}
	ScreenCapture.CaptureScreenshot(image_path);
	using (StreamWriter writer = new StreamWriter(label_path, false)){
	    foreach (var box in bounding_boxes){
		float x = (box.x + box.width/2) / Screen.width;
		float y = (box.y + box.height/2) / Screen.height;
		float w = box.width / Screen.width;
		float h = box.height / Screen.height;
	        string label = "0 " + x.ToString("F6") + " " + y.ToString("F6") + 
			       " " + w.ToString("F6") + " " + h.ToString("F6");
		writer.WriteLine(label);
	    }
	}
    }

    void Reset(){
	foreach (GameObject obj in instantiated_objects){
	    Destroy(obj);
	}
	instantiated_objects.Clear();
	bounding_boxes.Clear();
    }

    void EndOfRun(){
	exit_button.gameObject.SetActive(true);
	string report_text = "\n" + 
		             "Run Complete!" + "\n\n" +
			     "Images generated: " + number_of_images + "\n" + 
			     "Objects per image: " + objects_per_image + "\n" + 
			     "Bounding boxes drawn: " + draw_boxes + "\n" + 
			     "Data saved to: " + project_dir + "\\Output";
	report.text = report_text;
    }

    public void OnExitButtonClick(){
        UnityEditor.EditorApplication.isPlaying = false;
    }

    int GetValue(TMP_InputField inputField){
	if (int.TryParse(inputField.text, out int value)){
	    return value;
	} else{
	    Debug.LogError($"Invalid input in {inputField.gameObject.name}.");
	    return 0;
	}
    }

    void OnGUI(){
	if (draw_boxes){
	    foreach (var box in bounding_boxes){
		GUI.Box(box, GUIContent.none);
	    }
	}
    }

}
