using UnityEngine;

public class SlowZone : MonoBehaviour
{
    [SerializeField] float enterPentalty = 10;
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController pc))
        {
            pc.Slow(enterPentalty);
        }
    }
}
