using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour {
    InputSystemFirstPersonControls inputActions;
    private CharacterController controller;
    
    [SerializeField] private Camera cam;
    [SerializeField] private float movementSpeed = 2.0f;
    [SerializeField] public float lookSensitivity = 1.0f;
    
    private float xRotation = 0f;

    // Movement Vars
    private Vector3 velocity;
    public float gravity = -9.81f;
    private bool grounded;
    
    // Crouch Vars
    private float initHeight;
    [SerializeField] private float crouchHeight;
    

    void Start() {
        controller = GetComponent<CharacterController>();
        initHeight = controller.height;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        Move();
        Rotate();
        Crouch();
    }


    void Crouch() {
        
    }

    void Rotate() {
        Vector2 looking = GetPlayerLook();
        
        float lookX = looking.x * lookSensitivity * Time.deltaTime;
        float lookY = looking.y * lookSensitivity * Time.deltaTime;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        transform.Rotate(Vector3.up * lookX);
        cam.transform.parent.Rotate(Vector3.up * lookX);
    }

    void Move() {
        grounded = controller.isGrounded;
        if (grounded && velocity.y < 0) {
            velocity.y = -2f;
        }
    }
    
    public Vector2 GetPlayerMovement()
    {
        return inputActions.FPSController.Move.ReadValue<Vector2>();
    }

    Vector2 GetPlayerLook() {
        return inputActions.FPSController.Look.ReadValue<Vector2>();
    }
}
