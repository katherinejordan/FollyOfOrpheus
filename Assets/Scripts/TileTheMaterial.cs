using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class TileTheMaterial : MonoBehaviour
{
    [SerializeField] private string textureProperty = "_BaseMap";
    [SerializeField] private float size = 1f;

    private Renderer rend;
    private MaterialPropertyBlock mpb;

    private void OnEnable() => MapTiling();
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
        if (rend == null) rend = GetComponent<Renderer>();
        if (rend == null || rend.sharedMaterial == null || size <= 0f) return;
        if (!rend.sharedMaterial.HasProperty(textureProperty)) return;

        mpb ??= new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb);

        Vector2 scale = new Vector2(transform.localScale.x / size, transform.localScale.z / size);
        Vector2 offset = rend.sharedMaterial.GetTextureOffset(textureProperty);
        mpb.SetVector(textureProperty + "_ST", new Vector4(scale.x, scale.y, offset.x, offset.y));

        rend.SetPropertyBlock(mpb);
    }

    private void OnDisable()
    {
        if (rend != null) rend.SetPropertyBlock(null);
    }
}
