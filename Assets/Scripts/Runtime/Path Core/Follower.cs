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
        // Cheks if the item has a pathcreator object if not finds the path object and assigns it
        if (pathCreator == null && !isSignal)
        {
            try
            {
                PathCreator path = GameObject.FindGameObjectWithTag("Rail").GetComponent<PathCreator>();

                if (path == null) throw new Exception();

                objectOnPath.follower.pathCreator = path;
                pathCreator = path;
            }
            catch (Exception e)
            {
                Debug.LogWarning("No path in the Scene: " + e);
                return;
            }
        }

        // If object is front cab
        if (frontAttachment == null && !isSignal)
        {
            trainController = GetComponent<TrainController>();
            model.transform.position = transform.position = new Vector3(0, 0, 0);
            model.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (gameObject.tag == "Train")
        {
            distanceTravelled = PlayerPrefs.GetFloat(gameObject.name);
        }

        if (isSignal)
        {
            distanceTravelled = PlayerPrefs.GetFloat(gameObject.name);
        }

        Debug.Log(gameObject.name + "\t" + distanceOffset);

        // Place object to correct path distance set in editor
        //distanceTravelled += distanceOffset;
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
     * Returns itself as a gameobject
     * 
     * @return                      Returns itself as a gameobject
     */
    public GameObject GetGameObject()
    {
        return gameObject;
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
