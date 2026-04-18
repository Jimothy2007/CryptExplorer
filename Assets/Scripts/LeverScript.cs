using System.Collections;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    //[SerializeField] private GameObject platformToMove;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private GameObject activatedHitbox;
    [SerializeField] private GameObject deactivatedHitbox;

    private Quaternion deactivatedRotation = Quaternion.Euler(0, 0, 45);
    private Quaternion activatedRotation = Quaternion.Euler(0, 0, -45);
    private bool isActivated = false;

    public void Start()
    {
        transform.rotation = deactivatedRotation;
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
        Debug.Log("Lever activated! Moving platform...");
        StartCoroutine(RotateLever(activatedRotation));
    }

    public void DeactivateLever()
    {
        if (!isActivated) return;
        isActivated = false;
        UpdateHitboxes();
        Debug.Log("Lever deactivated! Moving platform back...");
        StartCoroutine(RotateLever(deactivatedRotation));
    }

    private void UpdateHitboxes()
    {
        activatedHitbox.SetActive(isActivated);
        deactivatedHitbox.SetActive(!isActivated);
    }

    private IEnumerator RotateLever(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t+= Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}
