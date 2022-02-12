using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("Mass of the Train")]
    [SerializeField] public float mass = 1000.0f;                   // default 1000 kg
    [Header("Breaking force of the train")]
    [SerializeField] public float maxBreakForce = 50.0f;            // default 50 N
}
