using System;
using UnityEngine;

public class SetupPath : MonoBehaviour
{
    [SerializeField] private GameObject[] rockPrefabs;
    [SerializeField] private float stepSize;
    [SerializeField] private Vector2 stepRange;
    [SerializeField] private Vector2 edgeRange;
    [SerializeField] private Vector2 sizeRange;
    [SerializeField] private Vector2 ySizeRange;
    [SerializeField] private Vector2 tiltRange;
    [SerializeField] private Vector2 downRange;
    [SerializeField] private Vector3 startPosition;

    [SerializeField] private Transform rockParent;
    [SerializeField] private Transform finish;

    [ContextMenu("Place Rocks")]
    private void Place()
    {
        Rid();
        Bounds bounds = GetComponent<MeshFilter>().sharedMesh.bounds;

        Vector3 localStart = new Vector3(bounds.center.x, bounds.max.y, bounds.min.z);
        Vector3 localEnd = new Vector3(bounds.center.x, bounds.max.y, bounds.max.z);
        finish.transform.position = localEnd;
        transform.position += startPosition - transform.TransformPoint(localStart);

        Vector3 start = transform.TransformPoint(localStart);
        Vector3 end =  transform.TransformPoint(localEnd);

        float length = Vector3.Distance(start, end);
        Vector3 along = (end - start).normalized;
        float halfWidth = bounds.extents.x * transform.lossyScale.x;

        for (float d = 0f; d <= length; d += stepSize)
        {
            Vector3 point = start + along * d;
            SpawnRock(point, along, -1f, halfWidth);
            SpawnRock(point, along, 1f, halfWidth);
        }
    }

    private void SpawnRock(Vector3 centerPoint, Vector3 along, float side, float halfWidth)
    {
        float sideDistance = halfWidth + rand(edgeRange);
        float jitter = rand(stepRange);

        Vector3 position = centerPoint
            + transform.right * (side * sideDistance)
            + along * jitter
            - transform.up * rand(downRange);

        Quaternion randomRot = Quaternion.Euler(
            rand(tiltRange),
            UnityEngine.Random.Range(0f, 360f),
            rand(tiltRange));

        Quaternion rotation = randomRot;
        GameObject prefab = rockPrefabs[UnityEngine.Random.Range(0, rockPrefabs.Length)];
        GameObject rock = Instantiate(prefab, position, rotation, rockParent);

        float randXZ = rand(sizeRange);
        float randY = rand(ySizeRange);

        rock.transform.localScale = new Vector3(randXZ, randY, randXZ);
    }

    [ContextMenu("Get rid of rocks")]
    private void Rid()
    {
        for (int i = rockParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(rockParent.GetChild(i).gameObject);
    }

    private float rand(Vector2 range)
    {
        return UnityEngine.Random.Range(range.x, range.y);
    }
}
