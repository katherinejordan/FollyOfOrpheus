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
        panel.SetActive(false);
        finishLine.PlayerFinished += GoToCredits;
    }

    private void OnDisable()
    {
        finishLine.PlayerFinished -= GoToCredits;
    }

    private void GoToCredits() 
    {
        StartCoroutine(CreditsCoroutine());
    }

    private IEnumerator CreditsCoroutine()
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(timeToWait);
        SceneManager.LoadScene("Credits");
    }

}
