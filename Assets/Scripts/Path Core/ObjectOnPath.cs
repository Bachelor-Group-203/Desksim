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
    [SerializeField] public Follower follower;
    [SerializeField] public Vector3 objectOffset;
    [SerializeField] public float offsetDistance = 5;
    [SerializeField, HideInInspector] public PathSpace space;
    [SerializeField, HideInInspector] float distanceTravelled;
    [SerializeField, HideInInspector] RaycastHit hit;
    [SerializeField, HideInInspector] Vector3 position;
    [SerializeField, HideInInspector] Vector3 worldMouse;
    [SerializeField, HideInInspector] Ray mouseRay;
}
