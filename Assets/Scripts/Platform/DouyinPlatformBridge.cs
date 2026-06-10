using UnityEngine;
using System;

public class DouyinPlatformBridge : MonoBehaviour
{
    public static DouyinPlatformBridge Instance { get; private set; }

    [Header("Ad Settings")]
    public string bannerAdId = "";
    public string interstitialAdId = "";
    public string rewardedAdId = "";

    [Header("Social Settings")]
    public string shareTitle = "3D弹球大师";
    public string shareDescription = "来挑战我的弹球分数吧！";

    private Action<bool> _rewardedAdCallback;
    private bool _isAdShowing;

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
        InitializePlatform();
    }

    private void InitializePlatform()
    {
#if UNITY_EDITOR
        Debug.Log("[DouyinPlatform] Editor mode - platform features simulated");
#elif UNITY_ANDROID || UNITY_IOS
        Debug.Log("[DouyinPlatform] Native platform initialized");
#endif
    }

    public void ShowBannerAd()
    {
        if (_isAdShowing) return;

#if UNITY_EDITOR
        Debug.Log("[DouyinPlatform] Banner ad shown (simulated)");
#else
        // Native banner ad implementation
#endif
    }

    public void HideBannerAd()
    {
#if UNITY_EDITOR
        Debug.Log("[DouyinPlatform] Banner ad hidden (simulated)");
#else
        // Native hide banner
#endif
    }

    public void ShowInterstitialAd()
    {
        if (_isAdShowing) return;
        _isAdShowing = true;

#if UNITY_EDITOR
        Debug.Log("[DouyinPlatform] Interstitial ad shown (simulated)");
        _isAdShowing = false;
#else
        // Native interstitial implementation
#endif
    }

    public void ShowRewardedAd(Action<bool> callback)
    {
        if (_isAdShowing) return;
        _isAdShowing = true;
        _rewardedAdCallback = callback;

#if UNITY_EDITOR
        Debug.Log("[DouyinPlatform] Rewarded ad shown (simulated - auto reward)");
        _isAdShowing = false;
        _rewardedAdCallback?.Invoke(true);
        _rewardedAdCallback = null;
#else
        // Native rewarded ad implementation
#endif
    }

    public void ShowLeaderboard()
    {
#if UNITY_EDITOR
        Debug.Log("[DouyinPlatform] Leaderboard opened (simulated)");
#else
        // Native leaderboard implementation
#endif
    }

    public void SubmitScore(int score)
    {
#if UNITY_EDITOR
        Debug.Log($"[DouyinPlatform] Score submitted: {score}");
#else
        // Native score submission
#endif
    }

    public void ShareResult(int score)
    {
        string title = $"{shareTitle} - {score}分!";

#if UNITY_EDITOR
        Debug.Log($"[DouyinPlatform] Share: {title} - {shareDescription}");
#else
        // Native share implementation using Douyin SDK
#endif
    }

    public void RecordAndShare()
    {
#if UNITY_EDITOR
        Debug.Log("[DouyinPlatform] Record and share (simulated)");
#else
        // Native screen recording and share
#endif
    }

    public void Vibrate(int milliseconds = 50)
    {
        if (PlayerPrefs.GetInt("Vibration", 1) == 0) return;

#if UNITY_EDITOR
        Debug.Log($"[DouyinPlatform] Vibrate: {milliseconds}ms");
#elif UNITY_ANDROID
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        vibrator.Call("vibrate", (long)milliseconds);
#elif UNITY_IOS
        Handheld.Vibrate();
#endif
    }

    public void OnRewardedAdComplete(bool rewarded)
    {
        _isAdShowing = false;
        _rewardedAdCallback?.Invoke(rewarded);
        _rewardedAdCallback = null;
    }
}
