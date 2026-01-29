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

    [Header("UI - Tâche actuelle")]
    public Image currentIcon;                  // L'icône qui reste toujours blanche
    public TextMeshProUGUI currentText;

    [Header("UI - Checkmark succès")]
    public Image successCheckImage;            // ← Drag ici ton image du V vert
    [SerializeField] private float checkDisplayTime = 1.2f;   // Combien de temps le V reste visible

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
    // 🚀 14 objets en ANGLAIS
    private string[] targetNames = {
        "TV", "Photo frame", "Trash can", "Baguette", "Tissue box", 
        "Yellow book", "Camembert", "Bottle", "Computer", "Handbag", 
        "Wine glass", "Pillow", "Plant", "Clothes hanger"
    };

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

        // Cacher le checkmark au démarrage
        if (successCheckImage != null)
            successCheckImage.gameObject.SetActive(false);
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

        // ────────────── AFFICHAGE DU V VERT ──────────────
        if (successCheckImage != null)
        {
            successCheckImage.gameObject.SetActive(true);
            successCheckImage.color = new Color(1, 1, 1, 0f); // départ transparent

            // Fade in + léger pop
            successCheckImage.DOFade(1f, 0.25f);
            successCheckImage.transform.DOScale(1.2f, 0.25f).SetEase(Ease.OutBack);

            // Attend + fade out
            StartCoroutine(HideCheckAfterDelay());
        }

        // On vide le texte et on passe à la suivante
        currentText.text = "";
        currentTarget++;

        // 🚀 CHANGE : 14 objets au lieu de 5
        if (currentTarget >= 14)
        {
            GameManager.Instance.Win();
        }
        else
        {
            UpdateCurrentTask();
        }
    }

    private IEnumerator HideCheckAfterDelay()
    {
        yield return new WaitForSeconds(checkDisplayTime);

        if (successCheckImage != null)
        {
            successCheckImage.DOFade(0f, 0.3f);
            successCheckImage.transform.DOScale(0.8f, 0.3f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(0.3f);
            successCheckImage.gameObject.SetActive(false);
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

    // 🚫 FEEDBACK PHOTO RATÉE
    private IEnumerator FeedbackMissedPhoto()
    {
        if (playerCam == null) yield break;

        Vector3 originalLocalPos = playerCam.transform.localPosition;

        playerCam.transform.DOShakePosition(
            duration: missShakeDuration,
            strength: new Vector3(missShakeStrength, missShakeStrength, 0),
            vibrato: 12,
            randomness: 65,
            snapping: false,
            fadeOut: true
        );

        playerCam.transform.DOShakeRotation(
            duration: missShakeDuration * 0.7f,
            strength: new Vector3(1.5f, 1.5f, 1f),
            vibrato: 10,
            randomness: 70,
            fadeOut: true
        );

        yield return new WaitForSeconds(missShakeDuration);

        playerCam.transform.localPosition = originalLocalPos;

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
        // L'icône reste TOUJOURS blanche
        if (currentIcon != null)
        {
            currentIcon.color = Color.white;
        }

        currentText.text = "Take photo of: " + targetNames[currentTarget];
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