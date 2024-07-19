using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTarget : MonoBehaviour {
    [SerializeField] Transform body = default;
    [SerializeField] float animSpeed = 1.0f;
    [SerializeField] float stepDistance = 2.0f;
    [SerializeField] float stepLength = 2.0f;
    [SerializeField] float stepHeight = 1.5f;
    [SerializeField] Vector3 footOffset = default;

    [Space(20), Header("Gizmos")]
    [SerializeField] float gizmoSize = 1.0f;
    [SerializeField] Color targetColor = Color.yellow;
    [SerializeField] Color currentColor = Color.blue;


    public Vector3 GetPosition() => transform.position;
    public void UpdateTargetPosition(Vector3 pos) => targetPos = pos;
    public bool IsMoving() => lerp < 1.0f;


    public float footSpacing;
    public Vector3 currentPos, targetPos, prevPos;
    public float lerp = 0.0f;


    void Start() {
        footSpacing = transform.localPosition.x;
        currentPos = targetPos = prevPos = transform.position;
        lerp = 1.0f;
    }

    public Ray ray;
    public RaycastHit hit;

    public void UpdatePositions(IKTarget other) {
        transform.position = currentPos;

        
        ray = new Ray(body.position + body.right + (Vector3.up * 0.5f), Vector3.down);
        if (Physics.Raycast(ray, out hit, 10)) {
            if (Vector3.Distance(targetPos, hit.point) > stepDistance && !other.IsMoving() && lerp >= 1) {
                lerp = 0.0f;
                int direction = body.InverseTransformPoint(hit.point).z > body.InverseTransformPoint(targetPos).z ? 1 : -1;
                Vector3 offset = Vector3.forward * (stepLength * direction) + footOffset;
                UpdateTargetPosition(hit.point + offset + transform.TransformDirection(Vector3.forward));
            }
        }
        
        print(hit.point);
        
        if (IsMoving()) {
            currentPos = Vector3.Lerp(prevPos, targetPos, lerp);
            currentPos.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;
            lerp += Time.deltaTime * animSpeed;
        } else {
            prevPos = targetPos;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = targetColor;
        Gizmos.DrawSphere(targetPos, gizmoSize);

        Gizmos.color = currentColor;
        Gizmos.DrawSphere(currentPos, gizmoSize);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(ray.origin, hit.point);
    }
}
