using UnityEngine;

public class ScoreRolloverController : MonoBehaviour, IPinballTrigger
{
    [Header("Rollover Settings")]
    public int scoreValue = 100;
    public float cooldown = 0.5f;

    [Header("Visual")]
    public MeshRenderer rolloverRenderer;
    public Transform lightIndicator;
    public Color inactiveColor = new Color(0.3f, 0.3f, 0.3f);
    public Color activeColor = new Color(1f, 1f, 0f);

    [Header("Audio")]
    public AudioClip rolloverSound;

    private bool _isLit;
    private float _cooldownTimer;
    private AudioSource _audioSource;
    private MaterialPropertyBlock _propertyBlock;

    public bool IsLit => _isLit;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (_cooldownTimer > 0)
            _cooldownTimer -= Time.deltaTime;

        UpdateVisuals();
    }

    public void OnBallEnter(BallController ball)
    {
        if (_cooldownTimer > 0) return;
        _cooldownTimer = cooldown;

        _isLit = true;
        GameManager.Instance?.AddScore(scoreValue);

        if (lightIndicator != null)
            lightIndicator.gameObject.SetActive(true);

        if (_audioSource != null && rolloverSound != null)
            _audioSource.PlayOneShot(rolloverSound, 0.7f);
    }

    public void ResetRollover()
    {
        _isLit = false;
        if (lightIndicator != null)
            lightIndicator.gameObject.SetActive(false);
    }

    private void UpdateVisuals()
    {
        if (rolloverRenderer == null) return;

        Color currentColor = _isLit ? activeColor : inactiveColor;
        rolloverRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor("_BaseColor", currentColor);
        _propertyBlock.SetColor("_EmissionColor", currentColor * (_isLit ? 2f : 0.1f));
        rolloverRenderer.SetPropertyBlock(_propertyBlock);
    }
}

public class RolloverBankController : MonoBehaviour
{
    [Header("Bank Settings")]
    public ScoreRolloverController[] rollovers;
    public int bonusScore = 2000;
    public bool autoReset = true;
    public float resetDelay = 1f;

    [Header("Audio")]
    public AudioClip completeSound;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (rollovers == null || rollovers.Length == 0)
            rollovers = GetComponentsInChildren<ScoreRolloverController>();
    }

    private void Update()
    {
        CheckAllLit();
    }

    private void CheckAllLit()
    {
        if (rollovers == null || rollovers.Length == 0) return;

        bool allLit = true;
        foreach (var rollover in rollovers)
        {
            if (!rollover.IsLit)
            {
                allLit = false;
                break;
            }
        }

        if (allLit)
        {
            OnAllLit();
        }
    }

    private void OnAllLit()
    {
        GameManager.Instance?.AddScore(bonusScore);

        if (_audioSource != null && completeSound != null)
            _audioSource.PlayOneShot(completeSound);

        if (autoReset)
        {
            Invoke(nameof(ResetAll), resetDelay);
        }
    }

    public void ResetAll()
    {
        if (rollovers == null) return;
        foreach (var rollover in rollovers)
            rollover.ResetRollover();
    }
}
