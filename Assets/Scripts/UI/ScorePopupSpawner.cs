using UnityEngine;

public class ScorePopupSpawner : MonoBehaviour
{
    [Header("Settings")]
    public Camera uiCamera;
    public Transform ballTransform;

    private int _lastScore;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += OnScoreChanged;
            _lastScore = GameManager.Instance.Score;
        }

        if (uiCamera == null)
            uiCamera = Camera.main;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnScoreChanged -= OnScoreChanged;
    }

    private void OnScoreChanged(int newScore)
    {
        if (ballTransform == null || uiCamera == null) return;

        int delta = newScore - _lastScore;
        _lastScore = newScore;

        if (delta <= 0) return;

        float multiplier = GameManager.Instance != null ? GameManager.Instance.ComboMultiplier : 1f;
        SpawnPopup(ballTransform.position, delta, multiplier);
    }

    public void SpawnPopup(Vector3 worldPos, int score, float multiplier)
    {
        FloatingScoreText.Create(worldPos, score, multiplier, uiCamera.transform);
    }
}
