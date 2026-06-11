using UnityEngine;

public class LaunchLaneReturnDeflectorController : MonoBehaviour
{
    [Header("Deflector Settings")]
    public Vector3 launchDirection = Vector3.forward;
    public Vector3 returnRedirectDirection = new Vector3(-1f, 0f, 0.6f);
    public float launchNudgeForce = 1f;
    public float returnRedirectForce = 7f;

    [Header("Visual")]
    public MeshRenderer deflectorRenderer;
    public Color idleColor = new Color(1f, 0.8f, 0.1f);
    public Color redirectColor = new Color(1f, 0.25f, 0.1f);

    private MaterialPropertyBlock _propertyBlock;
    private int _baseColorId;
    private float _flashTimer;

    private void Awake()
    {
        Collider deflectorCollider = GetComponent<Collider>();
        if (deflectorCollider == null)
            deflectorCollider = gameObject.AddComponent<BoxCollider>();
        deflectorCollider.isTrigger = true;

        if (deflectorRenderer == null)
            deflectorRenderer = GetComponent<MeshRenderer>();

        _propertyBlock = new MaterialPropertyBlock();
        _baseColorId = Shader.PropertyToID("_BaseColor");
    }

    private void Start()
    {
        UpdateVisual(idleColor);
    }

    private void Update()
    {
        if (_flashTimer <= 0f) return;

        _flashTimer -= Time.deltaTime;
        if (_flashTimer <= 0f)
            UpdateVisual(idleColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        BallController ball = other.GetComponent<BallController>();
        if (ball == null)
            return;

        Vector3 launch = transform.TransformDirection(launchDirection.normalized);
        float velocityAlongLaunch = Vector3.Dot(ball.Velocity, launch);

        if (velocityAlongLaunch > 0f)
        {
            ball.ApplyForce(launch * launchNudgeForce, ForceMode.VelocityChange);
            return;
        }

        Vector3 redirect = transform.TransformDirection(returnRedirectDirection.normalized);
        ball.ApplyForce(redirect * returnRedirectForce, ForceMode.VelocityChange);

        _flashTimer = 0.15f;
        UpdateVisual(redirectColor);
    }

    private void UpdateVisual(Color color)
    {
        if (deflectorRenderer == null)
            return;

        deflectorRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(_baseColorId, color);
        _propertyBlock.SetColor("_EmissionColor", color * 1.5f);
        deflectorRenderer.SetPropertyBlock(_propertyBlock);
    }
}
