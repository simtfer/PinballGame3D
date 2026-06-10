using UnityEngine;

public class ScorePopupSpawner : MonoBehaviour
{
    [Header("Settings")]
    public Camera uiCamera;
    public Transform ballTransform;

    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnScoreChanged += OnScoreChanged;

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

        float multiplier = GameManager.Instance != null ? GameManager.Instance.ComboMultiplier : 1f;
        int baseScore = GameManager.Instance != null
            ? Mathf.RoundToInt(newScore / multiplier)
            : 100;

        SpawnPopup(ballTransform.position, baseScore, multiplier);
    }

    public void SpawnPopup(Vector3 worldPos, int score, float multiplier)
    {
        FloatingScoreText.Create(worldPos, score, multiplier, uiCamera.transform);
    }
}
