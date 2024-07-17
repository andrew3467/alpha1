using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform playerCamera; // Camera Transform
    public Transform hand;         // Hand/Flashlight Transform
    public float cameraSensitivity = 200.0f;
    public float cameraAcceleration = 5.0f;
    public float followDelay = 0.5f; // Delay for the camera to follow the flashlight
    public float maxFlashlightAngle = 45.0f; // Maximum angle the flashlight can deviate from the camera
    public float resetAngleThreshold = 60.0f; // Angle threshold to reset flashlight position

    private float rotationX = 0;
    private float rotationY = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Update rotation based on mouse input
        rotationX += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -90.0f, 90.0f);

        // Calculate the target rotation for the camera
        Quaternion targetCameraRotation = Quaternion.Euler(-rotationX, 0, 0);
        Quaternion targetPlayerRotation = Quaternion.Euler(0, rotationY, 0);

        // Smoothly interpolate the camera rotation towards the target rotation
        playerCamera.localRotation = Quaternion.Lerp(playerCamera.localRotation, targetCameraRotation, cameraAcceleration * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetPlayerRotation, cameraAcceleration * Time.deltaTime);

        // Calculate the desired rotation of the flashlight
        Quaternion desiredFlashlightRotation = Quaternion.Euler(-rotationX, rotationY, 0);

        // Clamp the flashlight rotation relative to the camera
        Vector3 flashlightEulerAngles = desiredFlashlightRotation.eulerAngles;
        flashlightEulerAngles.x = ClampAngle(flashlightEulerAngles.x, -maxFlashlightAngle, maxFlashlightAngle);
        flashlightEulerAngles.y = ClampAngle(flashlightEulerAngles.y, -maxFlashlightAngle, maxFlashlightAngle);

        // Apply the clamped rotation to the flashlight
        desiredFlashlightRotation = Quaternion.Euler(flashlightEulerAngles);

        // Smoothly interpolate the flashlight rotation towards the clamped rotation
        hand.rotation = Quaternion.Lerp(hand.rotation, desiredFlashlightRotation, followDelay * Time.deltaTime);

        // Check if the flashlight is within the camera bounds
        if (!IsFlashlightWithinCameraBounds())
        {
            // Reset flashlight position to where the camera is pointing
            hand.rotation = playerCamera.rotation;
        }
    }

    // Helper method to clamp an angle between a min and max value
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < 90 || angle > 270) // if angle in the critical region...
        {
            if (angle > 180) angle -= 360; // convert all angles to -180 to 180
            if (max > 180) max -= 360;
            if (min > 180) min -= 360;
        }
        angle = Mathf.Clamp(angle, min, max);
        if (angle < 0) angle += 360; // if angle negative, convert to 0-360
        return angle;
    }

    // Check if the flashlight is within the camera bounds
    bool IsFlashlightWithinCameraBounds()
    {
        float angleDifference = Quaternion.Angle(playerCamera.rotation, hand.rotation);
        return angleDifference <= resetAngleThreshold;
    }
}
