using UnityEngine;

public class LeverHitboxScript : MonoBehaviour
{
    [SerializeField] private LeverScript leverScript;
    [SerializeField] private Transform snapPoint;

    private void Start()
    {
        snapPoint = transform;
    }

    public LeverScript GetLeverScript()
    {
        return leverScript;
    }

    public Transform GetSnapPoint()
    {
        return snapPoint;
    }

    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // transparent green
        Gizmos.matrix = transform.localToWorldMatrix;

        if (col is BoxCollider box)
        {
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = new Color(0f, 1f, 0f, 1f);
            Gizmos.DrawWireCube(box.center, box.size);
        }

        else if (col is SphereCollider sphere)
        {
            Gizmos.DrawSphere(sphere.center, sphere.radius);
            Gizmos.color = new Color(0f, 1f, 0f, 1f);
            Gizmos.DrawWireSphere(sphere.center, sphere.radius);
        }
    }
}
