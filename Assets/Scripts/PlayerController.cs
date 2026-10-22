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
    [SerializeField] private float burstSize = 5;
    [SerializeField] private float burstFade = 20;

    [Header("Input")]
    [SerializeField] private InputReader reader;
    private Rigidbody rb;
    private Animator animator;
    private float totalTurn;
    private float startAngle;
    private Vector3 velocity;
    private float currentBurst;

    private void OnEnable()
    {
        totalTurn = transform.eulerAngles.y; //stop from snapping to 0
        startAngle = totalTurn;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        animator.SetBool("IsRunning", true);
    }

    private void Update()
    {
        totalTurn += reader.LookInput.x * mouseSens; //get the turn amount once per frame
        totalTurn = Mathf.Clamp(totalTurn, startAngle - 45f, startAngle + 45f);
    }

    private void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(0f, totalTurn, 0f));
        
        velocity = transform.forward * (speed + currentBurst);
        float currentFall = rb.linearVelocity.y;
        velocity.y = currentFall;
        rb.linearVelocity = velocity;
        currentBurst = Mathf.MoveTowards(currentBurst, 0, burstFade * Time.fixedDeltaTime);
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

    public void AttackBurst()
    {
        currentBurst = burstSize;
    }
}
