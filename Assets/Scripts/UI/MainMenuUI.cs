using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button leaderboardButton;
    public Button shareButton;

    [Header("Display")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI versionText;

    [Header("Animation")]
    public Animator menuAnimator;
    public CanvasGroup menuCanvas;

    [Header("Settings Panel")]
    public GameObject settingsPanel;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle vibrationToggle;
    public Toggle particleToggle;
    public Button settingsBackButton;

    [Header("Platform")]
    public Button douyinShareButton;

    private void Start()
    {
        SetupButtons();
        LoadSettings();
        UpdateHighScore();

        if (versionText != null)
            versionText.text = "v" + Application.version;
    }

    private void SetupButtons()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
        if (leaderboardButton != null)
            leaderboardButton.onClick.AddListener(OnLeaderboardClicked);
        if (shareButton != null)
            shareButton.onClick.AddListener(OnShareClicked);
        if (settingsBackButton != null)
            settingsBackButton.onClick.AddListener(OnSettingsBack);
        if (douyinShareButton != null)
            douyinShareButton.onClick.AddListener(OnDouyinShare);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        if (vibrationToggle != null)
            vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);
        if (particleToggle != null)
            particleToggle.onValueChanged.AddListener(OnParticleChanged);
    }

    private void OnPlayClicked()
    {
        if (menuAnimator != null)
            menuAnimator.SetTrigger("Exit");

        Invoke(nameof(StartGame), 0.5f);
    }

    private void StartGame()
    {
        GameManager.Instance?.StartNewGame();
    }

    private void OnSettingsClicked()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
        if (menuCanvas != null)
            menuCanvas.alpha = 0.5f;
    }

    private void OnSettingsBack()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        if (menuCanvas != null)
            menuCanvas.alpha = 1f;
        SaveSettings();
    }

    private void OnLeaderboardClicked()
    {
        DouyinPlatformBridge.Instance?.ShowLeaderboard();
    }

    private void OnShareClicked()
    {
        DouyinPlatformBridge.Instance?.ShareResult(
            GameManager.Instance != null ? GameManager.Instance.HighScore : 0
        );
    }

    private void OnDouyinShare()
    {
        DouyinPlatformBridge.Instance?.RecordAndShare();
    }

    private void LoadSettings()
    {
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        if (vibrationToggle != null)
            vibrationToggle.isOn = PlayerPrefs.GetInt("Vibration", 1) == 1;
        if (particleToggle != null)
            particleToggle.isOn = PlayerPrefs.GetInt("Particles", 1) == 1;
    }

    private void SaveSettings()
    {
        if (musicVolumeSlider != null)
            PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        if (sfxVolumeSlider != null)
            PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        if (vibrationToggle != null)
            PlayerPrefs.SetInt("Vibration", vibrationToggle.isOn ? 1 : 0);
        if (particleToggle != null)
            PlayerPrefs.SetInt("Particles", particleToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    private void OnSFXVolumeChanged(float value) { }
    private void OnVibrationChanged(bool value) { }
    private void OnParticleChanged(bool value) { }

    private void UpdateHighScore()
    {
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = highScore > 0
                ? "最高分: " + highScore.ToString("N0")
                : "尚无记录";
        }
    }
}
