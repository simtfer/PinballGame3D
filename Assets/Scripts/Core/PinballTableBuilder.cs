using UnityEngine;

public class PinballTableBuilder : MonoBehaviour
{
    [Header("Table Dimensions")]
    public float tableWidth = 5f;
    public float tableHeight = 10f;
    public float wallHeight = 1f;
    public float wallThickness = 0.2f;
    public float tableTilt = 6f;

    [Header("Materials")]
    public Material tableSurfaceMaterial;
    public Material wallMaterial;
    public Material bumperMaterial;
    public Material flipperMaterial;
    public Material safetyPanelMaterial;
    public Material neonMaterial;

    [Header("Cyberpunk")]
    public CyberpunkMaterialGenerator cyberpunkGenerator;
    public bool useCyberpunkStyle = true;

    [Header("Prefabs")]
    public GameObject bumperPrefab;
    public GameObject targetPrefab;
    public GameObject flipperPrefab;
    public GameObject slingshotPrefab;
    public GameObject spinnerPrefab;
    public GameObject rampPrefab;
    public GameObject rolloverPrefab;

    [Header("Layout")]
    public int bumperCount = 3;
    public float bumperSpacing = 1.2f;
    public Vector3 bumperAreaCenter = new Vector3(0f, 0f, 3f);

    public void BuildTable()
    {
        if (useCyberpunkStyle && cyberpunkGenerator != null)
        {
            cyberpunkGenerator.GenerateMaterials();
            ApplyCyberpunkMaterials();
        }

        ClearExistingTable();
        CreateTableSurface();
        CreateWalls();
        CreateFlippers();
        CreateBumpers();
        CreateSlingshots();
        CreateTargets();
        CreateRamps();
        CreateLaunchLaneReturnDeflector();
        CreateSafetyPanel();
        ApplyTilt();
    }

    public void BuildTableWithoutCyberpunk()
    {
        ClearExistingTable();
        CreateTableSurface();
        CreateWalls();
        CreateFlippers();
        CreateBumpers();
        CreateSlingshots();
        CreateTargets();
        CreateRamps();
        CreateLaunchLaneReturnDeflector();
        CreateSafetyPanel();
        ApplyTilt();
    }

    private void ClearExistingTable()
    {
        Transform tableRoot = transform.Find("TableRoot");
        if (tableRoot != null)
            Destroy(tableRoot.gameObject);

        tableRoot = new GameObject("TableRoot").transform;
        tableRoot.SetParent(transform);
        tableRoot.localPosition = Vector3.zero;
    }

    private void CreateTableSurface()
    {
        Transform root = transform.Find("TableRoot");
        GameObject surface = GameObject.CreatePrimitive(PrimitiveType.Cube);
        surface.name = "TableSurface";
        surface.transform.SetParent(root);
        surface.transform.localPosition = Vector3.zero;
        surface.transform.localScale = new Vector3(tableWidth, 0.1f, tableHeight);

        Renderer renderer = surface.GetComponent<Renderer>();
        if (tableSurfaceMaterial != null)
            renderer.material = tableSurfaceMaterial;

        BoxCollider col = surface.GetComponent<BoxCollider>();
        col.isTrigger = false;
    }

    private void CreateWalls()
    {
        Transform root = transform.Find("TableRoot");
        float topZ = tableHeight / 2f;
        float sideJoinZ = topZ - 1.25f;
        float sideWallCenterZ = (-tableHeight / 2f + sideJoinZ) * 0.5f;
        float sideWallLength = sideJoinZ + tableHeight / 2f;

        CreateWall(root, "LeftWall",
            new Vector3(-tableWidth / 2 - wallThickness / 2, wallHeight / 2, sideWallCenterZ),
            new Vector3(wallThickness, wallHeight, sideWallLength));

        CreateWall(root, "RightWall",
            new Vector3(tableWidth / 2 + wallThickness / 2, wallHeight / 2, sideWallCenterZ),
            new Vector3(wallThickness, wallHeight, sideWallLength));

        CreateCurvedTopWall(root, sideJoinZ);

        CreateWall(root, "LaunchLaneRight",
            new Vector3(tableWidth / 2 - 0.3f, wallHeight / 2, -tableHeight / 4),
            new Vector3(wallThickness, wallHeight, tableHeight / 2));
    }

    private void CreateCurvedTopWall(Transform parent, float sideJoinZ)
    {
        float halfWidth = tableWidth / 2f;
        float topZ = tableHeight / 2f;
        float rise = topZ - sideJoinZ;
        float radius = (halfWidth * halfWidth + rise * rise) / (2f * rise);
        float centerZ = topZ - radius;
        float maxTheta = Mathf.Asin(halfWidth / radius);
        int segments = 12;
        float overlap = 0.03f;

        for (int i = 0; i < segments; i++)
        {
            float t0 = Mathf.Lerp(-maxTheta, maxTheta, i / (float)segments);
            float t1 = Mathf.Lerp(-maxTheta, maxTheta, (i + 1) / (float)segments);

            Vector3 p0 = new Vector3(radius * Mathf.Sin(t0), wallHeight / 2f, centerZ + radius * Mathf.Cos(t0));
            Vector3 p1 = new Vector3(radius * Mathf.Sin(t1), wallHeight / 2f, centerZ + radius * Mathf.Cos(t1));
            Vector3 mid = (p0 + p1) * 0.5f;
            Vector3 delta = p1 - p0;

            CreateWall(parent, $"TopArcWall_{i:00}", mid,
                new Vector3(wallThickness, wallHeight, delta.magnitude + overlap),
                Quaternion.Euler(0f, Mathf.Atan2(delta.x, delta.z) * Mathf.Rad2Deg, 0f));
        }
    }

    private void CreateWall(Transform parent, string name, Vector3 position, Vector3 scale)
    {
        CreateWall(parent, name, position, scale, Quaternion.identity);
    }

    private void CreateWall(Transform parent, string name, Vector3 position, Vector3 scale, Quaternion rotation)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.SetParent(parent);
        wall.transform.localPosition = position;
        wall.transform.localRotation = rotation;
        wall.transform.localScale = scale;

        Renderer renderer = wall.GetComponent<Renderer>();
        if (wallMaterial != null)
            renderer.material = wallMaterial;

        wall.AddComponent<WallController>();
    }

    private void CreateFlippers()
    {
        Transform root = transform.Find("TableRoot");
        float flipperY = 0.15f;
        float flipperZ = -tableHeight / 2 + 1.5f;

        if (flipperPrefab != null)
        {
            Instantiate(flipperPrefab, new Vector3(-1.2f, flipperY, flipperZ),
                Quaternion.Euler(0, 25, 0), root);
            Instantiate(flipperPrefab, new Vector3(1.2f, flipperY, flipperZ),
                Quaternion.Euler(0, -25, 0), root);
        }
    }

    private void CreateBumpers()
    {
        Transform root = transform.Find("TableRoot");
        float bumperY = 0.3f;

        for (int i = 0; i < bumperCount; i++)
        {
            float angle = i * (360f / bumperCount) * Mathf.Deg2Rad;
            float x = bumperAreaCenter.x + Mathf.Cos(angle) * bumperSpacing;
            float z = bumperAreaCenter.z + Mathf.Sin(angle) * bumperSpacing;

            Vector3 pos = new Vector3(x, bumperY, z);

            if (bumperPrefab != null)
            {
                Instantiate(bumperPrefab, pos, Quaternion.identity, root);
            }
            else
            {
                GameObject bumper = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                bumper.name = $"Bumper_{i}";
                bumper.transform.SetParent(root);
                bumper.transform.localPosition = pos;
                bumper.transform.localScale = new Vector3(0.6f, 0.3f, 0.6f);

                Renderer renderer = bumper.GetComponent<Renderer>();
                if (bumperMaterial != null)
                    renderer.material = bumperMaterial;

                bumper.AddComponent<BumperController>();
            }
        }
    }

    private void CreateSlingshots()
    {
        Transform root = transform.Find("TableRoot");
        float slingY = 0.2f;
        float slingZ = -tableHeight / 2 + 2.5f;

        CreateSlingshot(root, "LeftSlingshot", new Vector3(-1.8f, slingY, slingZ), 30f);
        CreateSlingshot(root, "RightSlingshot", new Vector3(1.8f, slingY, slingZ), -30f);
    }

    private void CreateSlingshot(Transform parent, string name, Vector3 position, float angle)
    {
        if (slingshotPrefab != null)
        {
            Instantiate(slingshotPrefab, position, Quaternion.Euler(0, angle, 0), parent);
        }
        else
        {
            GameObject slingshot = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slingshot.name = name;
            slingshot.transform.SetParent(parent);
            slingshot.transform.localPosition = position;
            slingshot.transform.localRotation = Quaternion.Euler(0, angle, 0);
            slingshot.transform.localScale = new Vector3(0.8f, 0.4f, 0.1f);

            slingshot.AddComponent<SlingshotController>();
        }
    }

    private void CreateTargets()
    {
        Transform root = transform.Find("TableRoot");

        for (int i = 0; i < 5; i++)
        {
            float x = -2f + i * 1f;
            float z = tableHeight / 2 - 1.5f;

            if (targetPrefab != null)
            {
                Instantiate(targetPrefab, new Vector3(x, 0.25f, z), Quaternion.identity, root);
            }
        }
    }

    private void CreateRamps()
    {
        Transform root = transform.Find("TableRoot");

        if (rampPrefab != null)
        {
            Instantiate(rampPrefab, new Vector3(-1.5f, 0.1f, 1f), Quaternion.Euler(-15, 0, 0), root);
            Instantiate(rampPrefab, new Vector3(1.5f, 0.1f, 0f), Quaternion.Euler(-15, 180, 0), root);
        }
    }

    private void CreateLaunchLaneReturnDeflector()
    {
        Transform root = transform.Find("TableRoot");
        float laneCenterX = tableWidth / 2f - 0.1f;
        float laneExitZ = 0.1f;

        GameObject deflector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        deflector.name = "LaunchLaneReturnDeflector";
        deflector.transform.SetParent(root);
        deflector.transform.localPosition = new Vector3(laneCenterX - 0.12f, wallHeight / 2f, laneExitZ);
        deflector.transform.localRotation = Quaternion.Euler(0f, 35f, 0f);
        deflector.transform.localScale = new Vector3(0.12f, wallHeight, 0.85f);

        Renderer renderer = deflector.GetComponent<Renderer>();
        if (neonMaterial != null)
            renderer.material = neonMaterial;
        else if (wallMaterial != null)
            renderer.material = wallMaterial;

        BoxCollider deflectorCollider = deflector.GetComponent<BoxCollider>();
        deflectorCollider.isTrigger = true;

        LaunchLaneReturnDeflectorController deflectorController = deflector.AddComponent<LaunchLaneReturnDeflectorController>();
        deflectorController.launchDirection = Vector3.forward;
        deflectorController.returnRedirectDirection = new Vector3(-1f, 0f, 0.6f);
        deflectorController.deflectorRenderer = deflector.GetComponent<MeshRenderer>();
    }

    private void CreateSafetyPanel()
    {
        Transform root = transform.Find("TableRoot");

        GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        panel.name = "SafetyPanel";
        panel.transform.SetParent(root);
        panel.transform.localPosition = new Vector3(0f, wallHeight + 0.25f, 0f);
        panel.transform.localScale = new Vector3(tableWidth + wallThickness * 2f, 0.08f, tableHeight + wallThickness * 2f);

        Renderer renderer = panel.GetComponent<Renderer>();
        if (safetyPanelMaterial != null)
            renderer.material = safetyPanelMaterial;

        BoxCollider panelCollider = panel.GetComponent<BoxCollider>();
        panelCollider.isTrigger = false;
    }

    private void ApplyTilt()
    {
        Transform root = transform.Find("TableRoot");
        if (root != null)
            root.localRotation = Quaternion.Euler(-tableTilt, 0, 0);
    }

    private void ApplyCyberpunkMaterials()
    {
        tableSurfaceMaterial = cyberpunkGenerator.GetTableSurfaceMaterial();
        wallMaterial = cyberpunkGenerator.GetWallMaterial();
        bumperMaterial = cyberpunkGenerator.GetBumperMaterial();
        flipperMaterial = cyberpunkGenerator.GetFlipperMaterial();
        safetyPanelMaterial = cyberpunkGenerator.GetTransparentMaterial();
        neonMaterial = cyberpunkGenerator.GetNeonMaterial();
    }
}
