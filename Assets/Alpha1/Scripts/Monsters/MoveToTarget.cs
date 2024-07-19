using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTarget : MonoBehaviour {
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float turnSpeed = 1.0f;
    [SerializeField] Transform target = default;


    void Update() {
        transform.LookAt(target.position);
        transform.Translate(transform.forward * (moveSpeed * Time.deltaTime));
    }
}
