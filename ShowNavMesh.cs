using UnityEngine;
using UnityEngine.AI;

public class ShowNavMesh : MonoBehaviour
{
    public static ShowNavMesh instance;
    [Tooltip("Color of the NavMesh surface visualization")]
    public Color surfaceColor = new Color(0, 1, 0, 0.3f); // Semi-transparent green

    [Tooltip("Minimum area to consider when generating the mesh")]
    public float minimumAreaMask = NavMesh.AllAreas;

    [Tooltip("Height offset for the visualization mesh")]
    public float heightOffset = 0.01f;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    void Awake() {
        instance = this;
    }

    void Start()
    {
        // Add MeshFilter and MeshRenderer components if they don't exist
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        // Create the NavMesh visualization
        GenerateNavMeshVisualization();
    }

    void GenerateNavMeshVisualization()
    {
        // Create a new Mesh
        Mesh navMeshMesh = new Mesh();
        navMeshMesh.name = "NavMesh Surface Visualization";

        // Get NavMesh data
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        // Adjust vertices to add a small height offset
        Vector3[] vertices = new Vector3[triangulation.vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = triangulation.vertices[i] + Vector3.up * heightOffset;
        }

        // Set mesh data
        navMeshMesh.vertices = vertices;
        navMeshMesh.triangles = triangulation.indices;

        // Recalculate mesh normals and bounds
        navMeshMesh.RecalculateNormals();
        navMeshMesh.RecalculateBounds();

        // Assign the mesh to the MeshFilter
        meshFilter.mesh = navMeshMesh;

        // Create a material for visualization
        Material surfaceMaterial = new Material(Shader.Find("Standard"));
        surfaceMaterial.color = surfaceColor;
        surfaceMaterial.SetFloat("_Mode", 2); // Fade rendering mode
        surfaceMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        surfaceMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        surfaceMaterial.SetInt("_ZWrite", 0);
        surfaceMaterial.DisableKeyword("_ALPHATEST_ON");
        surfaceMaterial.EnableKeyword("_ALPHABLEND_ON");
        surfaceMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        surfaceMaterial.renderQueue = 3000;

        // Assign material to MeshRenderer
        meshRenderer.material = surfaceMaterial;
    }

#if UNITY_EDITOR
    // Optional: Regenerate visualization if NavMesh changes in the editor
    void OnValidate()
    {
        if (Application.isPlaying)
        {
            GenerateNavMeshVisualization();
        }
    }
#endif
}