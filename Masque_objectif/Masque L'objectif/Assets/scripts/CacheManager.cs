using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CacheManager : MonoBehaviour
{
    public static CacheManager Instance { get; private set; }

    [Header("UI References")]
    public RectTransform cacheTransform;     // ← ton Image / RectTransform du cache
    public float fullCoverDuration = 14f;    // temps pour couvrir complètement
    public float resetDuration = 0.5f;       // durée du retour rapide
    public float minDistanceToShoot = 0.25f; // distance au centre pour pouvoir encore shooter

    private Vector2 startPosition;
    private Tween currentTween;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (cacheTransform == null)
        {
            Debug.LogError("Cache Transform non assigné dans CacheManager !");
            return;
        }

        // Position de départ (exemple : à gauche, hors écran)
        startPosition = new Vector2(-1500f, 0f); // ajuste selon ton écran
        cacheTransform.anchoredPosition = startPosition;

        StartMovingToCenter();
    }

    void Update()
    {
        // Contrôle principal : clic droit pour repousser
        if (Input.GetMouseButtonDown(1))
        {
            ResetCache();
        }
    }

    private void StartMovingToCenter()
    {
        currentTween?.Kill();

        // Mouvement vers le centre (0,0)
        currentTween = cacheTransform
            .DOAnchorPos(Vector2.zero, fullCoverDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => 
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.Lose();
            });
    }

    public void ResetCache()
    {
        currentTween?.Kill();

        // Repousse vers la position de départ + un peu plus loin
        cacheTransform
            .DOAnchorPos(startPosition * 1.15f, resetDuration)  // overshoot léger
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // Optionnel : repartir dans l'autre sens ou relancer
                startPosition *= -1; // change de côté à chaque fois (gauche ↔ droite)
                StartMovingToCenter();
            });
    }

    public bool CanShoot()
    {
        if (cacheTransform == null) return true;
        float dist = Vector2.Distance(cacheTransform.anchoredPosition, Vector2.zero);
        return dist > (Screen.width * minDistanceToShoot * 0.5f);
    }

    void OnDestroy()
    {
        currentTween?.Kill();
    }
}