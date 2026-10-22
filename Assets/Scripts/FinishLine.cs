using UnityEngine;
using UnityEngine.Events;

public class FinishLine : MonoBehaviour
{
    public event UnityAction PlayerFinished = delegate {} ;
    private bool isFinished;

    void OnTriggerEnter(Collider other)
    {
        if (isFinished) return;

        if(other.TryGetComponent(out PlayerController _))
        {
            PlayerFinished.Invoke();
            isFinished = true;
            Debug.Log("Finished!");
        }
    }
}
