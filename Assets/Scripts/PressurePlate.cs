using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float snapDistance = 0.5f;
    [SerializeField] private float snapSpeed = 5f;
    [SerializeField] private GameObject linkedDoor;

    private Vector3 originalPosition;
    private Vector3 pushedPosition;
    private bool isPushed = false;

    void Start()
    {
        originalPosition = transform.position;
        pushedPosition = originalPosition + new Vector3(0f, -0.2f, 0f);

        if (linkedDoor == null)
        {
            Debug.LogWarning("PressurePlate: No linked door assigned.");
        }
    }

    private void Update()
    {
        Vector3 target = isPushed ? pushedPosition : originalPosition;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * moveSpeed);

        SnapBoxToCenter();

        if (linkedDoor != null && isPushed)
        {
            linkedDoor.GetComponent<DoorMovement>().OpenDoor();
        }
        else if (linkedDoor != null && !isPushed)
        {
            linkedDoor.GetComponent<DoorMovement>().CloseDoor();
        }
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PushableBlock"))
        {
            if (!isPushed)
            {
                collision.transform.SetParent(transform);
                isPushed = true;
            }
        }
    }

    private void OnCollisionExit(UnityEngine.Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PushableBlock"))
        {
            if (isPushed)
            {
                collision.transform.SetParent(null);
                isPushed = false;
            }
        }
    }

    private void SnapBoxToCenter()
    {
        Collider[] nearby = Physics.OverlapBox(transform.position, transform.localScale / 2 + Vector3.one * snapDistance);
        foreach (Collider col in nearby)
        {
            if (col.gameObject.CompareTag("PushableBlock"))
            {
                Rigidbody blockRb = col.GetComponent<Rigidbody>();

                Vector3 targetPos = new Vector3(transform.position.x, col.transform.position.y, transform.position.z);
                float distance = Vector3.Distance(new Vector3(col.transform.position.x, 0f, col.transform.position.z), new Vector3(transform.position.x, 0f, transform.position.z));
                if (distance < snapDistance)
                {
                    if (blockRb != null)
                    {
                        blockRb.linearVelocity = new Vector3(0f, blockRb.linearVelocity.y, 0f);
                    }
                    col.transform.position = Vector3.Lerp(col.transform.position, targetPos, Time.deltaTime * snapSpeed);
                }
                else
                {
                    if (blockRb != null)
                    {
                        blockRb.isKinematic = false;
                    }
                }
            }
        }
    }
}
