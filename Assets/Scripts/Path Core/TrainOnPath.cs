using System.Collections;
using UnityEditor;
using PathCreation;
using PathCreation.Utility;
using System.Collections.Generic;
using UnityEngine;
public class TrainOnPath : MonoBehaviour
{
    public GameObject train;
    public PathCreator pathCreator;
    public PathSpace space;
    //public float minDst;
    float distanceTravelled; 
    Vector3 position;
    RaycastHit hit;
    Vector3 worldMouse;
    Ray mouseRay;
    private void Start()
    {
        /* minDst = 9999f;
        mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition); */
    }
    /* private void Update(){
        objectMouseHover();
    } */
    /* private void objectMouseHover () {
        for (int i = 0; i < pathCreator.bezierPath.NumPoints; i++) {
            Vector3 pos = MathUtility.TransformPoint (pathCreator.bezierPath[i], train.transform, space);
            float dst = HandleUtility.DistanceToCircle (pos, 1);
            minDst = (dst < minDst) ? minDst = dst : minDst;
        }
        train.transform.position = position;

        //position = Handles.matrix.inverse.MultiplyPoint(worldMouse);

        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, end);
        Quaternion normalRotation = Quaternion.Euler(180, 0, 90);
        Quaternion pathRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, end);
        transform.rotation = pathRotation * normalRotation;
    } */

}
