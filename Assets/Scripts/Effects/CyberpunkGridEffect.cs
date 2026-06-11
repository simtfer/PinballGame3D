using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class CyberpunkGridEffect : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridScale = 10f;
    public float pulseSpeed = 2f;
    public Color gridColor = Color.cyan;

    private MeshRenderer _renderer;
    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        _renderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat("_GridScale", gridScale);
        _propertyBlock.SetFloat("_PulseSpeed", pulseSpeed);
        _propertyBlock.SetColor("_GridColor", gridColor);
        _renderer.SetPropertyBlock(_propertyBlock);
    }
}
