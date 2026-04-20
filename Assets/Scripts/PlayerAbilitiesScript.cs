using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilitiesScript : MonoBehaviour
{
    private LeverScript currentLever;
    private Transform currentSnapPoint;
    void OnInteract(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            Debug.Log("Interact button pressed");
            if (currentLever != null)
            {
                GetComponent<PlayerMovement>().SnapToPoint(currentSnapPoint);
                currentLever.FlipLever();
                GetComponent<PlayerMovement>().UnsnapFromPoint();
                Debug.Log("Interacted with lever: " + currentLever.gameObject.name);
                currentLever = null;
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
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Lever"))
        {
            currentLever = null;
            currentSnapPoint = null;
        }
    }
}
