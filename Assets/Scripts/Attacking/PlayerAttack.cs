using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] InputReader reader;

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
        Debug.Log(reader.MoveInput);
        PlayerController controller = GetComponent<PlayerController>();
        controller.Boost(5, AccuracyLevel.OKAY);
    }
}
