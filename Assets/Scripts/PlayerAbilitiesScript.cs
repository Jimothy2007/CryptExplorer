using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilitiesScript : MonoBehaviour
{
    private LeverScript currentLever;
    void OnInteract(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            if (currentLever != null)
            {
                currentLever.FlipLever();
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
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Lever"))
        {
            currentLever = null;
        }
    }
}
