using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float maxMoveSpeed = 5f;
    [SerializeField] private float pushingMoveSpeed = 2f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float knockbackResistance = 1f;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private Transform playerMesh;
    [SerializeField] private Transform cameraBoom;

    [SerializeField] private float currentMoveSpeed;
    private Rigidbody rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        currentMoveSpeed = maxMoveSpeed;
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
        float boomYaw = cameraBoom.eulerAngles.y;
        Quaternion boomRotation = Quaternion.Euler(0, boomYaw, 0);

        Vector3 boomForward = boomRotation * Vector3.forward;
        Vector3 boomRight = boomRotation * Vector3.right;

        Vector3 moveDirection = (boomForward * moveInput.y + boomRight * moveInput.x);

        Vector3 targetVelocity = moveDirection * currentMoveSpeed;
        targetVelocity.y = rb.linearVelocity.y;

        rb.linearVelocity = targetVelocity;

        if (moveDirection != Vector3.zero)
        {
            float targetYaw = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetYaw, 0);
            playerMesh.rotation = Quaternion.Slerp(playerMesh.rotation, targetRotation, Time.fixedDeltaTime * 10f);
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

    public void changeToPushSpeed()
    {
        currentMoveSpeed = pushingMoveSpeed;
    }

    public void changeToNormalSpeed()
    {
        currentMoveSpeed = maxMoveSpeed;
    }
}
