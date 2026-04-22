using System.Collections;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    private Vector3 deactivatedLocation;
    private Vector3 activatedLocation;

    private bool isActivated = false;

    private void Start()
    {
        deactivatedLocation = transform.position;
        activatedLocation = transform.position + new Vector3(0, 3, 0);
    }

    public void MoveToActivatedPosition()
    {
        if (isActivated) return;
        isActivated = true;
        StopAllCoroutines();
        StartCoroutine(MovePlatform(activatedLocation));
    }

    public void MoveToDeactivatedPosition()
    {
        if (!isActivated) return;
        isActivated = false;
        StopAllCoroutines();
        StartCoroutine(MovePlatform(deactivatedLocation));
    }

    private IEnumerator MovePlatform(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
    }
}