using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSequence : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private FinishLine finishLine;
    [SerializeField] private float timeToWait; //put the animation clip length here

    private void OnEnable()
    {
        Time.timeScale = 1f;

        if (panel) panel.SetActive(false);

        if (finishLine == null)
        {
            Debug.LogError("EndingSequence: finishLine reference is missing!");
            return;
        }
        finishLine.PlayerFinished += GoToCredits;
    }

    private void OnDisable()
    {
        if (finishLine != null) finishLine.PlayerFinished -= GoToCredits;
    }

    private void GoToCredits()
    {
        StartCoroutine(CreditsCoroutine());
    }

    private IEnumerator CreditsCoroutine()
    {
        if (panel)
        {
            panel.SetActive(true);
            yield return new WaitForSecondsRealtime(timeToWait);
        }
        SceneManager.LoadScene("Credits");
    }
}