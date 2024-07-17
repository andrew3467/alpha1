using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour {
    public float moveSpeed = 1.0f;
    
    void FixedUpdate() {
        transform.Translate(Vector3.forward * (moveSpeed * Time.deltaTime));
    }
}
