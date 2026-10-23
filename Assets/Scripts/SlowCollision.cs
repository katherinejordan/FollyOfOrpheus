using UnityEngine;

public class SlowCollision : MonoBehaviour
{
    [SerializeField] float slow = 10;
    void OnCollisionEnter(Collision other)
    {
        
        if (other.transform.TryGetComponent(out PlayerController pc))
        {
            pc.Slow(slow);
        }
    }
}
