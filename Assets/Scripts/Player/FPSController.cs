using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour {
    [SerializeField] CharacterController characterController;
    [SerializeField] GameObject body;
    [SerializeField] Camera camera;
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float sensitivity = 1.0f;
    [SerializeField] float rotateXLimit = 45f;
    

    float rotationX = 0;
    Vector3 moveDirection = Vector3.zero;



    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void FixedUpdate() {
        Move();
        Rotate();
    }


    void Rotate() {
        rotationX += -Input.GetAxis("Mouse Y") * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -rotateXLimit, rotateXLimit);
        camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sensitivity, 0);
    }

    void Move() {
        bool grounded = characterController.isGrounded;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = moveSpeed * Input.GetAxis("Vertical");
        float curSpeedZ = moveSpeed * Input.GetAxis("Horizontal");

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedZ);
        
        if (!grounded) {
            moveDirection.y += Alpha1.GRAVITY;
        }

        characterController.Move(moveDirection);
    }
}
