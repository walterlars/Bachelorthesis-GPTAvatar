using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform Target;
    public float MouseSensitivity = 6f;
    public float CameraHeightOffset = 1.5f; 

    private float verticalRotation;
    private float horizontalRotation;

    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        Vector3 targetPosition = Target.position;
        targetPosition.y += CameraHeightOffset;
        transform.position = targetPosition;


        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        verticalRotation -= mouseY * MouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

        horizontalRotation += mouseX * MouseSensitivity;

        transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }
}
