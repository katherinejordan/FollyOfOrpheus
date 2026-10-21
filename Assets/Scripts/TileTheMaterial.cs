using UnityEngine;

[ExecuteAlways]
public class TileTheMaterial : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private float size = 1f;

    private void OnValidate() => MapTiling();

    private void Update()
    {
        if (Application.isPlaying) return;
        if (!transform.hasChanged) return;

        transform.hasChanged = false;
        MapTiling();
    }

    private void MapTiling()
    {
        if (mat == null) mat = GetComponent<Renderer>().sharedMaterial;
        if (mat == null || size <= 0f) return;

        Vector3 worldSize = GetComponent<Renderer>().bounds.size;
        mat.mainTextureScale = new Vector2(worldSize.x / size, worldSize.z / size);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(mat);
#endif
    }
}