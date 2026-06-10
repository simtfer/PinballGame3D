using UnityEngine;

public class RampController : MonoBehaviour, IPinballTrigger
{
    [Header("Ramp Settings")]
    public int scoreValue = 500;
    public float speedBoost = 2f;
    public bool isLoopRamp = false;

    [Header("Visual")]
    public MeshRenderer rampRenderer;
    public Color baseColor = new Color(0.1f, 0.8f, 0.4f);
    public Color activeColor = new Color(0.3f, 1f, 0.6f);

    [Header("Particles")]
    public ParticleSystem trailEffect;

    [Header("Audio")]
    public AudioClip rampSound;

    private bool _isActive;
    private float _activeTimer;
    private AudioSource _audioSource;
    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (_activeTimer > 0)
        {
            _activeTimer -= Time.deltaTime;
            if (_activeTimer <= 0) _isActive = false;
        }

        UpdateVisuals();
    }

    public void OnBallEnter(BallController ball)
    {
        _isActive = true;
        _activeTimer = 1f;

        Vector3 rampForward = transform.forward;
        ball.ApplyForce(rampForward * speedBoost, ForceMode.VelocityChange);

        GameManager.Instance?.AddScore(scoreValue);

        if (trailEffect != null)
            trailEffect.Play();

        if (_audioSource != null && rampSound != null)
            _audioSource.PlayOneShot(rampSound, 0.7f);
    }

    private void UpdateVisuals()
    {
        if (rampRenderer == null) return;

        Color currentColor = _isActive ? activeColor : baseColor;
        float emission = _isActive ? 1.5f : 0.2f;

        rampRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor("_BaseColor", currentColor);
        _propertyBlock.SetColor("_EmissionColor", currentColor * emission);
        rampRenderer.SetPropertyBlock(_propertyBlock);
    }
}
