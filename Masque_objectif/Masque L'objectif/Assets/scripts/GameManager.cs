using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // optionnel, mais souvent utile pour les managers
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Win()
    {
        Debug.Log("🎉 WIN ! 5 photos prises ! Perfect Shot !");
        Time.timeScale = 0f;

        // Réinitialise la vitesse du cache pour une nouvelle partie
        if (CacheManager.Instance != null)
        {
            CacheManager.Instance.ResetSpeed();
        }
    }

    public void Lose()
    {
        Debug.Log("💀 LOSE ! Cache a masqué l'objectif !");
        Time.timeScale = 0f;

        // Réinitialise la vitesse du cache pour une nouvelle partie
        if (CacheManager.Instance != null)
        {
            CacheManager.Instance.ResetSpeed();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            
            // Réinitialise la vitesse du cache avant de recharger la scène
            if (CacheManager.Instance != null)
            {
                CacheManager.Instance.ResetSpeed();
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}