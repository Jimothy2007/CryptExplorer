using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform lookAt;

    private void LateUpdate()
    {
        transform.position = target.position;
        transform.LookAt(lookAt);
    }
}
