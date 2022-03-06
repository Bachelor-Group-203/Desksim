using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Follower : MonoBehaviour
{
    public PathCreator pathCreator;
    public EndOfPathInstruction end;
    float distanceTravelled;

    private Traincontroller trainController;

    private void Awake()
    {
        trainController = GetComponent<Traincontroller>();
    }

    void Update()
    {
        if (pathCreator != null) // if path exists
        {
            // Move and rotate game object to points of the path
            distanceTravelled += trainController.Velocity * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, end);
            Quaternion normalRotation = Quaternion.Euler(180, 0, 90);
            Quaternion pathRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, end);

            transform.rotation = pathRotation * normalRotation;
        }
    }
}
