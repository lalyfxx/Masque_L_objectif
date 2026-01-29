using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerMenu : MonoBehaviour
{
    [Header("Boutons menu principal")]
    public Button playButton;
    public Button settingsButton;   // ← nouveau
    public Button quitButton;

    [Header("Menus / Panneaux")]
    public GameObject settingsPanel;  // ← drag le SettingsPanel ici

    void Start()
    {
        // Boutons principaux
        if (playButton)    playButton.onClick.AddListener(LoadGameScene);
        if (quitButton)    quitButton.onClick.AddListener(QuitGame);
        if (settingsButton) settingsButton.onClick.AddListener(OpenSettings);

        // Cache le panneau settings au démarrage (sécurité)
        if (settingsPanel) settingsPanel.SetActive(false);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("Lily_Proto");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ────────────────────────────────────────────────
    // OUVRIR / FERMER SETTINGS
    // ────────────────────────────────────────────────
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    // À attacher au bouton "Retour" ou "Fermer" du SettingsPanel
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
}