using UnityEngine;

public class SlashEnemy : Enemy
{
    private Vector2 slashDir => Vector2FromAttackDirection(attackableDirection);

    protected override float GetAccuracy(Vector3 pos)
    {
        throw new System.NotImplementedException();
    }
}