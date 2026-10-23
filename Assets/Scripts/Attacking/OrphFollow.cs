using UnityEngine;

public class OrphFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    private bool follow = true;
    private void Update()
    {
        if (follow)
        {
            transform.position = new Vector3(0, target.position.y, target.position.z) + offset;
        }
    }

    public void StopFollow()
    {
        follow = false;
    }
}
