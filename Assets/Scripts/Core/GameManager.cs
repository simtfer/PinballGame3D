using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int maxLives = 3;
    public float ballLaunchForce = 15f;
    public float slowMotionScale = 0.3f;
    public float comboTimeout = 2f;

    [Header("Score Settings")]
    public int bumperScore = 100;
    public int targetScore = 250;
    public int slingshotScore = 50;
    public int rampScore = 500;
    public int spinnerScore = 75;
    public float comboMultiplierStep = 0.5f;
    public float maxComboMultiplier = 5f;

    [Header("References")]
    public BallController ball;
    public PlungerController plunger;

    private int _score;
    private int _highScore;
    private int _lives;
    private int _combo;
    private float _comboTimer;
    private float _comboMultiplier = 1f;
    private bool _isGameActive;
    private GameState _currentState = GameState.MainMenu;

    public enum GameState
    {
        MainMenu,
        Playing,
        BallLost,
        GameOver,
        Paused
    }

    public int Score => _score;
    public int HighScore => _highScore;
    public int Lives => _lives;
    public int Combo => _combo;
    public float ComboMultiplier => _comboMultiplier;
    public bool IsGameActive => _isGameActive;
    public GameState CurrentState => _currentState;

    public event System.Action<int> OnScoreChanged;
    public event System.Action<int> OnLivesChanged;
    public event System.Action<int, float> OnComboChanged;
    public event System.Action<GameState> OnStateChanged;
    public event System.Action OnGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void Update()
    {
        if (_comboTimer > 0)
        {
            _comboTimer -= Time.deltaTime;
            if (_comboTimer <= 0)
            {
                _combo = 0;
                _comboMultiplier = 1f;
                OnComboChanged?.Invoke(_combo, _comboMultiplier);
            }
        }
    }

    public void StartNewGame()
    {
        _score = 0;
        _lives = maxLives;
        _combo = 0;
        _comboMultiplier = 1f;
        _isGameActive = true;
        SetState(GameState.Playing);

        OnScoreChanged?.Invoke(_score);
        OnLivesChanged?.Invoke(_lives);
        OnComboChanged?.Invoke(_combo, _comboMultiplier);

        ResetBall();
    }

    public void AddScore(int baseScore)
    {
        if (!_isGameActive) return;

        _combo++;
        _comboTimer = comboTimeout;
        _comboMultiplier = Mathf.Min(1f + (_combo - 1) * comboMultiplierStep, maxComboMultiplier);

        int finalScore = Mathf.RoundToInt(baseScore * _comboMultiplier);
        _score += finalScore;
        OnScoreChanged?.Invoke(_score);
        OnComboChanged?.Invoke(_combo, _comboMultiplier);

        if (_score > _highScore)
        {
            _highScore = _score;
            PlayerPrefs.SetInt("HighScore", _highScore);
            PlayerPrefs.Save();
        }
    }

    public void OnBallLost()
    {
        _lives--;
        OnLivesChanged?.Invoke(_lives);
        _combo = 0;
        _comboMultiplier = 1f;
        OnComboChanged?.Invoke(_combo, _comboMultiplier);

        if (_lives <= 0)
        {
            _isGameActive = false;
            SetState(GameState.GameOver);
            OnGameOver?.Invoke();
        }
        else
        {
            SetState(GameState.BallLost);
            Invoke(nameof(ResetBall), 1.5f);
        }
    }

    public void PauseGame()
    {
        if (_currentState == GameState.Playing)
        {
            SetState(GameState.Paused);
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        if (_currentState == GameState.Paused)
        {
            Time.timeScale = 1f;
            SetState(GameState.Playing);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        StartNewGame();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        _isGameActive = false;
        SetState(GameState.MainMenu);
        SceneManager.LoadScene("MainMenu");
    }

    private void ResetBall()
    {
        if (ball != null)
            ball.ResetBall();
        if (plunger != null)
            plunger.ResetPlunger();
        SetState(GameState.Playing);
    }

    private void SetState(GameState newState)
    {
        _currentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}
