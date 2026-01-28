using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CacheManager : MonoBehaviour {
    public static CacheManager Instance;
    public RectTransform cacheRect; // Drag ton Image UI
    public float growTime = 12f; // Temps full cover
    public float minScaleForShoot = 0.2f;

    private float timer = 0f;

    void Awake() { Instance = this; transform.localScale = Vector3.zero; }

    void Update() {
        if (Input.GetMouseButtonDown(1)) { // Clic droit : Retire !
            ResetCache();
        }

        timer += Time.deltaTime;
        float progress = timer / growTime;
        if (progress >= 1f) {
            GameManager.Instance.Lose();
        } else {
            cacheRect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
        }
    }

    public bool CanShoot() {
        return cacheRect.localScale.x < minScaleForShoot;
    }

    public void ResetCache() {
        timer = 0f;
        cacheRect.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
    }
}