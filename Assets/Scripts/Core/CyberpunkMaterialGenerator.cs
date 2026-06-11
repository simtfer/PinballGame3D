using UnityEngine;

public class CyberpunkMaterialGenerator : MonoBehaviour
{
    [Header("Cyberpunk Colors")]
    public Color baseColor = new Color(0.04f, 0.04f, 0.18f);
    public Color neonPink = new Color(1f, 0.176f, 0.584f);
    public Color neonCyan = new Color(0f, 1f, 1f);
    public Color wallPurple = new Color(0.2f, 0.1f, 0.3f);

    [Header("Material Settings")]
    public float emissionIntensity = 2f;
    public float pulseSpeed = 1.5f;

    private Material _tableSurfaceMat;
    private Material _wallMat;
    private Material _bumperMat;
    private Material _flipperMat;
    private Material _slingshotMat;
    private Material _neonMat;

    public void GenerateMaterials()
    {
        CreateTableSurfaceMaterial();
        CreateWallMaterial();
        CreateBumperMaterial();
        CreateFlipperMaterial();
        CreateSlingshotMaterial();
        CreateNeonMaterial();
    }

    private void CreateTableSurfaceMaterial()
    {
        _tableSurfaceMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        _tableSurfaceMat.SetColor("_BaseColor", baseColor);
        _tableSurfaceMat.SetColor("_EmissionColor", baseColor * 0.3f);
        _tableSurfaceMat.SetFloat("_Smoothness", 0.8f);
        _tableSurfaceMat.SetFloat("_Metallic", 0.2f);
        _tableSurfaceMat.EnableKeyword("_EMISSION");
    }

    private void CreateWallMaterial()
    {
        _wallMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        _wallMat.SetColor("_BaseColor", wallPurple);
        _wallMat.SetColor("_EmissionColor", neonCyan * emissionIntensity * 0.5f);
        _wallMat.SetFloat("_Smoothness", 0.6f);
        _wallMat.SetFloat("_Metallic", 0.3f);
        _wallMat.EnableKeyword("_EMISSION");
    }

    private void CreateBumperMaterial()
    {
        _bumperMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        _bumperMat.SetColor("_BaseColor", neonPink);
        _bumperMat.SetColor("_EmissionColor", neonPink * emissionIntensity);
        _bumperMat.SetFloat("_Smoothness", 0.9f);
        _bumperMat.SetFloat("_Metallic", 0.1f);
        _bumperMat.EnableKeyword("_EMISSION");
    }

    private void CreateFlipperMaterial()
    {
        _flipperMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        _flipperMat.SetColor("_BaseColor", neonCyan);
        _flipperMat.SetColor("_EmissionColor", neonCyan * emissionIntensity);
        _flipperMat.SetFloat("_Smoothness", 0.95f);
        _flipperMat.SetFloat("_Metallic", 0.8f);
        _flipperMat.EnableKeyword("_EMISSION");
    }

    private void CreateSlingshotMaterial()
    {
        _slingshotMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        _slingshotMat.SetColor("_BaseColor", neonPink);
        _slingshotMat.SetColor("_EmissionColor", neonPink * emissionIntensity);
        _slingshotMat.SetFloat("_Smoothness", 0.7f);
        _slingshotMat.SetFloat("_Metallic", 0.2f);
        _slingshotMat.EnableKeyword("_EMISSION");
    }

    private void CreateNeonMaterial()
    {
        _neonMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        _neonMat.SetColor("_BaseColor", neonCyan);
        _neonMat.SetColor("_EmissionColor", neonCyan * emissionIntensity * 1.5f);
        _neonMat.SetFloat("_Smoothness", 1f);
        _neonMat.SetFloat("_Metallic", 0f);
        _neonMat.EnableKeyword("_EMISSION");
    }

    public Material GetTableSurfaceMaterial() => _tableSurfaceMat;
    public Material GetWallMaterial() => _wallMat;
    public Material GetBumperMaterial() => _bumperMat;
    public Material GetFlipperMaterial() => _flipperMat;
    public Material GetSlingshotMaterial() => _slingshotMat;
    public Material GetNeonMaterial() => _neonMat;
}
