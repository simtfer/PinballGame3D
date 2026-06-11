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
    private Material _transparentMat;

    private Shader FindShader()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader != null) return shader;
        
        shader = Shader.Find("HDRP/Lit");
        if (shader != null) return shader;
        
        shader = Shader.Find("Standard");
        if (shader != null) return shader;
        
        return Shader.Find("Diffuse");
    }

    private Material CreateMaterial()
    {
        Shader shader = FindShader();
        if (shader == null)
        {
            Debug.LogError("CyberpunkMaterialGenerator: No valid shader found!");
            return new Material(Shader.Find("Diffuse"));
        }
        return new Material(shader);
    }

    public void GenerateMaterials()
    {
        CreateTableSurfaceMaterial();
        CreateWallMaterial();
        CreateBumperMaterial();
        CreateFlipperMaterial();
        CreateSlingshotMaterial();
        CreateNeonMaterial();
        CreateTransparentMaterial();
    }

    private void CreateTableSurfaceMaterial()
    {
        _tableSurfaceMat = CreateMaterial();
        _tableSurfaceMat.SetColor("_BaseColor", baseColor);
        _tableSurfaceMat.SetColor("_Color", baseColor);
        _tableSurfaceMat.SetColor("_EmissionColor", baseColor * 0.3f);
        _tableSurfaceMat.SetFloat("_Smoothness", 0.8f);
        _tableSurfaceMat.SetFloat("_Metallic", 0.2f);
        _tableSurfaceMat.EnableKeyword("_EMISSION");
    }

    private void CreateWallMaterial()
    {
        _wallMat = CreateMaterial();
        _wallMat.SetColor("_BaseColor", wallPurple);
        _wallMat.SetColor("_Color", wallPurple);
        _wallMat.SetColor("_EmissionColor", neonCyan * emissionIntensity * 0.5f);
        _wallMat.SetFloat("_Smoothness", 0.6f);
        _wallMat.SetFloat("_Metallic", 0.3f);
        _wallMat.EnableKeyword("_EMISSION");
    }

    private void CreateBumperMaterial()
    {
        _bumperMat = CreateMaterial();
        _bumperMat.SetColor("_BaseColor", neonPink);
        _bumperMat.SetColor("_Color", neonPink);
        _bumperMat.SetColor("_EmissionColor", neonPink * emissionIntensity);
        _bumperMat.SetFloat("_Smoothness", 0.9f);
        _bumperMat.SetFloat("_Metallic", 0.1f);
        _bumperMat.EnableKeyword("_EMISSION");
    }

    private void CreateFlipperMaterial()
    {
        _flipperMat = CreateMaterial();
        _flipperMat.SetColor("_BaseColor", neonCyan);
        _flipperMat.SetColor("_Color", neonCyan);
        _flipperMat.SetColor("_EmissionColor", neonCyan * emissionIntensity);
        _flipperMat.SetFloat("_Smoothness", 0.95f);
        _flipperMat.SetFloat("_Metallic", 0.8f);
        _flipperMat.EnableKeyword("_EMISSION");
    }

    private void CreateSlingshotMaterial()
    {
        _slingshotMat = CreateMaterial();
        _slingshotMat.SetColor("_BaseColor", neonPink);
        _slingshotMat.SetColor("_Color", neonPink);
        _slingshotMat.SetColor("_EmissionColor", neonPink * emissionIntensity);
        _slingshotMat.SetFloat("_Smoothness", 0.7f);
        _slingshotMat.SetFloat("_Metallic", 0.2f);
        _slingshotMat.EnableKeyword("_EMISSION");
    }

    private void CreateNeonMaterial()
    {
        _neonMat = CreateMaterial();
        _neonMat.SetColor("_BaseColor", neonCyan);
        _neonMat.SetColor("_Color", neonCyan);
        _neonMat.SetColor("_EmissionColor", neonCyan * emissionIntensity * 1.5f);
        _neonMat.SetFloat("_Smoothness", 1f);
        _neonMat.SetFloat("_Metallic", 0f);
        _neonMat.EnableKeyword("_EMISSION");
    }

    private void CreateTransparentMaterial()
    {
        _transparentMat = CreateMaterial();
        Color transparentColor = new Color(0.1f, 0.15f, 0.3f, 0.2f);
        _transparentMat.SetColor("_BaseColor", transparentColor);
        _transparentMat.SetColor("_Color", transparentColor);
        _transparentMat.SetFloat("_Smoothness", 0.95f);
        _transparentMat.SetFloat("_Metallic", 0.1f);
        
        if (_transparentMat.HasProperty("_Surface"))
            _transparentMat.SetFloat("_Surface", 1f);
        if (_transparentMat.HasProperty("_Blend"))
            _transparentMat.SetFloat("_Blend", 0f);
        if (_transparentMat.HasProperty("_SrcBlend"))
            _transparentMat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        if (_transparentMat.HasProperty("_DstBlend"))
            _transparentMat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        if (_transparentMat.HasProperty("_ZWrite"))
            _transparentMat.SetFloat("_ZWrite", 0);
        
        _transparentMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        _transparentMat.SetOverrideTag("RenderType", "Transparent");
        _transparentMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        _transparentMat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
    }

    public Material GetTableSurfaceMaterial() => _tableSurfaceMat;
    public Material GetWallMaterial() => _wallMat;
    public Material GetBumperMaterial() => _bumperMat;
    public Material GetFlipperMaterial() => _flipperMat;
    public Material GetSlingshotMaterial() => _slingshotMat;
    public Material GetNeonMaterial() => _neonMat;
    public Material GetTransparentMaterial() => _transparentMat;
}
