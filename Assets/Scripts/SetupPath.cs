using System;
using System.Linq;
using Unity.VisualScripting;
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
    [SerializeField] private GameObject slowWallPrefab;
    [SerializeField] private float slowWallLength;

    [SerializeField] private Transform rockParent;
    [SerializeField] private Transform finish;

    [Serializable]
    private struct EnemyConfig
    {
        public float weight; // spawnchance
        public GameObject prefab;
        public bool randX;
        public bool randY;
        public Vector2 xRange;
        public Vector2 yRange;
    }
    [Header("Enemy Stuff")]
    [SerializeField] private EnemyConfig[] enemies;
    [SerializeField] private Vector2 enemySpacing;
    [SerializeField] private float enemyStartDistance;

    [ContextMenu("Place")]
    private void Place()
    {
        Rid();
        Bounds bounds = GetComponent<MeshFilter>().sharedMesh.bounds;

        Vector3 localStart = new Vector3(bounds.center.x, bounds.max.y, bounds.min.z);
        Vector3 localEnd = new Vector3(bounds.center.x, bounds.max.y, bounds.max.z);
        
        transform.position += startPosition - transform.TransformPoint(localStart);

        Vector3 start = transform.TransformPoint(localStart);
        Vector3 end =  transform.TransformPoint(localEnd);
        finish.transform.position = end;

        float length = Vector3.Distance(start, end);
        Vector3 along = (end - start).normalized;
        float halfWidth = bounds.extents.x * transform.lossyScale.x;

        TileSlowWalls(start, along, length);

        for (float d = 0f; d <= length; d += stepSize)
        {
            Vector3 point = start + along * d;
            SpawnRock(point, along, -1f, halfWidth);
            SpawnRock(point, along, 1f, halfWidth);
        }

        PlaceEnemies(start, along, length);
    }

    private void PlaceEnemies(Vector3 start, Vector3 along, float length)
    {
        if (enemies.Length == 0)
            return;

        if (Mathf.Min(enemySpacing.x, enemySpacing.y) <= 0f)
        {
            return;
        }

        for (float d = enemyStartDistance; d <= length; d += rand(enemySpacing))
        {
            EnemyConfig enemy = PickEnemy();
            float x = enemy.randX ? rand(enemy.xRange) : 0f;
            float y = enemy.randY ? rand(enemy.yRange) : 0f;

            Vector3 position = start
                + along * d
                + transform.right * x
                + transform.up * y;

            Instantiate(enemy.prefab, position, transform.rotation, rockParent);
        }
    }

    private EnemyConfig PickEnemy()
    {
        float total = enemies.Sum(e => e.weight);

        float roll = UnityEngine.Random.Range(0f, total);
        foreach (EnemyConfig enemy in enemies)
        {
            roll -= enemy.weight;
            if (roll < 0f)
                return enemy;
        }

        return enemies[enemies.Length - 1];
    }

    private void TileSlowWalls(Vector3 start, Vector3 along, float length)
    {
        int count = Mathf.CeilToInt(length / slowWallLength);

        for (int i = 0; i < count; i++)
        {
            Vector3 position = start + along * (slowWallLength * (i + 0.5f));
            Instantiate(slowWallPrefab, position, transform.rotation, rockParent);
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

        Quaternion rotation = transform.localRotation * randomRot;
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
