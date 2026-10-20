using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Bakes each vertex's bind-pose position into UV channel 1 (TEXCOORD1) on import,
// so shaders get surface-stable coordinates that follow the mesh while it animates.
public class RestPositionUVPostprocessor : AssetPostprocessor
{
    public const int Channel = 1;

    static readonly string[] modelPaths =
    {
        "Assets/Art/TEMP_characterAndAnims/Models/HumanF_Model.fbx",
    };

    public override uint GetVersion() => 1;

    private void OnPostprocessModel(GameObject root)
    {
        if (System.Array.IndexOf(modelPaths, assetPath) < 0) return;

        var done = new HashSet<Mesh>();
        foreach (var smr in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            Bake(smr.sharedMesh, done);
        foreach (var mf in root.GetComponentsInChildren<MeshFilter>(true))
            Bake(mf.sharedMesh, done);
    }

    private static void Bake(Mesh mesh, HashSet<Mesh> done)
    {
        if (mesh == null || !done.Add(mesh)) return;

        var positions = new List<Vector3>();
        mesh.GetVertices(positions);
        mesh.SetUVs(Channel, positions);
    }
}
