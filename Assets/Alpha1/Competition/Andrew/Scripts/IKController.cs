using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour {
    
    [Header("Right Leg")]
    [SerializeField] IKTarget rightLeg;



    [Header("Left Leg")]
    [SerializeField] IKTarget leftLeg;


    void Update() {
        rightLeg.UpdatePositions(leftLeg);
        leftLeg.UpdatePositions(rightLeg);
    }
}
