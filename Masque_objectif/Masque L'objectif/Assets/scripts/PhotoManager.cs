using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;

public class PhotoManager : MonoBehaviour
{
    [Header("Références")]
    public Camera playerCam;
    public float rayDistance = 20f;
    public LayerMask photoTargetLayer;

    [Header("UI")]
    public Image currentIcon;
    public TextMeshProUGUI currentText;

    [Header("🎥 FLASH PHOTO EFFECT (bonne photo)")]
    public Image photoFlash;

    [Header("🚫 FEEDBACK PHOTO RATÉE")]
    public Image redFlashOverlay;
    [SerializeField] private float missShakeDuration = 0.22f;
    [SerializeField] private float missShakeStrength = 0.9f;
    [SerializeField] private float missRedFlashDuration = 0.20f;

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
        
        if (photoFlash != null)
            photoFlash.gameObject.SetActive(false);

        if (redFlashOverlay != null)
            redFlashOverlay.gameObject.SetActive(false);
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
                        StartCoroutine(FeedbackMissedPhoto());
                    }
                }
                else
                {
                    Debug.LogWarning("→ Touché mais PAS de PhotoTarget sur cet objet ou ses parents");
                    StartCoroutine(FeedbackMissedPhoto());
                }

                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 2f);
            }
            else
            {
                Debug.LogWarning($"RIEN TOUCHÉ (LayerMask = {LayerMask.LayerToName(photoTargetLayer.value)})");
                StartCoroutine(FeedbackMissedPhoto());
            }
        }
    }

    void TakePhoto()
    {
        StartCoroutine(PhotoFlashEffect());

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

    // ✨ FLASH PHOTO (bonne photo)
    private IEnumerator PhotoFlashEffect()
    {
        if (photoFlash == null) yield break;

        photoFlash.gameObject.SetActive(true);
        photoFlash.color = new Color(0.98f, 0.92f, 0.8f, 0f);

        photoFlash.DOFade(1.3f, 0.08f);
        yield return new WaitForSeconds(0.08f);
        yield return photoFlash.DOFade(0f, 0.12f).WaitForCompletion();

        photoFlash.gameObject.SetActive(false);
    }

    // 🚫 FEEDBACK PHOTO RATÉE → corrigé : sur playerCam.transform
    private IEnumerator FeedbackMissedPhoto()
    {
        if (playerCam == null) yield break;

        Vector3 originalLocalPos = playerCam.transform.localPosition;

        // Shake position sur le Transform de la caméra
        playerCam.transform.DOShakePosition(
            duration: missShakeDuration,
            strength: new Vector3(missShakeStrength, missShakeStrength, 0),
            vibrato: 12,
            randomness: 65,
            snapping: false,
            fadeOut: true
        );

        // Shake rotation sur le Transform de la caméra
        playerCam.transform.DOShakeRotation(
            duration: missShakeDuration * 0.7f,
            strength: new Vector3(1.5f, 1.5f, 1f),
            vibrato: 10,
            randomness: 70,
            fadeOut: true
        );

        yield return new WaitForSeconds(missShakeDuration);

        // Reset position (sécurité)
        playerCam.transform.localPosition = originalLocalPos;

        // Flash rouge
        if (redFlashOverlay != null)
        {
            redFlashOverlay.gameObject.SetActive(true);
            redFlashOverlay.color = new Color(0.9f, 0.2f, 0.2f, 0f);

            redFlashOverlay.DOFade(0.45f, missRedFlashDuration * 0.4f);
            yield return new WaitForSeconds(missRedFlashDuration * 0.4f);

            yield return redFlashOverlay.DOFade(0f, missRedFlashDuration * 0.6f).WaitForCompletion();

            redFlashOverlay.gameObject.SetActive(false);
        }
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