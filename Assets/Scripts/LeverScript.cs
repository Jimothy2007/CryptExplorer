using System.Collections;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    //[SerializeField] private GameObject platformToMove;
    [SerializeField] private GameObject activatedHitbox;
    [SerializeField] private GameObject deactivatedHitbox;
    //[SerializeField] private Animator animator;
    private bool isActivated = false;

    public void Start()
    {
        UpdateHitboxes();
    }

    public void FlipLever()
    {
        if (isActivated)
        {
            DeactivateLever();
        }
        else
        {
            ActivateLever();
        }
    }

    public void ActivateLever()
    {
        if (isActivated) return;
        isActivated = true;
        UpdateHitboxes();
        //animator.SetBool("IsActivated", true);
        Debug.Log("Lever activated! Moving platform...");
    }

    public void DeactivateLever()
    {
        if (!isActivated) return;
        isActivated = false;
        UpdateHitboxes();
        //animator.SetBool("IsActivated", false);
        Debug.Log("Lever deactivated! Moving platform back...");
    }

    private void UpdateHitboxes()
    {
        activatedHitbox.SetActive(isActivated);
        deactivatedHitbox.SetActive(!isActivated);
    }
}
