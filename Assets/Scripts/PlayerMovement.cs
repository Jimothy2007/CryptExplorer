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
    [SerializeField] private float pushCheckDistance = 0.6f;
    private Vector3 lockedAxis = Vector3.zero;
    private bool isPushing = false;
    private PushableBlock currentBlock;
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

        CheckForBlock(moveDirection);

        if (isPushing && lockedAxis != Vector3.zero)
        {
            float axisAmount = Vector3.Dot(moveDirection, lockedAxis);
            moveDirection = lockedAxis * axisAmount;
        }

        Vector3 targetVelocity = moveDirection * currentMoveSpeed;
        targetVelocity.y = rb.linearVelocity.y;

        if (isPushing && lockedAxis != Vector3.zero)
        {
            float axisVelocity = Vector3.Dot(targetVelocity, lockedAxis);
            targetVelocity = lockedAxis * axisVelocity;
            targetVelocity.y = rb.linearVelocity.y;
        }

        rb.linearVelocity = targetVelocity;

        if (moveDirection != Vector3.zero)
        {
            float targetYaw = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            targetYaw = Mathf.Round(targetYaw / 45f) * 45f;
            playerMesh.rotation = Quaternion.Euler(0, targetYaw, 0);
        }
    }

    private void CheckForBlock(Vector3 moveDirection)
    {
        if (moveDirection.magnitude < 0.1f)
        {
            StopPushing();
            return;
        }

        Vector3 snappedDirection = SnapToAxis(moveDirection);

        RaycastHit hit;
        if (Physics.BoxCast(playerMesh.position, new Vector3(0.4f, 0.5f, 0.4f), playerMesh.forward, out hit, playerMesh.rotation, pushCheckDistance))
        {
            PushableBlock block = hit.collider.GetComponent<PushableBlock>();
            if (block != null)
            {
                if (!isPushing)
                {
                    isPushing = true;
                    currentMoveSpeed = pushingMoveSpeed;
                    lockedAxis = SnapToAxis(moveDirection);
                    currentBlock = block;
                    block.StartPush(lockedAxis, rb);
                }
                return;
            }
        }
        StopPushing();
    }

    private void StopPushing()
    {
        if (isPushing)
        {
            isPushing = false;
            currentMoveSpeed = maxMoveSpeed;
            lockedAxis = Vector3.zero;
            if (currentBlock != null)
            {
                currentBlock.StopPush();
                currentBlock = null;
            }
        }
    }

    private Vector3 SnapToAxis(Vector3 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            return new Vector3(Mathf.Sign(direction.x), 0, 0);
        }
        else
        {
            return new Vector3(0, 0, Mathf.Sign(direction.z));
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
        Gizmos.color = isGrounded ? Color.blue : Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * 1.1f);

        Gizmos.color = isPushing ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(playerMesh.position + playerMesh.forward * pushCheckDistance,
            new Vector3(0.8f, 1f, 0.8f));
    }
}
