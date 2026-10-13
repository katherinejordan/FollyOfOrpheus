using Unity.VisualScripting;
using UnityEngine;

public interface IAttackable
{
    /// <summary>
    /// Attacks an enemy with attack directions and a position of attack that
    /// is used to determine accuracy.
    /// </summary>
    /// <param name="dir">The Direction of the attack</param>
    /// <param name="pos">Where the Attack Lands</param>
    void Attack(Vector2 dir, Vector3 pos, IBoostable boostable);
}

public enum AttackDirection
{
    DIRECT, // no input
    HORIZONTAL, // A or D
    VERTICAL, // W or S
    DIAGONAL_UP_RIGHT, // WD, SA
    DIAGONAL_DOWN_RIGHT, // AW, DS
}