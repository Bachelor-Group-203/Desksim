using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnityEngine;
using UnityEditor;
using PathCreation;

/**
 * Follower computes trains position onto path by distance of path
 */
public class Follower : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] public PathCreator pathCreator;
    [SerializeField] public GameObject model;
    [SerializeField] public TrainController trainController;
    [SerializeField] public bool isSignal;
    [SerializeField] public float distanceOffset;

    [Header("Modifiers")]
    [SerializeField] public float attachOffset;
    [SerializeField] public EndOfPathInstruction end;

    //Hidden variables
    [SerializeField, HideInInspector] public Vector3 objectOffset;
    [SerializeField, HideInInspector] float distanceTravelled;
    [SerializeField, HideInInspector] public Follower frontAttachment;
    [SerializeField, HideInInspector] public ObjectOnPath objectOnPath;
    [SerializeField, HideInInspector] public Vector3 signalOffsetPos;

    /**
     * Called on the first frame this script is enabled
     */
    private void Start()
    {
        // If object is front cab
        if (frontAttachment == null)
        {
            trainController = GetComponent<TrainController>();
            distanceOffset = EditorPrefs.GetFloat((string)gameObject.name, distanceOffset);
            model.transform.position = transform.position = new Vector3(0, 0, 0);
            model.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // Place object to correct path distance set in editor
        distanceTravelled += distanceOffset;
    }

    /**
     * Called every frame
     */
    void Update()
    {
        if (pathCreator == null) {
            return;
        }

        // If it is not front cab, else front cab
        if (frontAttachment != null)
            distanceTravelled = frontAttachment.distanceTravelled - attachOffset;
        else if (!isSignal)
            distanceTravelled += trainController.Velocity * Time.deltaTime;
        
        // Move and rotate game object to points of the path
        UpdateObject(distanceTravelled);
    }

    /**
     * Updates train position in relation to path
     *
     * @param       distance        placed distance of path
     */
    void UpdateObject(float distance)
    {
        Quaternion normalRotation;
        model.transform.position = pathCreator.path.GetPointAtDistance(distance, end);

        // If object is every cab, else signal
        if (objectOffset == new Vector3(0, 0, 0) && !isSignal && frontAttachment != null)
        {
            // Rotation of the wagons
            normalRotation = Quaternion.Euler(180, 0, 90);
        }
        else if (isSignal)
        {
            // Rotation of the signal
            normalRotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            // Rotation of the driving cab
            normalRotation = Quaternion.Euler(180, 0, 90);
        }
        
        // Rotate the object relative to point on path
        Quaternion pathRotation = pathCreator.path.GetRotationAtDistance(distance, end);
        model.transform.rotation = pathRotation * normalRotation;
    }

    /**
     * Update distance offset for Start() function
     */
    public void UpdateDistanceOffset(float dst)
    {
        distanceOffset = dst;
    }

    /**
     * Update distance offset for Start() function
     */
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
