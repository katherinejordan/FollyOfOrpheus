using System.Drawing;
using UnityEngine;

public class SlashEnemy : Enemy
{
    private Vector3 slashPoint;

    private Vector2 slashDir => Vector2FromAttackDirection(attackableDirection);

    protected override float GetAccuracy(Vector3 pos)
    {
        float dist = DistanceToLine(pos);
        float diff = maxDistance - dist;

        if (diff <= 0) return 0;

        return diff / maxDistance;
    }
    
    // Should return distance of strike point to nearest point on the slash line
    private float DistanceToLine(Vector3 pos)
    {
        return 0;
    }
}