using UnityEngine;

public class PerformanceManager : MonoBehaviour
{
    public static PerformanceManager Instance { get; private set; }

    [Header("Quality Settings")]
    public int targetFrameRate = 60;
    public bool enableVSync = false;
    public int vSyncCount = 0;

    [Header("Dynamic Quality")]
    public bool enableDynamicQuality = true;
    public float lowFPSThreshold = 25f;
    public float highFPSThreshold = 55f;
    public float adjustInterval = 2f;

    [Header("Memory")]
    public int maxTextureSize = 1024;
    public bool compressAudio = true;

    private float _fpsTimer;
    private int _frameCount;
    private float _currentFPS;
    private int _currentQualityLevel;

    public float CurrentFPS => _currentFPS;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ApplyInitialSettings();
    }

    private void Update()
    {
        _frameCount++;
        _fpsTimer += Time.unscaledDeltaTime;

        if (_fpsTimer >= 1f)
        {
            _currentFPS = _frameCount / _fpsTimer;
            _frameCount = 0;
            _fpsTimer = 0f;
        }

        if (enableDynamicQuality)
        {
            if (Time.frameCount % (int)(adjustInterval * targetFrameRate) == 0)
                AdjustQuality();
        }
    }

    private void ApplyInitialSettings()
    {
        Application.targetFrameRate = targetFrameRate;
        QualitySettings.vSyncCount = enableVSync ? vSyncCount : 0;

        DetectDeviceAndSetQuality();
    }

    private void DetectDeviceAndSetQuality()
    {
        int ram = SystemInfo.systemMemorySize;
        int vram = SystemInfo.graphicsMemorySize;
        int processorCount = SystemInfo.processorCount;

        if (ram >= 4096 && vram >= 2048 && processorCount >= 6)
        {
            SetQualityLevel(2);
        }
        else if (ram >= 2048 && vram >= 1024 && processorCount >= 4)
        {
            SetQualityLevel(1);
        }
        else
        {
            SetQualityLevel(0);
        }
    }

    private void AdjustQuality()
    {
        if (_currentFPS < lowFPSThreshold && _currentQualityLevel > 0)
        {
            SetQualityLevel(_currentQualityLevel - 1);
        }
        else if (_currentFPS > highFPSThreshold && _currentQualityLevel < 2)
        {
            SetQualityLevel(_currentQualityLevel + 1);
        }
    }

    private void SetQualityLevel(int level)
    {
        _currentQualityLevel = level;

        switch (level)
        {
            case 0:
                QualitySettings.shadows = ShadowQuality.HardOnly;
                QualitySettings.shadowResolution = ShadowResolution.Low;
                QualitySettings.antiAliasing = 0;
                QualitySettings.particleRaycastBudget = 16;
                Shader.globalMaximumLOD = 150;
                break;
            case 1:
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.shadowResolution = ShadowResolution.Medium;
                QualitySettings.antiAliasing = 2;
                QualitySettings.particleRaycastBudget = 64;
                Shader.globalMaximumLOD = 250;
                break;
            case 2:
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.shadowResolution = ShadowResolution.High;
                QualitySettings.antiAliasing = 4;
                QualitySettings.particleRaycastBudget = 256;
                Shader.globalMaximumLOD = 400;
                break;
        }

        Debug.Log($"[PerformanceManager] Quality level set to {level}");
    }
}
