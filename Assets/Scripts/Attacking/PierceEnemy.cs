using UnityEngine;

public class PierceEnemy : Enemy
{
    private Vector3 piercePoint;

    /// <summary>
    /// Returns the accuracy from 0-100 based on how close to center the position is
    /// </summary>
    /// <param name="pos">Where it gets attacked</param>
    /// <returns></returns>
    protected override float GetAccuracy(Vector3 pos)
    {
        float dist = Vector3.Distance(pos, piercePoint);
        float diff = maxDistance - dist;

        if (diff <= 0) return 0;

        return diff / maxDistance;
    }
}