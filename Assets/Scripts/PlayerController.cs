using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IBoostable
{
    [Header("Player Prefs")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxSpeed = 150;
    [SerializeField] private float minSpeed = 7;
    [SerializeField] private float mouseSens = 0.1f;
    [SerializeField] private float sideBurst = 5;
    [SerializeField] private float forwardBurst = 5;
    [SerializeField] private float burstVertical = 5;
    [SerializeField] private float burstFade = 20;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float distanceFromGround = 0.15f;

    [Header("Input")]
    [SerializeField] private InputReader reader;
    private Rigidbody rb;
    private Animator animator;
    private Collider col;
    private float totalTurn;
    private float startAngle;
    private Vector3 velocity;
    private Vector3 burstVelocity;

    private void OnEnable()
    {
        totalTurn = transform.eulerAngles.y; //stop from snapping to 0
        startAngle = totalTurn;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        col = GetComponentInChildren<Collider>();

        Cursor.lockState = CursorLockMode.Locked;
        animator.SetBool("IsRunning", true);
    }

    private void Update()
    {
        totalTurn += reader.LookInput.x * mouseSens; //get the turn amount once per frame
        totalTurn = Mathf.Clamp(totalTurn, startAngle - 45f, startAngle + 45f);

        animator.SetFloat("Speed", speed);
    }

    private void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(0f, totalTurn, 0f));
        velocity = transform.forward * speed + burstVelocity;
        float currentFall = rb.linearVelocity.y;
        velocity.y = currentFall;
        rb.linearVelocity = velocity;
        burstVelocity = Vector3.MoveTowards(burstVelocity, Vector3.zero, burstFade * Time.fixedDeltaTime);
    }

    public void Boost(float boostAmount, AccuracyLevel level)
    {
        speed = Mathf.Clamp(speed + boostAmount, minSpeed, maxSpeed);
        Debug.Log($"Speed: {speed}");
        Debug.Log($"Accuracy: {level}");
    }

    public void Slow(float missPenalty)
    {
        speed = Mathf.Clamp(speed - missPenalty, minSpeed, maxSpeed);
        Debug.Log($"Missing speed: {speed}");
    }

    public void AttackBurst(Vector2 input)
    {
        AttackDirection dir = Enemy.AttackDirectionFromVector2(input);

        switch (dir)
        {
            case AttackDirection.DIRECT:
                burstVelocity = transform.forward * forwardBurst;
                animator.CrossFadeInFixedTime("AttackForward", .05f);
                break;
            case AttackDirection.HORIZONTAL:
                burstVelocity = transform.right * input.x * sideBurst;
                string attack = input.x > 0 ? "AttackRight" : "AttackLeft";
                animator.CrossFadeInFixedTime(attack, .05f);
                break;
            case AttackDirection.VERTICAL:
                burstVelocity = Vector3.zero;
                if (input.y > 0 && IsGrounded())
                {
                    rb.AddForce(Vector3.up * input.y * burstVertical, ForceMode.VelocityChange);
                }
                else if (input.y < 0)
                {
                    rb.AddForce(Vector3.up * input.y * burstVertical, ForceMode.VelocityChange);
                }
                attack = input.y > 0 ? "AttackUp" : "AttackDown";
                animator.CrossFadeInFixedTime(attack, .05f);
                break;
        }
    }

    private float GroundRayLength => col.bounds.extents.y + distanceFromGround;

    private bool IsGrounded()
    {
        return Physics.Raycast(col.bounds.center, Vector3.down, GroundRayLength, groundLayer);
    }
}