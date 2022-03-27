using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnityEngine;
using PathCreation;

public class Follower : MonoBehaviour
{
    public PathCreator pathCreator;
    [HideInInspector] public Follower frontAttachment;
    [HideInInspector] public GameObject train;
    [HideInInspector] public TrainOnPath trainOnPath;
    TrainController trainController;
    public float dstOffset;
    public float attachOffset;
    public EndOfPathInstruction end;
    float distanceTravelled;

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
        else if (pathCreator == null)
        {
            pathCreator = frontAttachment.pathCreator;
        }
        distanceTravelled += dstOffset;
    }

    void Update()
    {
        if (pathCreator == null) {
            return;
        }
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

    public void UpdateDstOffset(float dst)
    {
        dstOffset = dst;
    }
    public PathCreator PathCreator
    {
        get {
            return pathCreator;
        }
        set {
            pathCreator = value;
        }
    }
}
