using UnityEngine;

public class NeonGlowEffect : MonoBehaviour
{
    [Header("Glow Settings")]
    public Color glowColor = Color.cyan;
    public float glowIntensity = 2f;
    public float pulseSpeed = 1f;
    public float pulseAmplitude = 0.5f;
    public bool enablePulse = true;

    [Header("References")]
    public MeshRenderer targetRenderer;
    public Light pointLight;

    private MaterialPropertyBlock _propertyBlock;
    private float _baseIntensity;
    private int _emissionColorId;

    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
        _emissionColorId = Shader.PropertyToID("_EmissionColor");
        _baseIntensity = glowIntensity;
    }

    private void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<MeshRenderer>();
        if (pointLight == null)
            pointLight = GetComponentInChildren<Light>();
    }

    private void Update()
    {
        float currentIntensity = _baseIntensity;

        if (enablePulse)
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
            currentIntensity = _baseIntensity + pulse;
        }

        if (targetRenderer != null)
        {
            targetRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(_emissionColorId, glowColor * currentIntensity);
            targetRenderer.SetPropertyBlock(_propertyBlock);
        }

        if (pointLight != null)
        {
            pointLight.color = glowColor;
            pointLight.intensity = currentIntensity;
        }
    }

    public void Flash(float intensity = 5f, float duration = 0.3f)
    {
        StartCoroutine(FlashCoroutine(intensity, duration));
    }

    private System.Collections.IEnumerator FlashCoroutine(float intensity, float duration)
    {
        float originalIntensity = _baseIntensity;
        _baseIntensity = intensity;

        float timer = duration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            _baseIntensity = Mathf.Lerp(originalIntensity, intensity, timer / duration);
            yield return null;
        }

        _baseIntensity = originalIntensity;
    }
}
