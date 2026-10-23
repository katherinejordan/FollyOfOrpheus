using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAttack : MonoBehaviour, IBoostable
{
    [SerializeField] InputReader reader;
    [SerializeField] Transform hitpoint;
    [SerializeField] float radius = 1;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float windowLength = 0.25f;
    [SerializeField] float missPenalty = 5;
    [SerializeField] float cooldown = 0.5f;

    private bool attackHit;
    private bool isAttacking;
    private PlayerController controller;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        reader.AttackPressed += OnAttack;
    }

    private void OnDisable()
    {
        reader.AttackPressed -= OnAttack;
    }

    private void OnAttack()
    {
        if (isAttacking) return;
        StartCoroutine(AttackRoutine());
    }

    public void Boost(float boostAmount, AccuracyLevel level)
    {
        attackHit = true;
        controller.Boost(boostAmount, level);
    }

    private IEnumerator AttackRoutine()
    {
        controller.AttackBurst(reader.MoveInput);
        isAttacking = true;
        attackHit = false;

        float timer = 0f;

        while (timer < windowLength)
        {
            Collider[] colliders = Physics.OverlapSphere(hitpoint.position, radius, enemyLayer);
            foreach (Collider col in colliders)
            {
                if (col.TryGetComponent(out IAttackable attackable))
                {
                    Debug.Log(col.name);
                    attackable.Attack(reader.MoveInput, hitpoint.position, this);
                }
            }

            if (attackHit) break;
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (!attackHit) controller.Slow(missPenalty);
        yield return new WaitForSeconds(cooldown);
        isAttacking = false;
    }
}
