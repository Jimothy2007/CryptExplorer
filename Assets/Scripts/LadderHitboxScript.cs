using UnityEngine;

public class LadderHitboxScript : MonoBehaviour
{
    [SerializeField] private LadderScript ladderScript;
    [SerializeField] private bool isTopHitbox;

    public LadderScript GetLadderScript() => ladderScript;
    public bool IsTopHitbox() => isTopHitbox;

    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.matrix = transform.localToWorldMatrix;

        if (col is BoxCollider box)
        {
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
            Gizmos.DrawWireCube(box.center, box.size);
        }
    }
}
