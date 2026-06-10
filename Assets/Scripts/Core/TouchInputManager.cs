using UnityEngine;

public class TouchInputManager : MonoBehaviour
{
    [Header("Touch Settings")]
    public float swipeThreshold = 50f;
    public float tapMaxDuration = 0.3f;

    [Header("Flipper Zones")]
    public float leftFlipperZoneWidth = 0.4f;
    public float rightFlipperZoneWidth = 0.4f;

    [Header("Visual Feedback")]
    public GameObject touchIndicatorPrefab;
    public float indicatorFadeDuration = 0.3f;

    private Vector2 _touchStartPosition;
    private float _touchStartTime;
    private bool _leftFlipperActive;
    private bool _rightFlipperActive;
    private GameObject _leftIndicator;
    private GameObject _rightIndicator;

    public event System.Action<Vector2> OnSwipe;
    public event System.Action<Vector2> OnTap;
    public event System.Action<bool> OnLeftFlipper;
    public event System.Action<bool> OnRightFlipper;

    private void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0)
        {
            if (_leftFlipperActive)
            {
                _leftFlipperActive = false;
                OnLeftFlipper?.Invoke(false);
            }
            if (_rightFlipperActive)
            {
                _rightFlipperActive = false;
                OnRightFlipper?.Invoke(false);
            }
            return;
        }

        bool leftDetected = false;
        bool rightDetected = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            float normalizedX = touch.position.x / Screen.width;

            if (normalizedX < leftFlipperZoneWidth)
            {
                leftDetected = true;
                if (touch.phase == TouchPhase.Began)
                {
                    _touchStartPosition = touch.position;
                    _touchStartTime = Time.time;
                }
            }
            else if (normalizedX > 1f - rightFlipperZoneWidth)
            {
                rightDetected = true;
                if (touch.phase == TouchPhase.Began)
                {
                    _touchStartPosition = touch.position;
                    _touchStartTime = Time.time;
                }
            }
            else if (touch.phase == TouchPhase.Began)
            {
                _touchStartPosition = touch.position;
                _touchStartTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                HandleSwipeOrTap(touch.position);
            }
        }

        if (leftDetected && !_leftFlipperActive)
        {
            _leftFlipperActive = true;
            OnLeftFlipper?.Invoke(true);
        }
        else if (!leftDetected && _leftFlipperActive)
        {
            _leftFlipperActive = false;
            OnLeftFlipper?.Invoke(false);
        }

        if (rightDetected && !_rightFlipperActive)
        {
            _rightFlipperActive = true;
            OnRightFlipper?.Invoke(true);
        }
        else if (!rightDetected && _rightFlipperActive)
        {
            _rightFlipperActive = false;
            OnRightFlipper?.Invoke(false);
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _touchStartPosition = Input.mousePosition;
            _touchStartTime = Time.time;

            float normalizedX = Input.mousePosition.x / Screen.width;
            if (normalizedX < leftFlipperZoneWidth)
            {
                _leftFlipperActive = true;
                OnLeftFlipper?.Invoke(true);
            }
            else if (normalizedX > 1f - rightFlipperZoneWidth)
            {
                _rightFlipperActive = true;
                OnRightFlipper?.Invoke(true);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_leftFlipperActive)
            {
                _leftFlipperActive = false;
                OnLeftFlipper?.Invoke(false);
            }
            if (_rightFlipperActive)
            {
                _rightFlipperActive = false;
                OnRightFlipper?.Invoke(false);
            }

            HandleSwipeOrTap(Input.mousePosition);
        }
    }

    private void HandleSwipeOrTap(Vector2 endPosition)
    {
        Vector2 delta = endPosition - _touchStartPosition;
        float duration = Time.time - _touchStartTime;

        if (delta.magnitude > swipeThreshold && duration > tapMaxDuration)
        {
            OnSwipe?.Invoke(delta.normalized);
        }
        else if (duration <= tapMaxDuration)
        {
            OnTap?.Invoke(endPosition);
        }
    }
}
