using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematicTests : MonoBehaviour {
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform body = default;
    [SerializeField] InverseKinematicTests otherFoot = default;
    [SerializeField] float speed = 1;
    [SerializeField] float stepDistance = 4;
    [SerializeField] float stepLength = 4;
    [SerializeField] float stepHeight = 1;
    [SerializeField] Vector3 footOffset = default;
    [SerializeField] bool useOldMethod = false;
    
    float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;

    void Start()
    {
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1;
    }
    
    
    Ray ray;
    RaycastHit info;
    void Update() {
        if(useOldMethod) Method1(); else Method2();
    }

    void Method2() {
        ray = new Ray(body.position + (Vector3.right * transform.localPosition.z), Vector3.down);
        Physics.Raycast(ray, out info, 10, terrainLayer.value);
    }
    
    void Method1() {
        transform.position = currentPosition;
        transform.up = currentNormal;

        ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);

        Vector3 offset;
        if (Physics.Raycast(ray, out info, 10, terrainLayer.value))
        {
            if (Vector3.Distance(newPosition, info.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                offset = (Vector3.forward * (stepLength * direction)) + footOffset;
                newPosition = info.point + offset;
                newNormal = info.normal;
            }
        }

        if (IsMoving())
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.25f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(oldPosition, 0.25f);
        
        Gizmos.DrawLine(ray.origin, info.point);
    }



    public bool IsMoving()
    {
        return lerp < 1;
    }
}