using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhotoManager : MonoBehaviour {
    public Camera playerCam;
    public float rayDistance = 10f;
    public Image currentIcon;      
    public TextMeshProUGUI currentText;  

    private int currentTarget = 0;
    private string[] targetNames = {"Vase", "Livre", "Plante", "Chaussure", "Clé"};
    private Color[] targetColors = {Color.red, Color.blue, Color.green, new Color(0.6f,0.3f,0.1f), Color.yellow};

    void Start() {
        UpdateCurrentTask();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && CacheManager.Instance.CanShoot()) {
            Ray ray = playerCam.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance)) {
                PhotoTarget target = hit.collider.GetComponent<PhotoTarget>();
                if (target != null && target.targetID == currentTarget) {
                    TakePhoto();
                }
            }
        }
    }

    void TakePhoto() {
        currentIcon.color = new Color(1,1,1,0);
        currentText.text = "";

        currentTarget++;
        if (currentTarget >= 5) {
            GameManager.Instance.Win();
        } else {
            UpdateCurrentTask();
        }
    }

    void UpdateCurrentTask() {
        currentIcon.color = targetColors[currentTarget];
        currentText.text = "Photo de : " + targetNames[currentTarget];
    }
}