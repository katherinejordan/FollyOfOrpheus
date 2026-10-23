using UnityEngine;

public class SlowZone : MonoBehaviour
{
    [SerializeField] float slowPerSecond = 10;
    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out PlayerController pc))
        {
            pc.Slow(slowPerSecond * Time.fixedDeltaTime);
        }
    }
}
