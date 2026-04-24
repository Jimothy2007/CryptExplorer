using UnityEngine;

public class DoorMovement : MonoBehaviour
{
    [SerializeField] private float openingMoveSpeed = 5f;
    [SerializeField] private float closingMoveSpeed = 10f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(0f, 4f, 0f);
    }

    void Update()
    {
        Vector3 target = isOpen ? openPosition : closedPosition;
        float speed = isOpen ? openingMoveSpeed : closingMoveSpeed;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);
    }

    public void OpenDoor()
    {
        isOpen = true;
    }

    public void CloseDoor()
    {
        isOpen = false;
    }
}
