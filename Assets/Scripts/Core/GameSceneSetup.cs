using UnityEngine;

public class GameSceneSetup : MonoBehaviour
{
    [Header("Core Managers")]
    public GameManager gameManager;
    public AudioManager audioManager;
    public PerformanceManager performanceManager;
    public DouyinPlatformBridge platformBridge;
    public TouchInputManager touchInput;

    [Header("Game Objects")]
    public BallController ball;
    public PlungerController plunger;
    public CameraController gameCamera;
    public PinballTableBuilder tableBuilder;
    public CyberpunkMaterialGenerator cyberpunkGenerator;

    [Header("UI")]
    public GameHUD gameHUD;
    public MainMenuUI mainMenu;
    public GameOverUI gameOverUI;
    public PauseMenuUI pauseMenu;

    [Header("Scene Setup")]
    public bool autoBuildTable = true;
    public bool startWithMenu = true;
    public bool enableCyberpunkStyle = true;

    private void Awake()
    {
        EnsureManagers();
    }

    private void Start()
    {
        if (enableCyberpunkStyle && cyberpunkGenerator != null)
        {
            cyberpunkGenerator.GenerateMaterials();
        }

        if (autoBuildTable && tableBuilder != null)
            tableBuilder.BuildTable();

        if (startWithMenu)
        {
            if (gameHUD != null) gameHUD.gameObject.SetActive(false);
            if (gameOverUI != null) gameOverUI.gameObject.SetActive(false);
            if (pauseMenu != null) pauseMenu.gameObject.SetActive(false);
            if (mainMenu != null) mainMenu.gameObject.SetActive(true);
        }
        else
        {
            GameManager.Instance?.StartNewGame();
        }
    }

    private void EnsureManagers()
    {
        if (gameManager == null)
        {
            GameObject go = new GameObject("GameManager");
            gameManager = go.AddComponent<GameManager>();
        }

        if (audioManager == null)
        {
            GameObject go = new GameObject("AudioManager");
            audioManager = go.AddComponent<AudioManager>();
        }

        if (performanceManager == null)
        {
            GameObject go = new GameObject("PerformanceManager");
            performanceManager = go.AddComponent<PerformanceManager>();
        }

        if (platformBridge == null)
        {
            GameObject go = new GameObject("PlatformBridge");
            platformBridge = go.AddComponent<DouyinPlatformBridge>();
        }

        if (touchInput == null)
        {
            GameObject go = new GameObject("TouchInputManager");
            touchInput = go.AddComponent<TouchInputManager>();
        }
    }
}
