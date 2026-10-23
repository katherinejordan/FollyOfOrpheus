using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerController))]
public class SpeedDeath : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private FinishLine finishLine;

    [Header("Death Rules")]
    [SerializeField] private float speedThreshold = 9f;
    [SerializeField] private float timeAllowedBelow = 2.5f;

    [Header("On Death")]
    [SerializeField] private SoundData deathSound;
    [SerializeField] private bool restartOnDeath = true;
    [SerializeField] private float restartDelay = 2f;

    public event UnityAction PlayerDied = delegate { };

    private float timeBelow;
    private bool isDead;
    private bool finished;
    private Rigidbody rb;
    private Animator animator;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (finishLine) finishLine.PlayerFinished += OnFinished;
    }

    private void OnDisable()
    {
        if (finishLine) finishLine.PlayerFinished -= OnFinished;
    }

    private void OnFinished()
    {
        finished = true;
    }

    private void Update()
    {
        if (isDead || finished || player.IsRamping()) return;

        if (player.Speed < speedThreshold)
            timeBelow += Time.deltaTime;
        else
            timeBelow = 0f;
        if (timeBelow >= timeAllowedBelow)
            Die();
    }

    private void Die()
    {
        isDead = true;
        player.enabled = false;
        Vector3 v = rb.linearVelocity;
        v.x = 0f;
        v.z = 0f;
        rb.linearVelocity = v;

        animator.SetBool("IsRunning", false);
        if (deathSound != null) AudioManager.Instance?.Play(deathSound);
        PlayerDied.Invoke();
        Debug.Log("Player died: too slow");
        if (restartOnDeath) StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSecondsRealtime(restartDelay);
        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}