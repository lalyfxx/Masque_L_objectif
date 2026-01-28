using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhotoManager : MonoBehaviour
{
    [Header("Références")]
    public Camera playerCam;
    public float rayDistance = 20f;
    public LayerMask photoTargetLayer;          // ← drag le layer "Targets" ici dans l'Inspector

    [Header("UI")]
    public Image currentIcon;
    public TextMeshProUGUI currentText;

    [Header("Debug")]
    public bool drawPermanentRay = true;        // pour voir la ligne rouge en permanence

    public int currentTarget = 0;
    private string[] targetNames = { "Vase", "Livre", "Plante", "Chaussure", "Clé" };
    private Color[] targetColors = { Color.red, Color.blue, Color.green, new Color(0.6f, 0.3f, 0.1f), Color.yellow };

    // Pour que PhotoTarget puisse lire currentTarget sans FindObjectOfType
    public static PhotoManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateCurrentTask();
    }

    void Update()
    {
        // Ligne rouge permanente en Scene view (très utile pour debug)
        if (drawPermanentRay && playerCam != null)
        {
            Ray debugRay = playerCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            Debug.DrawRay(debugRay.origin, debugRay.direction * rayDistance, Color.red);
        }

        if (Input.GetMouseButtonDown(0) && CacheManager.Instance.CanShoot())
        {
            Ray ray = playerCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, photoTargetLayer))
            {
                GameObject touched = hit.collider.gameObject;

                Debug.Log($"[HIT] Objet touché : {touched.name}", touched);
                Debug.Log($"[HIT] Chemin hiérarchie : {GetFullPath(touched.transform)}", touched);
                Debug.Log($"[HIT] Tag : {touched.tag}   |   Layer : {LayerMask.LayerToName(touched.layer)}", touched);

                PhotoTarget target = hit.collider.GetComponentInParent<PhotoTarget>(); // InParent au cas où collider sur enfant

                if (target != null)
                {
                    Debug.Log($"[HIT] PhotoTarget trouvé → ID = {target.targetID}  (cible actuelle = {currentTarget})", target.gameObject);

                    if (target.targetID == currentTarget)
                    {
                        Debug.Log("→ PHOTO VALIDE !", target.gameObject);
                        TakePhoto();
                    }
                    else
                    {
                        Debug.LogWarning($"→ Mauvais ID (attendu {currentTarget}, reçu {target.targetID})");
                    }
                }
                else
                {
                    Debug.LogWarning("→ Touché mais PAS de PhotoTarget sur cet objet ou ses parents");
                }

                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 2f);
            }
            else
            {
                Debug.LogWarning($"RIEN TOUCHÉ (LayerMask = {LayerMask.LayerToName(photoTargetLayer.value)})");
            }
        }
    }

    void TakePhoto()
    {
        currentIcon.color = new Color(1, 1, 1, 0);
        currentText.text = "";

        currentTarget++;
        if (currentTarget >= 5)
        {
            GameManager.Instance.Win();
        }
        else
        {
            UpdateCurrentTask();
        }
    }

    void UpdateCurrentTask()
    {
        currentIcon.color = targetColors[currentTarget];
        currentText.text = "Photo de : " + targetNames[currentTarget];
    }

    // Petite fonction utilitaire pour le debug
    private string GetFullPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}