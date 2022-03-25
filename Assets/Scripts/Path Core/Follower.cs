using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnityEngine;
using PathCreation;

public class Follower : MonoBehaviour
{
    public Follower frontAttachment;
    public PathCreator pathCreator;
    [HideInInspector] public GameObject train;
    [HideInInspector] public TrainOnPath trainOnPath;
    TrainController trainController;
    public float dstOffset;
    public float attachOffset;
    float distanceTravelled;
    public EndOfPathInstruction end;

    private void Start()
    {
        if (frontAttachment == null && train != null && trainOnPath != null)
        {
                trainController = GetComponent<TrainController>();
                trainOnPath = GetComponent<TrainOnPath>();
                dstOffset = trainOnPath.newDstOffset;
                train.transform.position = this.transform.position = new Vector3(0, 0, 0);
                train.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        
        if (frontAttachment != null && pathCreator == null)
        {
            //StartCoroutine(Wait(10));
            pathCreator = frontAttachment.pathCreator;
        }
        distanceTravelled += dstOffset;
    }

    void Update()
    {
        if (pathCreator != null) {
            if (frontAttachment != null)
                distanceTravelled = frontAttachment.distanceTravelled - attachOffset;
            else 
                distanceTravelled += trainController.Velocity * Time.deltaTime;
            // Move and rotate game object to points of the path
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, end);
            Quaternion normalRotation = Quaternion.Euler(180, 0, 90);
            Quaternion pathRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, end);
            transform.rotation = pathRotation * normalRotation;
        }
    }

    public void UpdateDstOffset(float dst)
    {
        dstOffset = dst;
    }

    /* private IEnumerator Wait(float sec)
    {
        yield return new WaitForSeconds(sec);
    } */
}
