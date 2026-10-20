using UnityEngine;

public class CutLine : MonoBehaviour
{
    [SerializeField] private Material cutLineMat;
    [SerializeField] private int cutLineMatIndex = 1;
    [SerializeField] private Transform viewer;
    [SerializeField] private Vector3 cutPoint = new(0f, 1f, 0f); // in rest-pose mesh space
    private Vector3 worldSpacePoint;
    [SerializeField] private float width;

    private Renderer rend;
    private MaterialPropertyBlock mpb;

    static readonly int NormalID = Shader.PropertyToID("_CutNormal");
    static readonly int PointID = Shader.PropertyToID("_CutPoint");

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        mpb = new();

        if (viewer == null)
        {
            viewer = Camera.main.transform;
        }
        worldSpacePoint = transform.TransformPoint(cutPoint);
    }

    private void LateUpdate()
    {
        Vector3 normal = Normal();

        rend.GetPropertyBlock(mpb, cutLineMatIndex);
        mpb.SetVector(NormalID, normal);
        mpb.SetVector(PointID, cutPoint);
        rend.SetPropertyBlock(mpb, cutLineMatIndex);
    }

    private Vector3 Normal()
    {
        Vector3 toViewer = (viewer.position - worldSpacePoint).normalized;
        Vector3 n = Vector3.Cross(Vector3.up, toViewer);
        return transform.InverseTransformDirection(n).normalized;
    }

}
