using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private InputReader reader;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject endingPanel;
    private bool isPaused;

    private void OnEnable()
    {
        panel.SetActive(false);
        reader.PausePressed += TogglePause;
    }

    private void OnDisable()
    {
        reader.PausePressed -= TogglePause;
    }

    private void TogglePause()
    {
        if (endingPanel != null && endingPanel.activeSelf) return; //stop during ending panel
        SetPaused(!isPaused);
    }

    public void Resume()
    {
        SetPaused(false);
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;
        panel.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }
}