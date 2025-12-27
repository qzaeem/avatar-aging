using UnityEngine;

public class RuntimeAvatarAging : MonoBehaviour
{
    [Header("Target Avatar (Avatar B)")]
    public SkinnedMeshRenderer targetSMR;

    [Header("Canonical Aging Mesh")]
    public Mesh canonicalMesh; // Mesh that has only Age_Old blendshape

    [Range(0f, 100f)]
    public float ageValue = 0f; // 0 = young, 100 = old

    private Vector3[] baseVertices;
    private Vector3[] deltaVertices;
    private Mesh workingMesh;

    private int ageBlendIndex;

    void Start()
    {
        // Duplicate target mesh so we can modify it at runtime
        workingMesh = Instantiate(targetSMR.sharedMesh);
        targetSMR.sharedMesh = workingMesh;

        baseVertices = workingMesh.vertices;

        // Find Age_Old blendshape in canonical mesh
        ageBlendIndex = canonicalMesh.GetBlendShapeIndex("Age_Old");
        if (ageBlendIndex == -1)
        {
            Debug.LogError("Canonical mesh does not have Age_Old blendshape!");
            return;
        }

        // Get delta vertices for Age_Old
        deltaVertices = new Vector3[canonicalMesh.vertexCount];
        Vector3[] dummy = new Vector3[canonicalMesh.vertexCount];
        canonicalMesh.GetBlendShapeFrameVertices(ageBlendIndex, 0, deltaVertices, dummy, dummy);
    }

    void Update()
    {
        ApplyAging(ageValue / 100f); // convert 0-100 to 0-1
    }

    void ApplyAging(float weight)
    {
        Vector3[] newVertices = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
        {
            // Apply weighted delta to target avatar's original vertices
            newVertices[i] = baseVertices[i] + deltaVertices[i] * weight;
        }
        workingMesh.vertices = newVertices;
        workingMesh.RecalculateBounds();
        // Optionally recalc normals for better lighting
        workingMesh.RecalculateNormals();
    }
}
