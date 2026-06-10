using UnityEngine;

public class FlipperController : MonoBehaviour, IPinballElement
{
    [Header("Flipper Settings")]
    public bool isLeftFlipper = true;
    public float restAngle = 25f;
    public float activeAngle = -25f;
    public float flipSpeed = 30f;
    public float hitForce = 20f;
    public float cooldown = 0.15f;

    [Header("Input")]
    public KeyCode leftFlipperKey = KeyCode.LeftShift;
    public KeyCode rightFlipperKey = KeyCode.RightShift;
    public KeyCode touchLeftKey = KeyCode.A;
    public KeyCode touchRightKey = KeyCode.D;

    [Header("Hinge")]
    public HingeJoint hingeJoint;

    [Header("Visual")]
    public MeshRenderer flipperRenderer;
    public Color restColor = new Color(0.8f, 0.2f, 0.2f);
    public Color activeColor = new Color(1f, 0.5f, 0.1f);

    [Header("Audio")]
    public AudioClip flipSound;
    public AudioClip hitSound;

    private bool _isActive;
    private float _currentAngle;
    private float _lastHitTime;
    private AudioSource _audioSource;
    private MaterialPropertyBlock _propertyBlock;
    private JointSpring _spring;

    public bool IsActive => _isActive;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _propertyBlock = new MaterialPropertyBlock();
        _currentAngle = restAngle;
    }

    private void Start()
    {
        if (hingeJoint == null)
        {
            hingeJoint = GetComponent<HingeJoint>();
            if (hingeJoint == null)
                hingeJoint = gameObject.AddComponent<HingeJoint>();
        }

        SetupHinge();
    }

    private void SetupHinge()
    {
        hingeJoint.useSpring = true;
        _spring = hingeJoint.spring;
        _spring.spring = 500f;
        _spring.damper = 10f;
        _spring.targetPosition = restAngle;
        hingeJoint.spring = _spring;

        var limits = hingeJoint.limits;
        limits.min = -35f;
        limits.max = 35f;
        limits.bounciness = 0f;
        hingeJoint.limits = limits;
        hingeJoint.useLimits = true;
    }

    private void Update()
    {
        HandleInput();
        UpdateFlipper();
        UpdateVisuals();
    }

    private void HandleInput()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        KeyCode key = isLeftFlipper ? leftFlipperKey : rightFlipperKey;
        KeyCode touchKey = isLeftFlipper ? touchLeftKey : touchRightKey;

        bool pressed = Input.GetKey(key) || Input.GetKey(touchKey) || GetTouchInput();
        _isActive = pressed;
    }

    private bool GetTouchInput()
    {
        if (Input.touchCount <= 0) return false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            float screenX = touch.position.x / Screen.width;

            if (isLeftFlipper && screenX < 0.4f) return true;
            if (!isLeftFlipper && screenX > 0.6f) return true;
        }
        return false;
    }

    private void UpdateFlipper()
    {
        float targetAngle = _isActive ? activeAngle : restAngle;
        _currentAngle = Mathf.MoveTowards(_currentAngle, targetAngle, flipSpeed * Time.deltaTime);

        _spring.targetPosition = _currentAngle;
        hingeJoint.spring = _spring;
    }

    public void OnBallHit(BallController ball, Collision collision)
    {
        if (Time.time - _lastHitTime < cooldown) return;
        _lastHitTime = Time.time;

        if (_isActive)
        {
            Vector3 hitDir = Vector3.up;
            if (isLeftFlipper)
                hitDir = (Vector3.up + Vector3.forward + Vector3.right * 0.3f).normalized;
            else
                hitDir = (Vector3.up + Vector3.forward + Vector3.left * 0.3f).normalized;

            ball.ApplyForce(hitDir * hitForce, ForceMode.Impulse);
            GameManager.Instance?.AddScore(10);
            PlaySound(hitSound, 1f);
        }
        else
        {
            PlaySound(hitSound, 0.5f);
        }
    }

    private void UpdateVisuals()
    {
        if (flipperRenderer == null) return;

        Color currentColor = Color.Lerp(restColor, activeColor, _isActive ? 1f : 0f);
        flipperRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor("_BaseColor", currentColor);
        _propertyBlock.SetColor("_EmissionColor", currentColor * (_isActive ? 1.5f : 0.3f));
        flipperRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (_audioSource != null && clip != null)
            _audioSource.PlayOneShot(clip, volume);
    }
}
