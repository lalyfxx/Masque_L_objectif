using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;  // ← NEEDED pour le flash smooth

public class PhotoManager : MonoBehaviour
{
    [Header("Références")]
    public Camera playerCam;
    public float rayDistance = 20f;
    public LayerMask photoTargetLayer;

    [Header("UI")]
    public Image currentIcon;
    public TextMeshProUGUI currentText;

    [Header("🎥 FLASH PHOTO EFFECT")]
    public Image photoFlash;           // ← Drag ton Image Flash (blanc plein écran)

    [Header("Debug")]
    public bool drawPermanentRay = true;

    public int currentTarget = 0;
    private string[] targetNames = { "Vase", "Livre", "Plante", "Chaussure", "Clé" };
    private Color[] targetColors = { Color.red, Color.blue, Color.green, new Color(0.6f, 0.3f, 0.1f), Color.yellow };

    public static PhotoManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateCurrentTask();
        
        // Cache le flash au démarrage
        if (photoFlash != null)
            photoFlash.gameObject.SetActive(false);
    }

    void Update()
    {
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

                PhotoTarget target = hit.collider.GetComponentInParent<PhotoTarget>();

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
        // 🚀 FLASH PHOTO JUICY !
        StartCoroutine(PhotoFlashEffect());

        // UI task disparaît
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

    // ✨ EFFET FLASH PHOTO (0.2s total, ultra impactant)
    private IEnumerator PhotoFlashEffect()
    {
        if (photoFlash == null) yield break;

        // Active + couleur de départ (blanc chaud, invisible)
        photoFlash.gameObject.SetActive(true);
        photoFlash.color = new Color(0.98f, 0.92f, 0.8f, 0f);  // blanc légèrement chaud

        // 1. FLASH INTENSE (0 → 1.3 en 0.08s)
        photoFlash.DOFade(1.3f, 0.08f);

        yield return new WaitForSeconds(0.08f);

        // 2. FADE OUT RAPIDE (1.3 → 0 en 0.12s)
        yield return photoFlash.DOFade(0f, 0.12f).WaitForCompletion();

        // 3. Cache
        photoFlash.gameObject.SetActive(false);
    }

    void UpdateCurrentTask()
    {
        currentIcon.color = targetColors[currentTarget];
        currentText.text = "Photo de : " + targetNames[currentTarget];
    }

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