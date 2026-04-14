using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float knockbackResistance = 1f;
    [SerializeField] private bool isGrounded = false;

    private Transform cameraTransform;
    private Rigidbody rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
    }

    void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

    void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        float cameraYaw = cameraTransform.eulerAngles.y;
        Quaternion cameraRotation = Quaternion.Euler(0, cameraYaw, 0);

        Vector3 cameraForward = cameraRotation * Vector3.forward;
        Vector3 cameraRight = cameraRotation * Vector3.right;

        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x);

        Vector3 targetVelocity = moveDirection * moveSpeed;
        targetVelocity.y = rb.linearVelocity.y;

        rb.linearVelocity = targetVelocity;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        rb.AddForce(direction.normalized * force / knockbackResistance, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        bool hit = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        isGrounded = hit;
        return isGrounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * 1.1f);
    }
}
