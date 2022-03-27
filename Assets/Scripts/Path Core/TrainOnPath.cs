using System.Collections;
using UnityEditor;
using PathCreation;
using PathCreation.Utility;
using System.Collections.Generic;
using UnityEngine;
public class TrainOnPath : MonoBehaviour
{
    public Follower follower;
    public PathSpace space;
    float distanceTravelled; 
    Vector3 position;
    RaycastHit hit;
    Vector3 worldMouse;
    Ray mouseRay;
}
