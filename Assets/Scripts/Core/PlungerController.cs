using UnityEngine;

public class PlungerController : MonoBehaviour
{
    [Header("Plunger Settings")]
    public float maxPullDistance = 1.5f;
    public float springForce = 50f;
    public float dampingFactor = 0.8f;
    public Transform plungerMesh;

    [Header("Input")]
    public KeyCode pullKey = KeyCode.Space;

    [Header("Visual")]
    public MeshRenderer plungerRenderer;
    public Color idleColor = new Color(0.3f, 0.3f, 0.8f);
    public Color pulledColor = new Color(1f, 0.3f, 0.3f);

    [Header("Audio")]
    public AudioClip pullSound;
    public AudioClip launchSound;

    private float _pullAmount;
    private bool _isPulling;
    private Vector3 _startPosition;
    private AudioSource _audioSource;
    private BallController _ball;
    private MaterialPropertyBlock _propertyBlock;

    public float PullAmount => _pullAmount;
    public float LaunchForce => _pullAmount * springForce;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _startPosition = plungerMesh != null ? plungerMesh.localPosition : transform.position;
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        _ball = GameManager.Instance != null ? GameManager.Instance.ball : FindObjectOfType<BallController>();
    }

    private void Update()
    {
        HandleInput();
        UpdateVisuals();
    }

    private void HandleInput()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;
        if (_ball == null || _ball.IsLaunched) return;

        if (Input.GetKeyDown(pullKey) || GetTouchDown())
        {
            _isPulling = true;
            if (_audioSource != null && pullSound != null)
                _audioSource.PlayOneShot(pullSound);
        }

        if (_isPulling)
        {
            _pullAmount = Mathf.Min(_pullAmount + Time.deltaTime * 1.5f, 1f);

            if (Input.GetKeyUp(pullKey) || GetTouchUp())
            {
                _isPulling = false;
                Launch();
            }
        }
    }

    private void Launch()
    {
        if (_ball == null || _pullAmount < 0.05f) return;

        float force = LaunchForce;
        _ball.LaunchBall(Vector3.forward, force);

        if (_audioSource != null && launchSound != null)
            _audioSource.PlayOneShot(launchSound);

        _pullAmount = 0f;
    }

    public void ResetPlunger()
    {
        _pullAmount = 0f;
        _isPulling = false;
    }

    private void UpdateVisuals()
    {
        if (plungerMesh != null)
        {
            Vector3 pos = _startPosition;
            pos.y -= _pullAmount * maxPullDistance;
            plungerMesh.localPosition = pos;
        }

        if (plungerRenderer != null)
        {
            Color currentColor = Color.Lerp(idleColor, pulledColor, _pullAmount);
            plungerRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", currentColor);
            _propertyBlock.SetColor("_EmissionColor", currentColor * _pullAmount * 2f);
            plungerRenderer.SetPropertyBlock(_propertyBlock);
        }
    }

    private bool GetTouchDown()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began)
                    return true;
            }
        }
        return false;
    }

    private bool GetTouchUp()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Ended)
                    return true;
            }
        }
        return false;
    }
}
