using UnityEngine;
using UnityEngine.Events;

public class BeforeFinish : MonoBehaviour
{

    public OrphFollow follow;
    private bool isFinished;

    void OnTriggerEnter(Collider other)
    {
        if (isFinished) return;

        if(other.TryGetComponent(out PlayerController _))
        {
            follow.StopFollow();
            isFinished = true;
            Debug.Log("Finished!");
        }
    }
}
