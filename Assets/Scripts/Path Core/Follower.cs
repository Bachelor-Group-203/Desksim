using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Follower : MonoBehaviour
{
    public PathCreator pathCreator;
    public EndOfPathInstruction end;
    public Follower frontAttachment;
    float distanceTravelled;
    public float dstOffset;
    public float attachOffset;
    private TrainController trainController;

    private void Awake()
    {
        if (frontAttachment == null) 
        {
            distanceTravelled += dstOffset;
            trainController = GetComponent<TrainController>();
        }
    }

    void Update()
    {
        if (pathCreator != null) {
            if (frontAttachment != null)
            {
                distanceTravelled = frontAttachment.distanceTravelled - attachOffset;
            }
            else 
                distanceTravelled += trainController.Velocity * Time.deltaTime;
            // Move and rotate game object to points of the path
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, end);
            Quaternion normalRotation = Quaternion.Euler(180, 0, 90);
            Quaternion pathRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, end);
            transform.rotation = pathRotation * normalRotation;
        }
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(4);
    }
}
