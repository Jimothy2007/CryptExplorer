using UnityEngine;

public class PushableBlock : MonoBehaviour
{
    [SerializeField] private float pushForce = 5f;

    private Rigidbody rb;
    private bool isBeingPushed = false;
    private Vector3 pushDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        if (isBeingPushed)
        {
            rb.linearVelocity = pushDirection.normalized * pushForce;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 dir = transform.position - collision.gameObject.transform.position;
            pushDirection = SnapToAxis(dir);
            isBeingPushed = true;
            collision.gameObject.GetComponent<PlayerMovement>().changeToPushSpeed();

            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                pushForce = playerRb.linearVelocity.magnitude;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().changeToNormalSpeed();
            isBeingPushed = false;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private Vector3 SnapToAxis(Vector3 direction)
    {
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.z))
        {
            return new Vector3(direction.x, 0f, 0f);
        }
        else
        {
            return new Vector3(0f, 0f, direction.z);
        }
    }
}
