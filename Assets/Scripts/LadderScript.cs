using UnityEngine;

public class LadderScript : MonoBehaviour
{
    [SerializeField] private Transform snapPoint;
    [SerializeField] private Transform topPoint;
    [SerializeField] private float climbSpeed = 2f;
    [SerializeField] private float descendSpeed = 3f;

    public Transform GetSnapPoint() => snapPoint;
    public Transform GetTopPoint() => topPoint;
    public float GetClimbSpeed() => climbSpeed;
    public float GetDescendSpeed() => descendSpeed;

    private void OnDrawGizmos()
    {
        if (snapPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(snapPoint.position, 0.15f);
            Gizmos.DrawRay(snapPoint.position, snapPoint.forward * 0.5f);
        }

        if (topPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(topPoint.position, 0.15f);
            Gizmos.DrawRay(topPoint.position, topPoint.forward * 0.5f);
        }
    }
}
