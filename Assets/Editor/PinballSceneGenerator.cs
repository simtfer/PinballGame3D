#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class PinballSceneGenerator
{
    private const string GameScenePath = "Assets/Scenes/GameScene.unity";
    private const string GeneratedFlipperPrefabPath = "Assets/Prefabs/GeneratedFlipper.prefab";

    [MenuItem("Pinball/Generate Complete Game Scene")]
    public static void GenerateCompleteGameScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        if (System.IO.File.Exists(GameScenePath))
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "Generate Complete Game Scene",
                "This will replace Assets/Scenes/GameScene.unity with a generated scene. Continue?",
                "Generate",
                "Cancel");

            if (!overwrite)
                return;
        }

        EnsureFolder("Assets", "Scenes");

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "GameScene";

        Camera mainCamera = CreateMainCamera();
        CreateDirectionalLight();

        GameManager gameManager = CreateManager<GameManager>("GameManager");
        AudioManager audioManager = CreateManager<AudioManager>("AudioManager");
        PerformanceManager performanceManager = CreateManager<PerformanceManager>("PerformanceManager");
        DouyinPlatformBridge platformBridge = CreateManager<DouyinPlatformBridge>("PlatformBridge");
        TouchInputManager touchInputManager = CreateManager<TouchInputManager>("TouchInputManager");

        PinballTableBuilder tableBuilder = CreateTableBuilder();
        BallController ball = CreateBall();
        PlungerController plunger = CreatePlunger(ball);
        gameManager.ball = ball;
        gameManager.plunger = plunger;
        CameraController cameraController = mainCamera.gameObject.AddComponent<CameraController>();
        cameraController.target = ball.transform;
        cameraController.enabled = false;

        GameSceneSetup gameSceneSetup = CreateGameSetup(
            gameManager,
            audioManager,
            performanceManager,
            platformBridge,
            touchInputManager,
            ball,
            plunger,
            cameraController,
            tableBuilder);

        CyberpunkMaterialGenerator cyberpunkGenerator = gameSceneSetup.gameObject.AddComponent<CyberpunkMaterialGenerator>();
        tableBuilder.cyberpunkGenerator = cyberpunkGenerator;
        gameSceneSetup.cyberpunkGenerator = cyberpunkGenerator;

        CreateBasicEventSystem();

        tableBuilder.BuildTable();
        AlignCameraToTable(mainCamera, tableBuilder);

        Selection.activeGameObject = gameSceneSetup.gameObject;
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, GameScenePath);
        AddSceneToBuildSettings(GameScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Generated complete pinball game scene: {GameScenePath}");
    }

    [MenuItem("Pinball/Open Generated Game Scene")]
    public static void OpenGeneratedGameScene()
    {
        if (!System.IO.File.Exists(GameScenePath))
        {
            EditorUtility.DisplayDialog(
                "Open Generated Game Scene",
                "Assets/Scenes/GameScene.unity does not exist. Run Pinball > Generate Complete Game Scene first.",
                "OK");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
    }

    private static Camera CreateMainCamera()
    {
        GameObject cameraObject = new GameObject("MainCamera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 12f, 0f);
        cameraObject.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.02f, 0.02f, 0.08f);
        camera.fieldOfView = 60f;
        camera.nearClipPlane = 0.3f;
        camera.farClipPlane = 100f;

        AudioListener listener = cameraObject.AddComponent<AudioListener>();
        _ = listener;

        return camera;
    }

    private static void AlignCameraToTable(Camera camera, PinballTableBuilder tableBuilder)
    {
        Transform tableRoot = tableBuilder.transform.Find("TableRoot");
        Quaternion tableRotation = tableRoot != null ? tableRoot.rotation : Quaternion.Euler(-tableBuilder.tableTilt, 0f, 0f);
        Vector3 tableNormal = tableRotation * Vector3.up;
        Vector3 tableForward = tableRotation * Vector3.forward;
        Vector3 target = tableRoot != null ? tableRoot.TransformPoint(Vector3.zero) : tableBuilder.transform.position;

        camera.transform.position = target + tableNormal * 12f + Vector3.back * (tableBuilder.tableHeight * 0.5f);
        camera.transform.rotation = Quaternion.LookRotation(target - camera.transform.position, tableForward);
        camera.orthographic = true;
        camera.orthographicSize = tableBuilder.tableHeight * 0.55f;
    }

    private static void CreateDirectionalLight()
    {
        GameObject lightObject = new GameObject("DirectionalLight");
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(0.8f, 0.8f, 1f);
        light.intensity = 1.5f;
        light.shadows = LightShadows.Soft;

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.2f);
    }

    private static T CreateManager<T>(string objectName) where T : Component
    {
        GameObject gameObject = new GameObject(objectName);
        return gameObject.AddComponent<T>();
    }

    private static PinballTableBuilder CreateTableBuilder()
    {
        GameObject tableObject = new GameObject("PinballTable");
        PinballTableBuilder builder = tableObject.AddComponent<PinballTableBuilder>();
        builder.tableTilt = 6f;
        AssignDefaultMaterials(builder);
        builder.flipperPrefab = CreateFlipperPrefabTemplate();
        return builder;
    }

    private static GameObject CreateFlipperPrefabTemplate()
    {
        EnsureFolder("Assets", "Prefabs");

        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GeneratedFlipperPrefabPath);
        if (existingPrefab != null)
            return existingPrefab;

        GameObject flipper = GameObject.CreatePrimitive(PrimitiveType.Cube);
        flipper.name = "GeneratedFlipper";
        flipper.transform.localScale = new Vector3(1.2f, 0.15f, 0.3f);

        FlipperController controller = flipper.AddComponent<FlipperController>();
        controller.flipperRenderer = flipper.GetComponent<MeshRenderer>();
        controller.hingeJoint = flipper.AddComponent<HingeJoint>();
        flipper.AddComponent<AudioSource>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(flipper, GeneratedFlipperPrefabPath);
        Object.DestroyImmediate(flipper);
        return prefab;
    }

    private static void AssignDefaultMaterials(PinballTableBuilder builder)
    {
        EnsureCyberpunkMaterials();

        builder.tableSurfaceMaterial = LoadMaterial("Assets/Materials/TableSurface.mat");
        builder.wallMaterial = LoadMaterial("Assets/Materials/Wall.mat");
        builder.bumperMaterial = LoadMaterial("Assets/Materials/Bumper.mat");
        builder.flipperMaterial = LoadMaterial("Assets/Materials/Flipper.mat");
        builder.neonMaterial = LoadMaterial("Assets/Materials/NeonStrip.mat");
        builder.safetyPanelMaterial = LoadMaterial("Assets/Materials/SafetyPanel.mat");
    }

    private static void EnsureCyberpunkMaterials()
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

        Color cyberpunkBase = new Color(0.04f, 0.04f, 0.18f, 1f);      // 深蓝紫 #0a0a2e
        Color neonPink = new Color(1f, 0.176f, 0.584f, 1f);             // 霓虹粉 #ff2d95
        Color neonCyan = new Color(0f, 1f, 1f, 1f);                     // 青色 #00ffff
        Color wallPurple = new Color(0.2f, 0.1f, 0.3f, 1f);            // 深紫

        CreateOrUpdateMaterial(matPath, "TableSurface", neonShader, cyberpunkBase, cyberpunkBase * 0.3f, 0.8f);
        CreateOrUpdateMaterial(matPath, "Wall", neonShader, wallPurple, neonCyan, 1.5f);
        CreateOrUpdateMaterial(matPath, "Bumper", neonShader, neonPink, neonPink, 2f);
        CreateOrUpdateMaterial(matPath, "Flipper", neonShader, neonCyan, neonCyan, 2f);
        CreateOrUpdateMaterial(matPath, "NeonStrip", neonShader, cyberpunkBase, neonCyan, 2.5f);
        CreateOrUpdateMaterial(matPath, "Ball", neonShader, new Color(0.8f, 0.9f, 1f, 1f), neonCyan, 1.5f);

        Material safetyPanel = CreateOrUpdateMaterial(matPath, "SafetyPanel", glassShader, new Color(0.1f, 0.15f, 0.3f, 0.2f), neonCyan * 0.5f, 0.5f);
        ConfigureTransparentMaterial(safetyPanel, new Color(0.1f, 0.15f, 0.3f, 0.2f));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static Material CreateOrUpdateMaterial(string path, string name, Shader shader, Color baseColor, Color emissionColor, float glowIntensity)
    {
        string assetPath = $"{path}/{name}.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        if (material == null)
        {
            material = new Material(shader);
            AssetDatabase.CreateAsset(material, assetPath);
        }
        else if (shader != null && material.shader != shader)
        {
            material.shader = shader;
        }

        SetMaterialColor(material, "_BaseColor", baseColor);
        SetMaterialColor(material, "_Color", baseColor);
        SetMaterialColor(material, "_EmissionColor", emissionColor);

        if (material.HasProperty("_GlowIntensity"))
            material.SetFloat("_GlowIntensity", glowIntensity);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", 0.78f);
        if (material.HasProperty("_Metallic"))
            material.SetFloat("_Metallic", 0.05f);

        if (emissionColor.maxColorComponent > 0f)
            material.EnableKeyword("_EMISSION");

        EditorUtility.SetDirty(material);
        return material;
    }

    private static void ConfigureTransparentMaterial(Material material, Color glassColor)
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
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        EditorUtility.SetDirty(material);
    }

    private static void SetMaterialColor(Material material, string propertyName, Color color)
    {
        if (material.HasProperty(propertyName))
            material.SetColor(propertyName, color);
    }

    private static Material LoadMaterial(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Material>(path);
    }

    private static BallController CreateBall()
    {
        GameObject ballObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ballObject.name = "Ball";
        ballObject.transform.position = new Vector3(2.4f, 0.35f, -4f);
        ballObject.transform.localScale = Vector3.one * 0.3f;

        Rigidbody rigidbody = ballObject.GetComponent<Rigidbody>();
        if (rigidbody == null)
            rigidbody = ballObject.AddComponent<Rigidbody>();

        rigidbody.mass = 1f;
        rigidbody.drag = 0.1f;
        rigidbody.angularDrag = 0.05f;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        BallController ball = ballObject.AddComponent<BallController>();
        ball.ballRenderer = ballObject.GetComponent<MeshRenderer>();
        ballObject.AddComponent<AudioSource>();

        TrailRenderer trail = ballObject.AddComponent<TrailRenderer>();
        trail.time = 0.5f;
        trail.startWidth = 0.1f;
        trail.endWidth = 0f;
        ball.trailRenderer = trail;

        return ball;
    }

    private static PlungerController CreatePlunger(BallController ball)
    {
        GameObject plungerObject = new GameObject("Plunger");
        plungerObject.transform.position = new Vector3(2.4f, 0.25f, -4.6f);

        PlungerController plunger = plungerObject.AddComponent<PlungerController>();
        return plunger;
    }

    private static GameSceneSetup CreateGameSetup(
        GameManager gameManager,
        AudioManager audioManager,
        PerformanceManager performanceManager,
        DouyinPlatformBridge platformBridge,
        TouchInputManager touchInputManager,
        BallController ball,
        PlungerController plunger,
        CameraController cameraController,
        PinballTableBuilder tableBuilder)
    {
        GameObject setupObject = new GameObject("GameSetup");
        GameSceneSetup setup = setupObject.AddComponent<GameSceneSetup>();
        setup.gameManager = gameManager;
        setup.audioManager = audioManager;
        setup.performanceManager = performanceManager;
        setup.platformBridge = platformBridge;
        setup.touchInput = touchInputManager;
        setup.ball = ball;
        setup.plunger = plunger;
        setup.gameCamera = cameraController;
        setup.tableBuilder = tableBuilder;
        setup.autoBuildTable = false;
        setup.startWithMenu = false;
        return setup;
    }

    private static void CreateBasicEventSystem()
    {
        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }

    private static void AddSceneToBuildSettings(string scenePath)
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        for (int i = 0; i < scenes.Count; i++)
        {
            if (scenes[i].path == scenePath)
            {
                scenes[i] = new EditorBuildSettingsScene(scenePath, true);
                EditorBuildSettings.scenes = scenes.ToArray();
                return;
            }
        }

        scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static void EnsureFolder(string parentFolder, string childFolder)
    {
        string fullPath = $"{parentFolder}/{childFolder}";
        if (!AssetDatabase.IsValidFolder(fullPath))
            AssetDatabase.CreateFolder(parentFolder, childFolder);
    }
}
#endif
