using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CacheManager : MonoBehaviour
{
    public static CacheManager Instance;

    public RectTransform cacheRect;
    public float growTime = 12f;
    public float minScaleForShoot = 0.2f;

    private float timer = 0f;

    void Awake()
    {
        Instance = this;
        if (cacheRect != null) cacheRect.localScale = Vector3.zero;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ResetCache();
        }

        timer += Time.deltaTime;
        float progress = timer / growTime;

        if (progress >= 1f)
        {
            GameManager.Instance.Lose();
        }
        else
        {
            if (cacheRect != null)
                cacheRect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
        }
    }

    public bool CanShoot()
    {
        if (cacheRect == null) return true;
        return cacheRect.localScale.x < minScaleForShoot;
    }

    public void ResetCache()
    {
        timer = 0f;
        if (cacheRect != null)
            cacheRect.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
    }
}