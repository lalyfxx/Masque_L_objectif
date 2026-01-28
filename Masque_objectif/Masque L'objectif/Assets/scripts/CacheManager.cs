using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CacheManager : MonoBehaviour
{
    public static CacheManager Instance { get; private set; }

    [Header("UI References")]
    public RectTransform cacheTransform;

    [Header("Vitesse Progressive")]
    [SerializeField] private float fullCoverDurationBase = 14f;    // Vitesse START (facile)
    [SerializeField] private float durationReductionPerCycle = 1.5f;  // ↓1.5s par cycle
    [SerializeField] private float minDuration = 6f;               // Vitesse MAX (difficile)
    
    [Header("Reset")]
    public float resetDuration = 0.5f;
    public float minDistanceToShoot = 0.25f;
    public float spawnRadius = 1200f;

    private Vector2 startPosition;
    private Tween currentTween;
    
    // 🚀 NOUVEAU : Compteur pour accélérer
    private int cycleCount = 0;
    private float currentDuration;

    void Awake()
    {
        Instance = this;
        ResetSpeed();  // Reset vitesse au démarrage
    }

    void Start()
    {
        if (cacheTransform == null)
        {
            Debug.LogError("Cache Transform non assigné !");
            return;
        }

        SetRandomStartPosition();
        cacheTransform.anchoredPosition = startPosition;
        StartMovingToCenter();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ResetCache();
        }
    }

    // 🚀 CALCUL VITESSE PROGRESSIVE
    private float GetCurrentDuration()
    {
        currentDuration = Mathf.Max(
            fullCoverDurationBase - (cycleCount * durationReductionPerCycle),
            minDuration
        );
        return currentDuration;
    }

    private void SetRandomStartPosition()
    {
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        startPosition = new Vector2(
            Mathf.Cos(randomAngle) * spawnRadius,
            Mathf.Sin(randomAngle) * spawnRadius
        );
    }

    private void StartMovingToCenter()
    {
        currentTween?.Kill();

        float duration = GetCurrentDuration();
        Debug.Log($"Cycle {cycleCount} : Vitesse = {duration:F1}s (plus rapide !)");

        currentTween = cacheTransform
            .DOAnchorPos(Vector2.zero, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => 
            {
                GameManager.Instance?.Lose();
            });
    }

    public void ResetCache()
    {
        currentTween?.Kill();

        Vector2 overshootTarget = startPosition * 1.15f;
        
        cacheTransform
            .DOAnchorPos(overshootTarget, resetDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // 🚀 ACCÉLÈRE pour le prochain cycle !
                cycleCount++;
                
                SetRandomStartPosition();
                cacheTransform.anchoredPosition = startPosition;
                StartMovingToCenter();
            });
    }

    // 🚀 RESET VITESSE (appelé au restart)
    public void ResetSpeed()
    {
        cycleCount = 0;
        Debug.Log("✅ Vitesse remise à zéro (facile)");
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