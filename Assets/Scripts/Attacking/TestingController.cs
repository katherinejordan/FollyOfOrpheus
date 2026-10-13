using Unity.VisualScripting;
using UnityEngine;

public class TestingController : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    [SerializeField] private Enemy enemy;
    [SerializeField] private Transform hitLocation;
    [SerializeField] private FalsePlayer falsePlayer;

    private void OnEnable()
    {
        inputReader.JumpPressed += OnSpace;
    }

    private void OnDisable()
    {
        inputReader.JumpPressed -= OnSpace;
    }

    private void OnSpace()
    {
        if (enemy == null) return;
        Debug.Log("Attacking from false player");
        enemy.Attack(Vector2.zero, hitLocation.position, falsePlayer);
    }
}
