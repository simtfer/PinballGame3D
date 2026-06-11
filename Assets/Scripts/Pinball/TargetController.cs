using UnityEngine;

public class TargetController : MonoBehaviour, IPinballElement
{
    [Header("Target Settings")]
    public int scoreValue = 250;
    public float resetDelay = 2f;
    public float hitForce = 5f;
    public bool isDropTarget = true;

    [Header("Visual")]
    public MeshRenderer targetRenderer;
    public Color activeColor = new Color(1f, 0.8f, 0f);
    public Color hitColor = new Color(0.2f, 1f, 0.2f);
    public float dropSpeed = 3f;

    [Header("Particles")]
    public ParticleSystem hitParticles;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip resetSound;

    private bool _isHit;
    private float _resetTimer;
    private float _dropPosition;
    private Vector3 _originalPosition;
    private AudioSource _audioSource;
    private MaterialPropertyBlock _propertyBlock;
    private Collider _collider;

    public bool IsHit => _isHit;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _propertyBlock = new MaterialPropertyBlock();
        _collider = GetComponent<Collider>();
        _originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (_isHit)
        {
            _resetTimer -= Time.deltaTime;
            if (_resetTimer <= 0)
                ResetTarget();

            if (isDropTarget)
            {
                _dropPosition = Mathf.MoveTowards(_dropPosition, -0.5f, dropSpeed * Time.deltaTime);
                Vector3 pos = transform.localPosition;
                pos.z = _originalPosition.z + _dropPosition;
                transform.localPosition = pos;
            }
        }

        UpdateVisuals();
    }

    public void OnBallHit(BallController ball, Collision collision)
    {
        if (_isHit) return;

        _isHit = true;
        _resetTimer = resetDelay;

        Vector3 pushDir = (ball.transform.position - transform.position).normalized;
        ball.ApplyForce(pushDir * hitForce, ForceMode.Impulse);

        GameManager.Instance?.AddScore(scoreValue);

        if (hitParticles != null)
            hitParticles.Play();

        PlaySound(hitSound);
    }

    private void ResetTarget()
    {
        _isHit = false;
        _dropPosition = 0f;
        transform.localPosition = _originalPosition;

        if (_collider != null)
            _collider.enabled = true;

        PlaySound(resetSound, 0.5f);
    }

    private void UpdateVisuals()
    {
        if (targetRenderer == null) return;

        Color currentColor = _isHit ? hitColor : activeColor;
        float emission = _isHit ? 0.5f : 1.5f;

        targetRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor("_BaseColor", currentColor);
        _propertyBlock.SetColor("_EmissionColor", currentColor * emission);
        targetRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (_audioSource != null && clip != null)
            _audioSource.PlayOneShot(clip, volume);
    }
}

public class TargetBankController : MonoBehaviour
{
    [Header("Bank Settings")]
    public TargetController[] targets;
    public int bonusScore = 1000;
    public float resetDelay = 3f;

    [Header("Visual")]
    public ParticleSystem completeEffect;

    [Header("Audio")]
    public AudioClip completeSound;

    private AudioSource _audioSource;
    private bool _completed;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (targets == null || targets.Length == 0)
            targets = GetComponentsInChildren<TargetController>();
    }

    private void Update()
    {
        if (!_completed)
            CheckAllTargetsHit();
    }

    private void CheckAllTargetsHit()
    {
        if (targets == null || targets.Length == 0) return;

        bool allHit = true;
        foreach (var target in targets)
        {
            if (!target.IsHit)
            {
                allHit = false;
                break;
            }
        }

        if (allHit)
        {
            _completed = true;
            OnBankComplete();
            Invoke(nameof(ResetCompleted), resetDelay);
        }
    }

    private void ResetCompleted()
    {
        _completed = false;
    }

    private void OnBankComplete()
    {
        GameManager.Instance?.AddScore(bonusScore);

        if (completeEffect != null)
            completeEffect.Play();

        if (_audioSource != null && completeSound != null)
            _audioSource.PlayOneShot(completeSound);
    }
}
