using UnityEngine;
using UnityEngine.InputSystem;

public class CameraBoom : MonoBehaviour
{
    [SerializeField] private float boomLength = 4f;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float minTilt = -15f;
    [SerializeField] private float maxTilt = 45f;
    [SerializeField] private Transform cameraTarget;

    private float yaw;
    private float pitch;
    private Vector2 lookInput;

    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    private void Update()
    {
        yaw += lookInput.x * mouseSensitivity * Time.deltaTime;
        pitch -= lookInput.y * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minTilt, maxTilt);
        
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        cameraTarget.position = transform.position + transform. rotation * Vector3.back * boomLength;
    }
}
