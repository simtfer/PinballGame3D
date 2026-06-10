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
        if (neonShader == null) neonShader = Shader.Find("Universal Render Pipeline/Lit");

        CreateMaterial(matPath, "TableSurface", neonShader, new Color(0.05f, 0.05f, 0.15f), new Color(0.1f, 0.1f, 0.3f));
        CreateMaterial(matPath, "Wall", neonShader, new Color(0.2f, 0.2f, 0.4f), new Color(0.3f, 0.3f, 0.8f));
        CreateMaterial(matPath, "Bumper", neonShader, new Color(0.2f, 0.6f, 1f), new Color(0.2f, 0.6f, 1f));
        CreateMaterial(matPath, "Flipper", neonShader, new Color(0.8f, 0.2f, 0.2f), new Color(1f, 0.3f, 0.3f));
        CreateMaterial(matPath, "NeonStrip", neonShader, Color.black, new Color(0f, 1f, 1f));
        CreateMaterial(matPath, "Ball", neonShader, new Color(0.8f, 0.8f, 0.8f), new Color(0f, 1f, 1f));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Default materials created in Assets/Materials/");
    }

    private void CreateMaterial(string path, string name, Shader shader, Color baseColor, Color emissionColor)
    {
        Material mat = new Material(shader);
        mat.SetColor("_BaseColor", baseColor);
        mat.SetColor("_EmissionColor", emissionColor);
        mat.SetFloat("_GlowIntensity", 1.5f);
        AssetDatabase.CreateAsset(mat, $"{path}/{name}.mat");
    }
}
#endif
