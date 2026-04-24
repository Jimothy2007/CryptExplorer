using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilitiesScript : MonoBehaviour
{
    private LeverScript currentLever;
    private Transform currentSnapPoint;
    private LadderScript currentLadderInRange;
    private bool ladderCooldown = false;
    private bool mountingFromTop = false;

    void OnInteract(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            Debug.Log("Interact button pressed");
            if (currentLever != null && currentSnapPoint != null)
            {
                GetComponent<PlayerMovement>().SnapToPoint(currentSnapPoint);
                currentLever.FlipLever();
                GetComponent<PlayerMovement>().UnsnapFromPoint();
                Debug.Log("Interacted with lever: " + currentLever.gameObject.name);
                currentLever = null;
            }

            PlayerMovement playerMovement = GetComponent<PlayerMovement>();

            if (currentLadderInRange != null && !playerMovement.IsClimbing())
            {
                if (mountingFromTop)
                {
                    playerMovement.StartClimbingFromTop(currentLadderInRange);
                }
                else
                {
                    playerMovement.StartClimbing(currentLadderInRange);
                }
            }
            else if (playerMovement.IsClimbing())
            {
                playerMovement.StopClimbing(false);
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Lever"))
        {
            LeverHitboxScript hitboxScript = collider.GetComponent<LeverHitboxScript>();
            if (hitboxScript != null)
            {
                currentLever = hitboxScript.GetLeverScript();
                currentSnapPoint = hitboxScript.GetSnapPoint();
            }
        }

        if (collider.CompareTag("Ladder") && !ladderCooldown)
        {
            LadderHitboxScript hitboxScript = collider.GetComponent<LadderHitboxScript>();
            if (hitboxScript != null)
            {
                currentLadderInRange = hitboxScript.GetLadderScript();
                mountingFromTop = hitboxScript.IsTopHitbox();
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Lever"))
        {
            currentLever = null;
            currentSnapPoint = null;
        }

        if (collider.CompareTag("Ladder"))
        {
            currentLadderInRange = null;
            ladderCooldown = false;
            mountingFromTop = false;
        }
    }

    public void SetLadderCooldown(bool value)
    {
        ladderCooldown = value;
        currentLadderInRange = null;
    }
}
