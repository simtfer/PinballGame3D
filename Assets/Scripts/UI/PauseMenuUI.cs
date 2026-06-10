using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    public Button settingsButton;

    [Header("Display")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    [Header("Panel")]
    public CanvasGroup canvasGroup;
    public Animator panelAnimator;

    private void Start()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnGameStateChanged;

        HideMenu();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.Paused)
            ShowMenu();
        else
            HideMenu();
    }

    private void ShowMenu()
    {
        gameObject.SetActive(true);
        if (canvasGroup != null) canvasGroup.alpha = 1f;
        if (panelAnimator != null) panelAnimator.SetTrigger("Show");

        if (scoreText != null && GameManager.Instance != null)
            scoreText.text = "当前分数: " + GameManager.Instance.Score.ToString("N0");
        if (highScoreText != null && GameManager.Instance != null)
            highScoreText.text = "最高分: " + GameManager.Instance.HighScore.ToString("N0");
    }

    private void HideMenu()
    {
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    private void OnResumeClicked()
    {
        GameManager.Instance?.ResumeGame();
    }

    private void OnRestartClicked()
    {
        GameManager.Instance?.RestartGame();
    }

    private void OnMainMenuClicked()
    {
        GameManager.Instance?.ReturnToMainMenu();
    }
}
