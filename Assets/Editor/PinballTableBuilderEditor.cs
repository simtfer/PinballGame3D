#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PinballTableBuilder))]
public class PinballTableBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PinballTableBuilder builder = (PinballTableBuilder)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Table Builder Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Build Table", GUILayout.Height(30)))
        {
            Undo.RecordObject(builder.gameObject, "Build Table");
            builder.BuildTable();
            EditorUtility.SetDirty(builder.gameObject);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Prefabs", GUILayout.Height(25)))
        {
            CreateDefaultPrefabs();
        }

        if (GUILayout.Button("Setup Lighting", GUILayout.Height(25)))
        {
            SetupSceneLighting();
        }

        if (GUILayout.Button("Create Materials", GUILayout.Height(25)))
        {
            CreateDefaultMaterials();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Cyberpunk Style", EditorStyles.boldLabel);

        if (GUILayout.Button("Setup Cyberpunk Generator", GUILayout.Height(25)))
        {
            SetupCyberpunkGenerator();
        }
    }

    private void CreateDefaultPrefabs()
    {
        string prefabPath = "Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabPath))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        CreateBumperPrefab(prefabPath);
        CreateFlipperPrefab(prefabPath);
        CreateBallPrefab(prefabPath);
        CreateSlingshotPrefab(prefabPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Default prefabs created in Assets/Prefabs/");
    }

    private void CreateBumperPrefab(string path)
    {
        GameObject bumper = new GameObject("Bumper");
        MeshFilter mf = bumper.AddComponent<MeshFilter>();
        mf.mesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
        MeshRenderer mr = bumper.AddComponent<MeshRenderer>();
        bumper.AddComponent<BumperController>();
        bumper.AddComponent<SphereCollider>();

        PrefabUtility.SaveAsPrefabAsset(bumper, $"{path}/Bumper.prefab");
        DestroyImmediate(bumper);
    }

    private void CreateFlipperPrefab(string path)
    {
        GameObject flipper = new GameObject("Flipper");
        MeshFilter mf = flipper.AddComponent<MeshFilter>();
        mf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        MeshRenderer mr = flipper.AddComponent<MeshRenderer>();
        flipper.AddComponent<FlipperController>();
        flipper.AddComponent<HingeJoint>();
        flipper.transform.localScale = new Vector3(1.2f, 0.15f, 0.3f);

        PrefabUtility.SaveAsPrefabAsset(flipper, $"{path}/Flipper.prefab");
        DestroyImmediate(flipper);
    }

    private void CreateBallPrefab(string path)
    {
        GameObject ball = new GameObject("Ball");
        MeshFilter mf = ball.AddComponent<MeshFilter>();
        mf.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
        MeshRenderer mr = ball.AddComponent<MeshRenderer>();
        ball.AddComponent<BallController>();
        ball.AddComponent<SphereCollider>();
        ball.AddComponent<Rigidbody>();
        ball.AddComponent<AudioSource>();
        ball.transform.localScale = Vector3.one * 0.3f;

        TrailRenderer trail = ball.AddComponent<TrailRenderer>();
        trail.time = 0.5f;
        trail.startWidth = 0.1f;
        trail.endWidth = 0f;

        PrefabUtility.SaveAsPrefabAsset(ball, $"{path}/Ball.prefab");
        DestroyImmediate(ball);
    }

    private void CreateSlingshotPrefab(string path)
    {
        GameObject slingshot = new GameObject("Slingshot");
        MeshFilter mf = slingshot.AddComponent<MeshFilter>();
        mf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        MeshRenderer mr = slingshot.AddComponent<MeshRenderer>();
        slingshot.AddComponent<SlingshotController>();
        slingshot.AddComponent<BoxCollider>();
        slingshot.transform.localScale = new Vector3(0.8f, 0.4f, 0.15f);

        PrefabUtility.SaveAsPrefabAsset(slingshot, $"{path}/Slingshot.prefab");
        DestroyImmediate(slingshot);
    }

    private void SetupSceneLighting()
    {
        Light mainLight = Object.FindObjectOfType<Light>();
        if (mainLight == null)
        {
            GameObject lightObj = new GameObject("DirectionalLight");
            mainLight = lightObj.AddComponent<Light>();
        }

        mainLight.type = LightType.Directional;
        mainLight.color = new Color(0.8f, 0.8f, 1f);
        mainLight.intensity = 1.5f;
        mainLight.shadows = LightShadows.Soft;
        mainLight.transform.rotation = Quaternion.Euler(50, -30, 0);

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.2f);

        Debug.Log("Scene lighting configured");
    }

    private void CreateDefaultMaterials()
    {
        string matPath = "Assets/Materials";
        if (!AssetDatabase.IsValidFolder(matPath))
            AssetDatabase.CreateFolder("Assets", "Materials");

        Shader neonShader = Shader.Find("Pinball/NeonGlow");
        if (neonShader == null)
            neonShader = Shader.Find("Universal Render Pipeline/Lit");
        if (neonShader == null)
            neonShader = Shader.Find("Standard");

        Shader glassShader = Shader.Find("Universal Render Pipeline/Lit");
        if (glassShader == null)
            glassShader = Shader.Find("Standard");
        if (glassShader == null)
            glassShader = neonShader;

        CreateMaterial(matPath, "TableSurface", neonShader, new Color(0.015f, 0.02f, 0.06f, 1f), new Color(0.02f, 0.08f, 0.16f, 1f), 0.7f);
        CreateMaterial(matPath, "Wall", neonShader, new Color(0.02f, 0.08f, 0.12f, 1f), new Color(0f, 0.9f, 1f, 1f), 1.8f);
        CreateMaterial(matPath, "Bumper", neonShader, new Color(0.05f, 0.08f, 0.18f, 1f), new Color(1f, 0.92f, 0.08f, 1f), 2.1f);
        CreateMaterial(matPath, "Flipper", neonShader, new Color(1f, 0.02f, 0.38f, 1f), new Color(1f, 0f, 0.72f, 1f), 1.9f);
        CreateMaterial(matPath, "NeonStrip", neonShader, new Color(0.005f, 0.005f, 0.02f, 1f), new Color(0f, 1f, 1f, 1f), 2.4f);
        CreateMaterial(matPath, "Ball", neonShader, new Color(0.82f, 0.95f, 1f, 1f), new Color(0f, 0.85f, 1f, 1f), 1.4f);

        Material safetyPanel = CreateMaterial(matPath, "SafetyPanel", glassShader, new Color(0.05f, 0.9f, 1f, 0.28f), new Color(0f, 0.85f, 1f, 1f), 0.8f);
        ConfigureTransparentMaterial(safetyPanel, new Color(0.05f, 0.9f, 1f, 0.28f));

        AssignCreatedMaterials(matPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Default materials created in Assets/Materials/");
    }

    private Material CreateMaterial(string path, string name, Shader shader, Color baseColor, Color emissionColor, float glowIntensity)
    {
        string assetPath = $"{path}/{name}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, assetPath);
        }
        else if (shader != null && mat.shader != shader)
        {
            mat.shader = shader;
        }

        SetMaterialColor(mat, "_BaseColor", baseColor);
        SetMaterialColor(mat, "_Color", baseColor);
        SetMaterialColor(mat, "_EmissionColor", emissionColor);

        if (mat.HasProperty("_GlowIntensity"))
            mat.SetFloat("_GlowIntensity", glowIntensity);
        if (mat.HasProperty("_Smoothness"))
            mat.SetFloat("_Smoothness", 0.78f);
        if (mat.HasProperty("_Metallic"))
            mat.SetFloat("_Metallic", 0.05f);

        if (emissionColor.maxColorComponent > 0f)
            mat.EnableKeyword("_EMISSION");

        EditorUtility.SetDirty(mat);
        return mat;
    }

    private void ConfigureTransparentMaterial(Material material, Color glassColor)
    {
        if (material == null)
            return;

        SetMaterialColor(material, "_BaseColor", glassColor);
        SetMaterialColor(material, "_Color", glassColor);

        if (material.HasProperty("_Surface"))
            material.SetFloat("_Surface", 1f);
        if (material.HasProperty("_Blend"))
            material.SetFloat("_Blend", 0f);
        if (material.HasProperty("_AlphaClip"))
            material.SetFloat("_AlphaClip", 0f);
        if (material.HasProperty("_Mode"))
            material.SetFloat("_Mode", 3f);
        if (material.HasProperty("_SrcBlend"))
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        if (material.HasProperty("_DstBlend"))
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        if (material.HasProperty("_ZWrite"))
            material.SetInt("_ZWrite", 0);

        material.SetOverrideTag("RenderType", "Transparent");
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        EditorUtility.SetDirty(material);
    }

    private void AssignCreatedMaterials(string path)
    {
        PinballTableBuilder builder = (PinballTableBuilder)target;
        Undo.RecordObject(builder, "Assign Pinball Materials");
        builder.tableSurfaceMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{path}/TableSurface.mat");
        builder.wallMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{path}/Wall.mat");
        builder.bumperMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{path}/Bumper.mat");
        builder.flipperMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{path}/Flipper.mat");
        builder.neonMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{path}/NeonStrip.mat");
        builder.safetyPanelMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{path}/SafetyPanel.mat");
        EditorUtility.SetDirty(builder);
    }

    private void SetupCyberpunkGenerator()
    {
        PinballTableBuilder builder = (PinballTableBuilder)target;
        Undo.RecordObject(builder, "Setup Cyberpunk Generator");

        CyberpunkMaterialGenerator generator = builder.GetComponent<CyberpunkMaterialGenerator>();
        if (generator == null)
        {
            generator = Undo.AddComponent<CyberpunkMaterialGenerator>(builder.gameObject);
        }

        builder.cyberpunkGenerator = generator;
        builder.useCyberpunkStyle = true;
        EditorUtility.SetDirty(builder);

        GameSceneSetup setup = FindObjectOfType<GameSceneSetup>();
        if (setup != null)
        {
            Undo.RecordObject(setup, "Wire Cyberpunk Generator");
            setup.cyberpunkGenerator = generator;
            EditorUtility.SetDirty(setup);
        }

        Debug.Log("Cyberpunk generator wired up to PinballTableBuilder and GameSceneSetup");
    }

    private void SetMaterialColor(Material material, string propertyName, Color color)
    {
        if (material.HasProperty(propertyName))
            material.SetColor(propertyName, color);
    }
}
#endif
