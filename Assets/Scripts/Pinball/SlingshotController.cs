using UnityEngine;

public class SlingshotController : MonoBehaviour, IPinballElement
{
    [Header("Slingshot Settings")]
    public float bounceForce = 15f;
    public int scoreValue = 50;

    [Header("Visual")]
    public MeshRenderer slingshotRenderer;
    public Color idleColor = new Color(0.5f, 0.2f, 0.8f);
    public Color hitColor = new Color(1f, 0.5f, 1f);
    public float hitFlashDuration = 0.15f;

    [Header("Particles")]
    public ParticleSystem hitParticles;

    [Header("Audio")]
    public AudioClip hitSound;

    private float _hitTimer;
    private AudioSource _audioSource;
    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        SetupCollider();
    }

    private void SetupCollider()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            PhysicMaterial mat = new PhysicMaterial("SlingshotMaterial");
            mat.bounciness = 0.9f;
            mat.bounceCombine = PhysicMaterialCombine.Maximum;
            mat.frictionCombine = PhysicMaterialCombine.Minimum;
            col.sharedMaterial = mat;
        }
    }

    private void Update()
    {
        if (_hitTimer > 0)
            _hitTimer -= Time.deltaTime;

        UpdateVisuals();
    }

    public void OnBallHit(BallController ball, Collision collision)
    {
        Vector3 pushDir = (ball.transform.position - transform.position).normalized;
        pushDir.y = Mathf.Max(pushDir.y, 0.3f);
        ball.ApplyForce(pushDir.normalized * bounceForce, ForceMode.Impulse);

        _hitTimer = hitFlashDuration;
        GameManager.Instance?.AddScore(scoreValue);

        if (hitParticles != null)
            hitParticles.Play();

        PlaySound(hitSound);
    }

    private void UpdateVisuals()
    {
        if (slingshotRenderer == null) return;

        float t = Mathf.Clamp01(_hitTimer / hitFlashDuration);
        Color currentColor = Color.Lerp(idleColor, hitColor, t);

        slingshotRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor("_BaseColor", currentColor);
        _propertyBlock.SetColor("_EmissionColor", currentColor * (0.3f + t * 2f));
        slingshotRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (_audioSource != null && clip != null)
            _audioSource.PlayOneShot(clip, volume);
    }
}
