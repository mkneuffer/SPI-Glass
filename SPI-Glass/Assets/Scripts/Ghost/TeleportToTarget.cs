using UnityEngine;

public class TeleportToTarget : MonoBehaviour
{
    [SerializeField] private Transform target; // The target object to teleport to

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
        }
    }
}
