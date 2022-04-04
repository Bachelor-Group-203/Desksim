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

    /**
     * Called on the first frame this script is enabled
     */
    private void Start()
    {
        if (frontAttachment == null)
        {
            trainController = GetComponent<TrainController>();
            distanceOffset = EditorPrefs.GetFloat((string)gameObject.name, distanceOffset);
            model.transform.position = transform.position = new Vector3(0, 0, 0);
            model.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        GetObjectOffset();
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
        if (frontAttachment != null)
            distanceTravelled = frontAttachment.distanceTravelled - attachOffset;
        else if (!isSignal)
            distanceTravelled += trainController.Velocity * Time.deltaTime;
        
        // Move and rotate game object to points of the path
        UpdateTrain(distanceTravelled);
    }

    /**
     * Sets offset of object on path (used for signals)
     */
    private void GetObjectOffset() {
        Vector3 vec;
        vec.x = EditorPrefs.GetFloat("x" + (string)gameObject.name);
        vec.y = EditorPrefs.GetFloat("y" + (string)gameObject.name);
        vec.z = EditorPrefs.GetFloat("z" + (string)gameObject.name);
        objectOffset = new Vector3(vec.x, vec.y, vec.z);
    }

    /**
     * Updates train position in relation to path
     *
     * @param       distance        placed distance of path
     */
    void UpdateTrain(float distance)
    {
        model.transform.position = pathCreator.path.GetPointAtDistance(distance, end);

        if (objectOffset != new Vector3(0, 0, 0) && frontAttachment == null)
            model.transform.position += objectOffset;

        Quaternion normalRotation = Quaternion.Euler(180, 0, 90);
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
