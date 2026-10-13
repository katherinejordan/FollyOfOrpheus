using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator), typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Prefs")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float verticalSpeed= 0f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float mouseSens = 0.1f;

    [Header("Input")]
    [SerializeField] private InputReader reader;
    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    private void OnEnable()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // reader.EnablePlayerActions();

        Cursor.lockState = CursorLockMode.Locked;
        animator.SetBool("IsRunning", true);
    }

    private void OnDisable()
    {
        // reader.DisablePlayerActions();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, reader.LookInput.x);

        if (controller.isGrounded && verticalSpeed < 0) //am i on the ground?
        {
            verticalSpeed = -1;
        }

        Vector3 horizontalSpeed = transform.forward * speed;
        verticalSpeed += gravity * Time.deltaTime;

        velocity = horizontalSpeed + Vector3.up * verticalSpeed;
        controller.Move(velocity * Time.deltaTime);
    }

}
