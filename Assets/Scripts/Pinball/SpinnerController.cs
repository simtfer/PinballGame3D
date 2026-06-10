using UnityEngine;

public class SpinnerController : MonoBehaviour, IPinballTrigger
{
    [Header("Spinner Settings")]
    public int scorePerSpin = 75;
    public float spinSpeed = 720f;
    public float slowdownRate = 100f;

    [Header("Visual")]
    public Transform spinnerMesh;
    public MeshRenderer spinnerRenderer;
    public Color idleColor = new Color(0.8f, 0.8f, 0.2f);
    public Color spinColor = new Color(1f, 1f, 0.5f);

    [Header("Audio")]
    public AudioClip spinSound;

    private float _currentSpinSpeed;
    private AudioSource _audioSource;
    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (_currentSpinSpeed > 0)
        {
            if (spinnerMesh != null)
                spinnerMesh.Rotate(Vector3.up, _currentSpinSpeed * Time.deltaTime);

            _currentSpinSpeed = Mathf.Max(0, _currentSpinSpeed - slowdownRate * Time.deltaTime);
        }

        UpdateVisuals();
    }

    public void OnBallEnter(BallController ball)
    {
        float ballSpeed = ball.Speed;
        _currentSpinSpeed = spinSpeed * Mathf.Clamp01(ballSpeed / 15f);

        GameManager.Instance?.AddScore(scorePerSpin);

        if (_audioSource != null && spinSound != null)
        {
            float pitch = 0.8f + Mathf.Clamp01(ballSpeed / 20f) * 0.4f;
            _audioSource.pitch = pitch;
            _audioSource.PlayOneShot(spinSound, 0.6f);
        }
    }

    private void UpdateVisuals()
    {
        if (spinnerRenderer == null) return;

        float spinRatio = _currentSpinSpeed / spinSpeed;
        Color currentColor = Color.Lerp(idleColor, spinColor, spinRatio);

        spinnerRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor("_BaseColor", currentColor);
        _propertyBlock.SetColor("_EmissionColor", currentColor * (0.2f + spinRatio * 2f));
        spinnerRenderer.SetPropertyBlock(_propertyBlock);
    }
}
