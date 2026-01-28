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
    }

    public void Lose()
    {
        Debug.Log("💀 LOSE ! Cache a masqué l'objectif !");
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}