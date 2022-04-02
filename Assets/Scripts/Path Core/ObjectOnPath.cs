using System.Collections;
using UnityEditor;
using PathCreation;
using PathCreation.Utility;
using System.Collections.Generic;
using UnityEngine;

/**
 * ObjectOnPath is a tool for "ObjectEditor" to work on object with this component
 */
public class ObjectOnPath : MonoBehaviour
{
    [HideInInspector] public PathSpace space;
    [SerializeField] public Vector3 objectOffset;
    public Follower follower;
    float distanceTravelled; 
    RaycastHit hit;
    Vector3 position;
    Vector3 worldMouse;
    Ray mouseRay;
}
