using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Écrans de fin (dans la même scène)")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Bouton Rejouer (optionnel)")]
    public Button winRestartButton;
    public Button loseRestartButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
    }

    void Start()
    {
        if (winRestartButton) winRestartButton.onClick.AddListener(RestartGame);
        if (loseRestartButton) loseRestartButton.onClick.AddListener(RestartGame);
    }

    public void Win()
    {
        Debug.Log("🎉 WIN ! 5 photos prises ! Perfect Shot !");

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            CanvasGroup cg = winPanel.GetComponent<CanvasGroup>() ?? winPanel.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            DOTween.To(() => cg.alpha, x => cg.alpha = x, 1f, 0.6f)
                   .SetEase(Ease.OutQuad);
        }

        Time.timeScale = 0f;

        if (CacheManager.Instance != null)
            CacheManager.Instance.ResetSpeed();
    }

    public void Lose()
    {
        Debug.Log("💀 LOSE ! Cache a masqué l'objectif !");

        if (losePanel != null)
        {
            losePanel.SetActive(true);
            CanvasGroup cg = losePanel.GetComponent<CanvasGroup>() ?? losePanel.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            DOTween.To(() => cg.alpha, x => cg.alpha = x, 1f, 0.6f)
                   .SetEase(Ease.OutQuad);
        }

        Time.timeScale = 0f;

        if (CacheManager.Instance != null)
            CacheManager.Instance.ResetSpeed();
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;

        if (CacheManager.Instance != null)
            CacheManager.Instance.ResetSpeed();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ────────────── NOUVEAU : QUITTER LE JEU ──────────────
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
}