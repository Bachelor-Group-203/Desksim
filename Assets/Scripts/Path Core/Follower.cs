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
    public PathCreator pathCreator;
    public GameObject model;
    public TrainController trainController;
    [SerializeField] public float distanceOffset;
    [HideInInspector] public Follower frontAttachment;
    [HideInInspector] public ObjectOnPath objectOnPath;
    public float attachOffset;
    public EndOfPathInstruction end;
    float distanceTravelled;
    [SerializeField] public Vector3 objectOffset;

    /**
     * Called on the first frame this script is enabled
     */
    private void Start()
    {
        if (frontAttachment == null)
        {
            trainController = GetComponent<TrainController>();
            distanceOffset = EditorPrefs.GetFloat("dstOffset", distanceOffset);
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
        else 
            distanceTravelled += trainController.Velocity * Time.deltaTime;
        // Move and rotate game object to points of the path
        UpdateTrain(distanceTravelled);
    }

    /**
     * Updates train position in relation to path
     *
     * @param       distance        placed distance of path
     */
    void UpdateTrain(float distance)
    {
        Debug.Log(objectOffset);
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
     * Sets offset of object on path (used for signals)
     */
    private void GetObjectOffset() {
        Vector3 vec;
        vec.x = EditorPrefs.GetFloat("xOffset");
        vec.y = EditorPrefs.GetFloat("yOffset");
        vec.z = EditorPrefs.GetFloat("zOffset");
        objectOffset = new Vector3(vec.x, vec.y, vec.z);
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
