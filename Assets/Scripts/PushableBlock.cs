using UnityEngine;

public class PushableBlock : MonoBehaviour
{
    private Rigidbody rb;
    private bool isBeingPushed = false;
    private Vector3 pushDirection;
    private Rigidbody currentPlayerRb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void StartPush(Vector3 direction, Rigidbody playerRb)
    {
        pushDirection = direction;
        currentPlayerRb = playerRb;
        isBeingPushed = true;
    }

    public void StopPush()
    {
        isBeingPushed = false;
        currentPlayerRb = null;
        rb.linearVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (isBeingPushed && currentPlayerRb != null)
        {
            float playerSpeed = Vector3.Dot(currentPlayerRb.linearVelocity, pushDirection.normalized);
            Vector3 targetVelocity = pushDirection.normalized * playerSpeed;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, 0.5f);
        }
        else
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, 0.5f);
        }
    }
}
