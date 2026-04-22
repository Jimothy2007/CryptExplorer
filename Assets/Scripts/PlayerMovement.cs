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
    private float knockbackTimer = 0f;
    private bool hasKnockback => knockbackTimer > 0f;
    private bool inputEnabled = true;
    private Vector3 lockedAxis = Vector3.zero;
    private bool isPushing = false;
    private bool isClimbing = false;
    private LadderScript currentLadder;
    private PushableBlock currentBlock;
    private Rigidbody rb;
    private Vector2 moveInput;
    public bool IsClimbing() => isClimbing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        currentMoveSpeed = maxMoveSpeed;
    }

    void OnMove(InputValue inputValue)
    {
        if (!inputEnabled && !isClimbing) return;
        moveInput = inputValue.Get<Vector2>();
    }

    void OnJump(InputValue inputValue)
    {
        if (!inputEnabled) return;
        if (inputValue.isPressed && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        if (isClimbing)
        {
            HandleClimbing();
            return;
        }

        if (!inputEnabled) return;

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

    private void HandleClimbing()
    {
        if (currentLadder == null) return;

        float verticalInput = moveInput.y;
        Debug.Log("Climbing - moveInput: " + moveInput + " verticalInput: " + verticalInput);

        if (verticalInput > 0f)
        {
            rb.linearVelocity = Vector3.up * currentLadder.GetClimbSpeed();
            Debug.Log("Moving up at speed: " + currentLadder.GetClimbSpeed());

            if (transform.position.y >= currentLadder.GetTopPoint().position.y)
            {
                StopClimbing(true);
                return;
            }
        }
        else if (verticalInput < 0f)
        {
            rb.linearVelocity = Vector3.down * currentLadder.GetDescendSpeed();
            Debug.Log("Moving down at speed: " + currentLadder.GetDescendSpeed());

            if (transform.position.y <= currentLadder.GetSnapPoint().position.y)
            {
                StopClimbing(false);
                return;
            }
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }

        Vector3 lockedPosition = rb.position;
        lockedPosition.x = currentLadder.GetSnapPoint().position.x;
        lockedPosition.z = currentLadder.GetSnapPoint().position.z;
        rb.MovePosition(lockedPosition);
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

    public void StartClimbing(LadderScript ladder)
    {
        isClimbing = true;
        currentLadder = ladder;
        DisableInput();

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation |
                         RigidbodyConstraints.FreezePositionX |
                         RigidbodyConstraints.FreezePositionZ;
        rb.MovePosition(ladder.GetSnapPoint().position);
        playerMesh.rotation = ladder.GetSnapPoint().rotation;
    }

    public void StopClimbing(bool exitFromTop)
    {
        isClimbing = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (exitFromTop && currentLadder != null)
        {
            rb.MovePosition(currentLadder.GetTopPoint().position);
            playerMesh.rotation = currentLadder.GetTopPoint().rotation;
        }

        currentLadder = null;
        EnableInput();
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
        knockbackTimer = 0.2f;
    }

    private bool IsGrounded()
    {
        bool hit = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        isGrounded = hit;
        return isGrounded;
    }

    public void DisableInput()
    {
        inputEnabled = false;
        moveInput = Vector2.zero;
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    public void EnableInput()
    {
        inputEnabled = true;
    }

    public void SnapToPoint(Transform snapPoint)
    {
        DisableInput();

        rb.linearVelocity = Vector3.zero;
        rb.MovePosition(snapPoint.position);

        playerMesh.rotation = snapPoint.rotation;
    }

    public void UnsnapFromPoint()
    {
        EnableInput();
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
