using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Score Display")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI newHighScoreText;
    public TextMeshProUGUI statsText;

    [Header("Buttons")]
    public Button retryButton;
    public Button mainMenuButton;
    public Button shareButton;
    public Button watchAdButton;

    [Header("Animation")]
    public Animator panelAnimator;
    public CanvasGroup canvasGroup;

    [Header("Effects")]
    public ParticleSystem confettiEffect;
    public GameObject newHighScoreBanner;

    private int _previousHighScore;

    private void Start()
    {
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        if (shareButton != null)
            shareButton.onClick.AddListener(OnShareClicked);
        if (watchAdButton != null)
            watchAdButton.onClick.AddListener(OnWatchAdClicked);

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver += ShowGameOver;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver -= ShowGameOver;
    }

    private void ShowGameOver()
    {
        gameObject.SetActive(true);

        _previousHighScore = PlayerPrefs.GetInt("HighScore", 0);
        int currentScore = GameManager.Instance != null ? GameManager.Instance.Score : 0;
        bool isNewHighScore = currentScore > _previousHighScore;

        if (finalScoreText != null)
            finalScoreText.text = currentScore.ToString("N0");

        if (highScoreText != null)
            highScoreText.text = "最高分: " + Mathf.Max(currentScore, _previousHighScore).ToString("N0");

        if (newHighScoreText != null)
            newHighScoreText.gameObject.SetActive(isNewHighScore);

        if (newHighScoreBanner != null)
            newHighScoreBanner.SetActive(isNewHighScore);

        if (confettiEffect != null && isNewHighScore)
            confettiEffect.Play();

        if (statsText != null)
            statsText.text = $"连击最高: x{GameManager.Instance.ComboMultiplier:F1}";

        if (panelAnimator != null)
            panelAnimator.SetTrigger("Show");
    }

    private void OnRetryClicked()
    {
        if (panelAnimator != null)
            panelAnimator.SetTrigger("Hide");
        Invoke(nameof(RestartGameAfterHide), 0.3f);
    }

    private void RestartGameAfterHide()
    {
        GameManager.Instance?.RestartGame();
    }

    private void OnMainMenuClicked()
    {
        gameObject.SetActive(false);
        GameManager.Instance?.ReturnToMainMenu();
    }

    private void OnShareClicked()
    {
        int score = GameManager.Instance != null ? GameManager.Instance.Score : 0;
        DouyinPlatformBridge.Instance?.ShareResult(score);
    }

    private void OnWatchAdClicked()
    {
        DouyinPlatformBridge.Instance?.ShowRewardedAd((success) =>
        {
            if (success)
            {
                gameObject.SetActive(false);
                GameManager.Instance?.RestartGame();
            }
        });
    }
}
