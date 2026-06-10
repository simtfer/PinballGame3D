using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 8f, -6f);
    public Vector3 lookAtOffset = new Vector3(0f, 0f, 2f);
    public float followSpeed = 5f;
    public float lookAtSpeed = 3f;

    [Header("Ball Tracking")]
    public bool trackBall = true;
    public float ballTrackWeight = 0.3f;
    public float maxTrackDistance = 3f;

    [Header("Effects")]
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.2f;
    public float fovChangeSpeed = 2f;
    public float maxFOVChange = 10f;

    [Header("Zoom")]
    public float baseFOV = 60f;
    public float zoomFOV = 45f;
    public bool autoZoomOnSlowBall = true;
    public float slowBallThreshold = 2f;

    private float _currentFOV;
    private float _shakeTimer;
    private Vector3 _shakeOffset;
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera == null)
            _camera = gameObject.AddComponent<Camera>();

        _currentFOV = baseFOV;
    }

    private void Start()
    {
        if (target == null && GameManager.Instance != null && GameManager.Instance.ball != null)
            target = GameManager.Instance.ball.transform;
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
        UpdateFOV();
        UpdateShake();
    }

    private void UpdateCameraPosition()
    {
        Vector3 desiredPos = transform.position;

        if (target != null && trackBall)
        {
            Vector3 ballOffset = target.position + offset;
            Vector3 trackPos = Vector3.Lerp(
                transform.parent != null ? transform.parent.position + offset : offset,
                ballOffset,
                ballTrackWeight
            );

            Vector3 targetPos = Vector3.Lerp(transform.position, trackPos, followSpeed * Time.deltaTime);
            desiredPos = targetPos;

            Vector3 lookTarget = target.position + lookAtOffset;
            Quaternion targetRot = Quaternion.LookRotation(lookTarget - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, lookAtSpeed * Time.deltaTime);
        }
        else
        {
            desiredPos = offset;
            transform.LookAt(lookAtOffset);
        }

        transform.position = desiredPos + _shakeOffset;
    }

    private void UpdateFOV()
    {
        float targetFOV = baseFOV;

        if (target != null && autoZoomOnSlowBall)
        {
            BallController ball = target.GetComponent<BallController>();
            if (ball != null && ball.Speed < slowBallThreshold && ball.IsLaunched)
            {
                targetFOV = zoomFOV;
            }
        }

        if (target != null)
        {
            BallController ball = target.GetComponent<BallController>();
            if (ball != null)
            {
                float speedRatio = ball.Speed / 25f;
                targetFOV += speedRatio * maxFOVChange;
            }
        }

        _currentFOV = Mathf.Lerp(_currentFOV, targetFOV, fovChangeSpeed * Time.deltaTime);
        _camera.fieldOfView = _currentFOV;
    }

    private void UpdateShake()
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;
            float intensity = _shakeTimer / shakeDuration;
            _shakeOffset = Random.insideUnitSphere * shakeIntensity * intensity;
            _shakeOffset.y = Mathf.Abs(_shakeOffset.y);
        }
        else
        {
            _shakeOffset = Vector3.zero;
        }
    }

    public void Shake(float intensity = -1f, float duration = -1f)
    {
        _shakeTimer = duration > 0 ? duration : shakeDuration;
        if (intensity > 0) shakeIntensity = intensity;
    }

    public void FocusOnBall(float duration = 2f)
    {
        if (target == null) return;
        // Temporarily increase ball tracking weight
        StartCoroutine(FocusCoroutine(duration));
    }

    private System.Collections.IEnumerator FocusCoroutine(float duration)
    {
        float originalWeight = ballTrackWeight;
        ballTrackWeight = 0.8f;
        yield return new WaitForSeconds(duration);
        ballTrackWeight = originalWeight;
    }
}
