using UnityEngine;

public class TableLightingController : MonoBehaviour
{
    [Header("Ambient Lighting")]
    public Light ambientLight;
    public Color ambientDayColor = new Color(0.02f, 0.02f, 0.1f);
    public Color ambientNightColor = new Color(0.01f, 0.01f, 0.05f);

    [Header("Spot Lights")]
    public Light[] spotLights;
    public float spotLightIntensity = 4f;
    public float spotLightRange = 10f;

    [Header("Neon Strips")]
    public MeshRenderer[] neonStrips;
    public Color[] neonColors = new Color[]
    {
        new Color(0f, 1f, 1f),
        new Color(1f, 0.176f, 0.584f),
        new Color(0f, 1f, 0.8f),
        new Color(0.8f, 0f, 1f)
    };
    public float neonScrollSpeed = 0.5f;

    [Header("Dynamic Effects")]
    public float comboFlashIntensity = 3f;
    public float comboFlashDuration = 0.2f;
    public Color multiBallColor = Color.red;

    private MaterialPropertyBlock[] _neonPropertyBlocks;
    private int _emissionColorId;

    private void Awake()
    {
        _emissionColorId = Shader.PropertyToID("_EmissionColor");
    }

    private void Start()
    {
        if (neonStrips != null && neonStrips.Length > 0)
        {
            _neonPropertyBlocks = new MaterialPropertyBlock[neonStrips.Length];
            for (int i = 0; i < neonStrips.Length; i++)
                _neonPropertyBlocks[i] = new MaterialPropertyBlock();
        }
    }

    private void Update()
    {
        UpdateNeonStrips();
    }

    private void UpdateNeonStrips()
    {
        if (neonStrips == null || _neonPropertyBlocks == null) return;

        float timeOffset = Time.time * neonScrollSpeed;

        for (int i = 0; i < neonStrips.Length; i++)
        {
            if (neonStrips[i] == null) continue;

            int colorIndex = (i + Mathf.FloorToInt(timeOffset)) % neonColors.Length;
            int nextColorIndex = (colorIndex + 1) % neonColors.Length;
            float t = timeOffset - Mathf.Floor(timeOffset);

            Color currentColor = Color.Lerp(neonColors[colorIndex], neonColors[nextColorIndex], t);

            neonStrips[i].GetPropertyBlock(_neonPropertyBlocks[i]);
            _neonPropertyBlocks[i].SetColor(_emissionColorId, currentColor * 2f);
            _neonPropertyBlocks[i].SetColor("_BaseColor", currentColor);
            neonStrips[i].SetPropertyBlock(_neonPropertyBlocks[i]);
        }
    }

    public void FlashForCombo(int comboCount)
    {
        if (spotLights == null) return;

        float intensity = spotLightIntensity + comboFlashIntensity * Mathf.Min(comboCount, 10);

        foreach (var light in spotLights)
        {
            if (light == null) continue;
            StartCoroutine(FlashLight(light, intensity, comboFlashDuration));
        }
    }

    public void SetMultiBallMode(bool active)
    {
        if (spotLights == null) return;

        foreach (var light in spotLights)
        {
            if (light == null) continue;
            light.color = active ? multiBallColor : Color.white;
            light.intensity = active ? spotLightIntensity * 1.5f : spotLightIntensity;
        }
    }

    private System.Collections.IEnumerator FlashLight(Light light, float targetIntensity, float duration)
    {
        float originalIntensity = light.intensity;
        float timer = duration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float t = timer / duration;
            light.intensity = Mathf.Lerp(originalIntensity, targetIntensity, t);
            yield return null;
        }

        light.intensity = originalIntensity;
    }
}
