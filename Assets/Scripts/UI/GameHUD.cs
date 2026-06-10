using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("Score Display")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI multiplierText;
    public Animator scoreAnimator;

    [Header("Lives Display")]
    public Image[] lifeIcons;
    public Sprite fullLifeSprite;
    public Sprite emptyLifeSprite;

    [Header("Combo Display")]
    public TextMeshProUGUI comboText;
    public CanvasGroup comboGroup;
    public Animator comboAnimator;

    [Header("Ball Status")]
    public Image ballStatusIcon;
    public TextMeshProUGUI ballStatusText;

    [Header("Power Meter")]
    public Slider powerMeter;
    public Image powerFill;
    public Gradient powerGradient;

    [Header("Notification")]
    public TextMeshProUGUI notificationText;
    public CanvasGroup notificationGroup;
    public Animator notificationAnimator;

    [Header("Pause Button")]
    public Button pauseButton;

    private void Start()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnScoreChanged += UpdateScore;
        GameManager.Instance.OnLivesChanged += UpdateLives;
        GameManager.Instance.OnComboChanged += UpdateCombo;
        GameManager.Instance.OnStateChanged += OnGameStateChanged;

        UpdateScore(GameManager.Instance.Score);
        UpdateLives(GameManager.Instance.Lives);
        UpdateCombo(GameManager.Instance.Combo, GameManager.Instance.ComboMultiplier);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseClicked);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScore;
            GameManager.Instance.OnLivesChanged -= UpdateLives;
            GameManager.Instance.OnComboChanged -= UpdateCombo;
            GameManager.Instance.OnStateChanged -= OnGameStateChanged;
        }
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = FormatScore(score);

        if (highScoreText != null)
            highScoreText.text = "BEST: " + FormatScore(GameManager.Instance.HighScore);

        if (scoreAnimator != null)
            scoreAnimator.SetTrigger("ScoreUpdate");
    }

    private void UpdateLives(int lives)
    {
        if (lifeIcons == null) return;

        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (i < lives)
            {
                lifeIcons[i].sprite = fullLifeSprite;
                lifeIcons[i].color = Color.white;
            }
            else
            {
                lifeIcons[i].sprite = emptyLifeSprite;
                lifeIcons[i].color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }
    }

    private void UpdateCombo(int combo, float multiplier)
    {
        if (comboGroup == null) return;

        if (combo > 1)
        {
            comboGroup.alpha = 1f;
            if (comboText != null)
                comboText.text = $"x{combo} COMBO!";
            if (multiplierText != null)
                multiplierText.text = $"x{multiplier:F1}";

            if (comboAnimator != null)
                comboAnimator.SetTrigger("ComboUpdate");
        }
        else
        {
            comboGroup.alpha = 0f;
        }
    }

    private void OnGameStateChanged(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.Playing:
                SetHUDVisible(true);
                break;
            case GameManager.GameState.BallLost:
                ShowNotification("BALL LOST");
                break;
            case GameManager.GameState.GameOver:
                SetHUDVisible(false);
                break;
            case GameManager.GameState.Paused:
                break;
        }
    }

    public void UpdatePowerMeter(float power)
    {
        if (powerMeter != null)
            powerMeter.value = power;

        if (powerFill != null && powerGradient != null)
            powerFill.color = powerGradient.Evaluate(power);
    }

    public void ShowNotification(string text, float duration = 2f)
    {
        if (notificationText == null || notificationGroup == null) return;

        notificationText.text = text;
        notificationGroup.alpha = 1f;

        if (notificationAnimator != null)
            notificationAnimator.SetTrigger("Show");

        Invoke(nameof(HideNotification), duration);
    }

    private void HideNotification()
    {
        if (notificationGroup != null)
            notificationGroup.alpha = 0f;
    }

    private void SetHUDVisible(bool visible)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.alpha = visible ? 1f : 0f;
    }

    private void OnPauseClicked()
    {
        GameManager.Instance?.PauseGame();
    }

    private string FormatScore(int score)
    {
        if (score >= 1000000)
            return (score / 1000000f).ToString("F1") + "M";
        if (score >= 1000)
            return (score / 1000f).ToString("F1") + "K";
        return score.ToString();
    }
}
