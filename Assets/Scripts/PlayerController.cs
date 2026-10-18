using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Prefs")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float mouseSens = 0.1f;

    [Header("Input")]
    [SerializeField] private InputReader reader;
    private Rigidbody rigidbody;
    private Animator animator;
    private float totalTurn;

    private void OnEnable()
    {
        totalTurn = transform.eulerAngles.y; //stop from snapping to 0
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        Cursor.lockState = CursorLockMode.Locked;
        animator.SetBool("IsRunning", true);
    }

    private void Update()
    {
        totalTurn += reader.LookInput.x; //get the turn amount once per frame
    }

    private void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(0f, totalTurn, 0f));
        
    }

}
