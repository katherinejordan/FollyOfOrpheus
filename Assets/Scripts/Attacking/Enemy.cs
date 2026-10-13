using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public abstract class Enemy : MonoBehaviour, IAttackable
{
    [SerializeField] protected GameObject attackIndicator;
    [SerializeField] protected AttackDirection attackableDirection;

    [Header("Speed Gain")]
    [SerializeField] protected float baseSpeedGain;
    [SerializeField] protected float maxDistance;
    [SerializeField] protected float perfectThreshold;

    private IBoostable player;
    public void Attack(Vector2 dir, Vector3 pos, IBoostable boostable)
    {
        AttackDirection aDir = AttackDirectionFromVector2(dir);
        if (aDir == attackableDirection)
        {
            AttackInternal(aDir, pos);
        }
    }

    protected virtual void AttackInternal(AttackDirection dir, Vector3 pos)
    {
        float accuracy = GetAccuracy(pos);
        if (accuracy == 0) return;

        Boost(accuracy);
        Die();
    }

    protected abstract float GetAccuracy(Vector3 pos);
    protected void Boost(float accuracy)
    {
        if (player == null) return;
        if (accuracy >= perfectThreshold) player.Boost(baseSpeedGain, AccuracyLevel.PERFECT);
        else player.Boost(accuracy * baseSpeedGain, AccuracyLevel.OKAY);
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    public static AttackDirection AttackDirectionFromVector2(Vector2 vector)
    {
        float x = vector.x;
        float y = vector.y;

        if ((x > 0 && y > 0) || (x < 0 && y < 0)) return AttackDirection.DIAGONAL_UP_RIGHT;
        if ((x > 0 && y < 0) || (x < 0 && y > 0)) return AttackDirection.DIAGONAL_DOWN_RIGHT;
        if (x != 0 && y == 0) return AttackDirection.HORIZONTAL;
        if (x == 0 && y != 0) return AttackDirection.VERTICAL;
        return AttackDirection.DIRECT;
    }

    public static Vector2 Vector2FromAttackDirection(AttackDirection direction)
    {
        switch (direction)
        {
            case AttackDirection.HORIZONTAL:
                return Vector2.right;
            case AttackDirection.VERTICAL:
                return Vector2.up;
            case AttackDirection.DIAGONAL_UP_RIGHT:
                return Vector2.right + Vector2.up;
            case AttackDirection.DIAGONAL_DOWN_RIGHT:
                return Vector2.right + Vector2.down;
            default:
                return Vector2.zero;
        }
    }
}