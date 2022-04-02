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
    [SerializeField] public float dstOffset;
    [HideInInspector] public Follower frontAttachment;
    [HideInInspector] public ObjectOnPath objectOnPath;
    [SerializeField, HideInInspector] Vector3 objectOffset;
    public float attachOffset;
    public EndOfPathInstruction end;
    float distanceTravelled;

    /**
     * Called on the first frame this script is enabled
     */
    private void Start()
    {
        distanceTravelled += dstOffset;
        if (model == null)
        {
            return;
        }
        if (frontAttachment == null)
        {
            trainController = GetComponent<TrainController>();
            dstOffset = EditorPrefs.GetFloat("dstOffset", dstOffset);
            model.transform.position = transform.position = new Vector3(0, 0, 0);
            model.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            distanceTravelled = frontAttachment.distanceTravelled - attachOffset;
        }
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
        transform.position = pathCreator.path.GetPointAtDistance(distance, end);
        transform.position += objectOffset;
        Quaternion normalRotation = Quaternion.Euler(180, 0, 90);
        Quaternion pathRotation = pathCreator.path.GetRotationAtDistance(distance, end);
        transform.rotation = pathRotation * normalRotation;
    }

    /**
     * Update distance offset for Start() function
     */
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
    public Vector3 ObjectOffset
    {
        get {
            return objectOffset;
        }
        set {
            objectOffset = value;
        }
    }
}
