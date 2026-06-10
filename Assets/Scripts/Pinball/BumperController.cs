using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BumperController : MonoBehaviour, IPinballElement
{
    [Header("Bumper Settings")]
    public float bounceForce = 12f;
    public float activationThreshold = 1f;
    public int scoreValue = 100;

    [Header("Visual")]
    public MeshRenderer bumperRenderer;
    public MeshFilter meshFilter;
    public Color idleColor = new Color(0.2f, 0.6f, 1f);
    public Color hitColor = Color.white;
    public float hitFlashDuration = 0.2f;
    public float scaleBounceAmount = 0.3f;
    public float scaleReturnSpeed = 5f;

    [Header("Particles")]
    public ParticleSystem hitParticles;
    public ParticleSystem ringParticles;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip strongHitSound;

    private float _hitTimer;
    private float _currentScale = 1f;
    private AudioSource _audioSource;
    private MaterialPropertyBlock _propertyBlock;
    private int _emissionColorId;
    private Vector3 _originalScale;

    public float HitIntensity => Mathf.Clamp01(_hitTimer / hitFlashDuration);

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _propertyBlock = new MaterialPropertyBlock();
        _emissionColorId = Shader.PropertyToID("_EmissionColor");
        _originalScale = transform.localScale;
    }

    private void Start()
    {
        SetupCollider();
    }

    private void SetupCollider()
    {
        Collider col = GetComponent<Collider>();
        if (col is MeshCollider meshCol)
        {
            meshCol.convex = true;
        }

        PhysicMaterial mat = col.sharedMaterial;
        if (mat == null)
        {
            mat = new PhysicMaterial("BumperMaterial");
            mat.bounciness = 0.8f;
            mat.bounceCombine = PhysicMaterialCombine.Maximum;
            mat.frictionCombine = PhysicMaterialCombine.Minimum;
            col.sharedMaterial = mat;
        }
    }

    private void Update()
    {
        UpdateHitAnimation();
        UpdateVisuals();
    }

    public void OnBallHit(BallController ball, Collision collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude;
        if (impactVelocity < activationThreshold) return;

        float intensity = Mathf.Clamp01(impactVelocity / 15f);

        Vector3 pushDir = (ball.transform.position - transform.position).normalized;
        pushDir.y = Mathf.Abs(pushDir.y) * 0.5f + 0.5f;
        ball.ApplyForce(pushDir * bounceForce * (0.5f + intensity * 0.5f), ForceMode.Impulse);

        _hitTimer = hitFlashDuration;
        _currentScale = 1f + scaleBounceAmount * intensity;

        SpawnParticles(collision.contacts[0].point, intensity);
        GameManager.Instance?.AddScore(scoreValue);

        AudioClip clip = intensity > 0.6f ? strongHitSound : hitSound;
        PlaySound(clip, 0.5f + intensity * 0.5f);
    }

    private void UpdateHitAnimation()
    {
        if (_hitTimer > 0)
        {
            _hitTimer -= Time.deltaTime;
        }

        _currentScale = Mathf.Lerp(_currentScale, 1f, scaleReturnSpeed * Time.deltaTime);
        transform.localScale = _originalScale * _currentScale;
    }

    private void UpdateVisuals()
    {
        if (bumperRenderer == null) return;

        float t = HitIntensity;
        Color currentColor = Color.Lerp(idleColor, hitColor, t);
        float emissionIntensity = 0.3f + t * 3f;

        bumperRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor("_BaseColor", currentColor);
        _propertyBlock.SetColor(_emissionColorId, currentColor * emissionIntensity);
        bumperRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void SpawnParticles(Vector3 point, float intensity)
    {
        if (hitParticles != null)
        {
            hitParticles.transform.position = point;
            var emission = hitParticles.emission;
            emission.burstCount = Mathf.CeilToInt(intensity * 20);
            hitParticles.Play();
        }

        if (ringParticles != null && intensity > 0.5f)
        {
            ringParticles.transform.position = point;
            ringParticles.Play();
        }
    }

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (_audioSource != null && clip != null)
            _audioSource.PlayOneShot(clip, volume);
    }
}
