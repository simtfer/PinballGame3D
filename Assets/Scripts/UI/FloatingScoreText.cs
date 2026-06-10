using UnityEngine;
using TMPro;

public class FloatingScoreText : MonoBehaviour
{
    [Header("Settings")]
    public float floatSpeed = 2f;
    public float fadeDuration = 1f;
    public float scaleUpSpeed = 5f;
    public float maxSize = 1.5f;

    [Header("References")]
    public TextMeshPro textMesh;
    public TextMeshProUGUI uiText;

    private float _timer;
    private float _startScale;

    public static FloatingScoreText Create(Vector3 worldPos, int score, float multiplier, Transform cameraTransform)
    {
        GameObject go = new GameObject("FloatingScore");
        go.transform.position = worldPos + Vector3.up * 0.5f;

        FloatingScoreText fst = go.AddComponent<FloatingScoreText>();
        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        fst.textMesh = tmp;

        tmp.fontSize = 3f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = GetScoreColor(score);

        if (multiplier > 1f)
            tmp.text = $"+{score}\n<multiply>x{multiplier:F1}</multiply>";
        else
            tmp.text = $"+{score}";

        return fst;
    }

    private void Start()
    {
        _startScale = transform.localScale.x;
        _timer = fadeDuration;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        float currentScale = Mathf.MoveTowards(transform.localScale.x, maxSize, scaleUpSpeed * Time.deltaTime);
        transform.localScale = Vector3.one * currentScale;

        float alpha = Mathf.Clamp01(_timer / fadeDuration);
        if (textMesh != null)
        {
            Color c = textMesh.color;
            c.a = alpha;
            textMesh.color = c;
        }

        if (_timer <= 0)
            Destroy(gameObject);
    }

    private static Color GetScoreColor(int score)
    {
        if (score >= 1000) return new Color(1f, 0.2f, 0.2f);
        if (score >= 500) return new Color(1f, 0.8f, 0f);
        if (score >= 100) return new Color(0.2f, 1f, 0.5f);
        return Color.white;
    }
}
