using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainValues : MonoBehaviour
{
    [Header("Mass of the Train in kg")]
    [SerializeField] public float mass = 1000.0f;                   // default 1000 ton
    [Header("Acceleration of the train in m/s")]
    [SerializeField] public float maxAcceleration = 5.0f;           // default 5 m/s
    [Header("Breaking force of the train in N")]
    [SerializeField] public float maxBreakForce = 50.0f;            // default 50 N
}
