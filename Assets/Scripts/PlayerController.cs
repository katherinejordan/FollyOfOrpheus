using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator), typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float horizontalSpeed = 8f;
    [SerializeField] private float gravity = -20f;
    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        animator.Play("Run");
    }

    private void Update()
    {
        
    }

}
