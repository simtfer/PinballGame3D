using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [Header("Ball Settings")]
    public float radius = 0.15f;
    public float maxSpeed = 25f;
    public float minSpeed = 0.5f;
    public float drag = 0.1f;
    public float angularDrag = 0.5f;
    public float ballLostThreshold = -5f;
    public PhysicMaterial physicMaterial;

    [Header("Visual Settings")]
    public MeshRenderer ballRenderer;
    public TrailRenderer trailRenderer;
    public ParticleSystem sparkParticles;
    public ParticleSystem glowParticles;
    public Color normalColor = Color.cyan;
    public Color boostColor = Color.yellow;
    public Color dangerColor = Color.red;

    [Header("Audio")]
    public AudioClip bounceSound;
    public AudioClip launchSound;
    public AudioClip loseSound;

    private Rigidbody _rb;
    private Vector3 _startPosition;
    private bool _isLaunched;
    private float _lastBounceTime;
    private MaterialPropertyBlock _propertyBlock;
    private int _emissionColorId;
    private AudioSource _audioSource;

    public bool IsLaunched => _isLaunched;
    public Vector3 Velocity => _rb != null ? _rb.velocity : Vector3.zero;
    public float Speed => _rb != null ? _rb.velocity.magnitude : 0f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _propertyBlock = new MaterialPropertyBlock();
        _emissionColorId = Shader.PropertyToID("_EmissionColor");

        SetupRigidbody();
        _startPosition = transform.position;
    }

    private void SetupRigidbody()
    {
        _rb.useGravity = true;
        _rb.drag = drag;
        _rb.angularDrag = angularDrag;
        _rb.mass = 1f;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.constraints = RigidbodyConstraints.None;

        SphereCollider col = GetComponent<SphereCollider>();
        if (col == null)
            col = gameObject.AddComponent<SphereCollider>();
        col.radius = radius;
        if (physicMaterial != null)
            col.material = physicMaterial;
    }

    private void FixedUpdate()
    {
        if (!_isLaunched) return;

        if (Speed > maxSpeed)
        {
            _rb.velocity = _rb.velocity.normalized * maxSpeed;
        }

        if (transform.position.y < ballLostThreshold)
        {
            OnBallFallen();
        }
    }

    private void Update()
    {
        UpdateVisuals();
    }

    public void LaunchBall(Vector3 direction, float force)
    {
        if (_isLaunched) return;

        _isLaunched = true;
        _rb.isKinematic = false;
        _rb.AddForce(direction * force, ForceMode.Impulse);

        if (trailRenderer != null) trailRenderer.enabled = true;
        PlaySound(launchSound);
    }

    public void ResetBall()
    {
        _isLaunched = false;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;
        transform.position = _startPosition;
        transform.rotation = Quaternion.identity;

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
            trailRenderer.Clear();
        }
    }

    public void ApplyForce(Vector3 force, ForceMode mode = ForceMode.Impulse)
    {
        if (!_isLaunched) return;
        _rb.AddForce(force, mode);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - _lastBounceTime < 0.05f) return;
        _lastBounceTime = Time.time;

        float impactVelocity = collision.relativeVelocity.magnitude;
        if (impactVelocity > 1f)
        {
            OnBounce(collision.contacts[0].point, collision.contacts[0].normal, impactVelocity);
        }

        IPinballElement element = collision.gameObject.GetComponent<IPinballElement>();
        if (element != null)
        {
            element.OnBallHit(this, collision);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IPinballTrigger trigger = other.GetComponent<IPinballTrigger>();
        if (trigger != null)
        {
            trigger.OnBallEnter(this);
        }
    }

    private void OnBounce(Vector3 point, Vector3 normal, float velocity)
    {
        float intensity = Mathf.Clamp01(velocity / maxSpeed);

        if (sparkParticles != null && intensity > 0.2f)
        {
            sparkParticles.transform.position = point;
            var emission = sparkParticles.emission;
            emission.rateOverTime = intensity * 50;
            if (!sparkParticles.isPlaying) sparkParticles.Play();
        }

        PlaySound(bounceSound, 0.5f + intensity * 0.5f);
    }

    private void OnBallFallen()
    {
        PlaySound(loseSound);
        GameManager.Instance?.OnBallLost();
    }

    private void UpdateVisuals()
    {
        if (ballRenderer == null) return;

        float speedRatio = Speed / maxSpeed;
        Color currentColor;

        if (speedRatio > 0.7f)
            currentColor = Color.Lerp(normalColor, dangerColor, (speedRatio - 0.7f) / 0.3f);
        else if (speedRatio > 0.3f)
            currentColor = Color.Lerp(boostColor, normalColor, (speedRatio - 0.3f) / 0.4f);
        else
            currentColor = boostColor;

        ballRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(_emissionColorId, currentColor * (1f + speedRatio * 3f));
        _propertyBlock.SetColor("_BaseColor", currentColor);
        ballRenderer.SetPropertyBlock(_propertyBlock);

        if (trailRenderer != null)
        {
            var gradient = trailRenderer.colorGradient;
            if (gradient != null && gradient.colorKeys.Length > 0)
            {
                gradient.colorKeys[0] = new GradientColorKey(currentColor, 0f);
                trailRenderer.colorGradient = gradient;
            }
        }
    }

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (_audioSource != null && clip != null)
        {
            _audioSource.PlayOneShot(clip, volume);
        }
    }
}

public interface IPinballElement
{
    void OnBallHit(BallController ball, Collision collision);
}

public interface IPinballTrigger
{
    void OnBallEnter(BallController ball);
}
