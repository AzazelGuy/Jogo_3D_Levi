using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float sensitivity = 2f;

    private float yaw;
    private float pitch;

    private void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * sensitivity;
        pitch -= mouseY * sensitivity;

        pitch = Mathf.Clamp(pitch, -80f, 80f);

        cameraTarget.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}