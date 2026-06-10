using UnityEngine;

public class KickbackController : MonoBehaviour, IPinballTrigger
{
    [Header("Kickback Settings")]
    public float kickForce = 20f;
    public Vector3 kickDirection = Vector3.forward;
    public float cooldown = 2f;
    public int scoreValue = 25;

    [Header("Visual")]
    public MeshRenderer kickbackRenderer;
    public Color readyColor = new Color(0.2f, 0.8f, 0.2f);
    public Color cooldownColor = new Color(0.5f, 0.2f, 0.2f);

    [Header("Particles")]
    public ParticleSystem kickEffect;

    [Header("Audio")]
    public AudioClip kickSound;

    private float _cooldownTimer;
    private bool _isReady = true;
    private AudioSource _audioSource;
    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (!_isReady)
        {
            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0)
                _isReady = true;
        }

        UpdateVisuals();
    }

    public void OnBallEnter(BallController ball)
    {
        if (!_isReady) return;

        _isReady = false;
        _cooldownTimer = cooldown;

        ball.ApplyForce(kickDirection.normalized * kickForce, ForceMode.Impulse);
        GameManager.Instance?.AddScore(scoreValue);

        if (kickEffect != null) kickEffect.Play();
        if (_audioSource != null && kickSound != null)
            _audioSource.PlayOneShot(kickSound);
    }

    private void UpdateVisuals()
    {
        if (kickbackRenderer == null) return;

        Color currentColor = _isReady ? readyColor : cooldownColor;
        kickbackRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor("_BaseColor", currentColor);
        _propertyBlock.SetColor("_EmissionColor", currentColor * (_isReady ? 1f : 0.2f));
        kickbackRenderer.SetPropertyBlock(_propertyBlock);
    }
}
