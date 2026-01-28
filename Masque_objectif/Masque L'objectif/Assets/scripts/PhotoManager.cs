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

    void Update()
{
    if (Input.GetMouseButtonDown(0) && CacheManager.Instance.CanShoot())
    {
        // ────────────────────────────────────────────────
        // VISUAL DEBUG : ligne rouge depuis la caméra
        // ────────────────────────────────────────────────
        Ray debugRay = playerCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        Debug.DrawRay(debugRay.origin, debugRay.direction * rayDistance, Color.red, 5f);  // reste 5 secondes

        if (Physics.Raycast(debugRay, out RaycastHit hit, rayDistance))
        {
            Debug.Log("Touché : " + hit.collider.name + "   (layer = " + LayerMask.LayerToName(hit.collider.gameObject.layer) + ")");
            Debug.DrawRay(debugRay.origin, debugRay.direction * hit.distance, Color.green, 2f);

            PhotoTarget target = hit.collider.GetComponent<PhotoTarget>();
            if (target != null && target.targetID == currentTarget)
            {
                Debug.Log("→ PhotoTarget valide ! ID = " + target.targetID);
                TakePhoto();
            }
            else
            {
                Debug.LogWarning("Touché mais pas de PhotoTarget ou mauvais ID");
            }
        }
        else
        {
            Debug.LogWarning("Raycast n'a rien touché dans " + rayDistance + " mètres");
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