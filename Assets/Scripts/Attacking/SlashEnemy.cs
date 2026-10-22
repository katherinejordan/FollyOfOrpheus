using UnityEngine;

public class SlashEnemy : Enemy
{
    [SerializeField] private Transform slashTransform;
    private Vector3 slashPoint => slashTransform.position;
    private Vector2 slashDir => Vector2FromAttackDirection(attackableDirection);
    private Vector3 SlashDir3D => (transform.right * slashDir.x + transform.up * slashDir.y).normalized;

    protected override float GetAccuracy(Vector3 pos)
    {
        float dist = DistanceToLine(pos);
        float diff = maxDistance - dist;

        if (diff <= 0) return 0;

        return diff / maxDistance;
    }
    
    private float DistanceToLine(Vector3 pos)
    {
        Vector3 dir = SlashDir3D;
        Vector3 offset = pos - slashPoint;
        float along = Vector3.Dot(offset, dir);
        Vector3 closest = slashPoint + dir * along;
        return Vector3.Distance(pos, closest);
    }
}