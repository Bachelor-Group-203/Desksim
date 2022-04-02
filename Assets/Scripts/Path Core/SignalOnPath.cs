using System.Collections;
using UnityEditor;
using PathCreation;
using PathCreation.Utility;
using System.Collections.Generic;
using UnityEngine;

/**
 * ObjectOnPath is a tool for "ObjectEditor" to work on object with this component
 */
public class SignalOnPath : MonoBehaviour
{
    [HideInInspector] public PathSpace space;
    public Follower follower;
    float distanceTravelled; 
    Vector3 position;
    RaycastHit hit;
    Vector3 worldMouse;
    Ray mouseRay;
}