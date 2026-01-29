using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class Countdown : MonoBehaviour
{
    [Header("Références UI")]
    public TextMeshProUGUI countdownText;          // Le texte qui affiche 3, 2, 1, GO!
    public GameObject countdownCanvas;             // Canvas ou panel du countdown

    [Header("Instructions avant countdown")]
    public GameObject instructionsPanel;           // ← Drag ton panel/image des commandes ici
    [SerializeField] private float instructionsDisplayTime = 2.5f;  // Durée d'affichage
    [SerializeField] private float instructionsFadeTime = 0.4f;     // Temps du fade in/out

    [Header("Timing Countdown")]
    [SerializeField] private float timeFor321 = 1f;     // Temps par chiffre (3,2,1)
    [SerializeField] private float timeForGO = 1.2f;    // Temps total pour GO! avec anim

    void Start()
    {
        // Vérifications
        if (countdownText == null || countdownCanvas == null)
        {
            Debug.LogError("CountdownText ou CountdownCanvas manquant !");
            return;
        }

        // Sécurité : tout caché au départ
        countdownCanvas.SetActive(false);
        countdownText.alpha = 0f;

        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }

        // Lancement de la séquence
        StartCoroutine(PlayIntroAndCountdown());
    }

    private IEnumerator PlayIntroAndCountdown()
    {
        Time.timeScale = 0f; // Pause complète du jeu

        // ──────────────────────────────
        // 1. Affichage des instructions (2.5s)
        // ──────────────────────────────
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);

            CanvasGroup cg = instructionsPanel.GetComponent<CanvasGroup>();
            if (cg == null) cg = instructionsPanel.AddComponent<CanvasGroup>();

            cg.alpha = 0f;
            cg.DOFade(1f, instructionsFadeTime);  // Fade in

            yield return new WaitForSecondsRealtime(instructionsDisplayTime);

            cg.DOFade(0f, instructionsFadeTime);  // Fade out
            yield return new WaitForSecondsRealtime(instructionsFadeTime);

            instructionsPanel.SetActive(false);
        }

        // ──────────────────────────────
        // 2. Countdown 3-2-1-GO!
        // ──────────────────────────────
        countdownCanvas.SetActive(true);

        // 3, 2, 1 → sans animation, juste affichage
        string[] numbers = { "3", "2", "1" };
        foreach (string num in numbers)
        {
            countdownText.text = num;
            countdownText.alpha = 1f;
            countdownText.transform.localScale = Vector3.one;

            yield return new WaitForSecondsRealtime(timeFor321);
        }

        // GO! → avec pop + fade out
        countdownText.text = "GO!";
        countdownText.alpha = 0f;
        countdownText.transform.localScale = Vector3.one * 0.7f;

        countdownText.DOFade(1f, 0.3f);
        countdownText.transform.DOScale(1.25f, 0.35f).SetEase(Ease.OutBack);

        yield return new WaitForSecondsRealtime(0.7f);

        countdownText.DOFade(0f, 0.3f);
        countdownText.transform.DOScale(0.8f, 0.3f).SetEase(Ease.InBack);

        yield return new WaitForSecondsRealtime(0.3f);

        // Fin
        countdownCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
}