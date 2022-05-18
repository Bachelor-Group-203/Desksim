using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SignalEnum;


/**
 * This class is the parent class for all the signal classes and contain core functions and variables that the
 * different signals share. 
 */
public class SignalScript : MonoBehaviour
{
    [Header("Default Objects")]
    [SerializeField] protected Transform lights;
    [SerializeField] protected Material signalOffMaterial;
    [Header("Select Behaviour")]
    [SerializeField] private SignalType signalType;
    [SerializeField] protected DetectTrain trainDetectionType = DetectTrain.Ingen;
    [Header("Signal Status")]
    [SerializeField] private int timer = 0;

    protected List<Transform> listOfLights = new List<Transform>();
    protected BoxCollider[] listOfCollider;

    protected bool trainTrigger = false;

    public int Timer
    {
        get
        {
            return timer;
        }
    }

    public DetectTrain TrainDetectionType
    {
        get
        {
            return trainDetectionType;
        }
    }

    public bool TrainTrigger
    {
        get
        {
            return trainTrigger;
        }
        set
        {
            trainTrigger = value;
        }
    }

    public SignalType SignalType
    {
        get
        {
            return signalType;
        }
    }

    /**
     * Turns off all the lights on the object
     */
    protected void TurnOffAllLights()
    {
        foreach (Transform light in listOfLights)
        {
            light.GetComponent<MeshRenderer>().material = signalOffMaterial;
        }
    }

    /**
     * The function finds all the Lights in the Lights component and turns them off
     */
    protected void GetAllLights()
    {
        foreach (Transform light in lights)
        {
            listOfLights.Add(light);
        }

        TurnOffAllLights();
    }

    /**
     * The function activates the right collider for the signal
     */
    protected void ActivateBoxColliders()
    {
        listOfCollider = transform.GetChild(0).GetComponents<BoxCollider>();

        foreach (BoxCollider boxCollider in listOfCollider)
        {
            boxCollider.enabled = false;
        }

        // Enables the apropriate collider
        if (trainDetectionType == DetectTrain.ForanSkilt)
        {
            listOfCollider[1].enabled = true;
        }
        else if (trainDetectionType == DetectTrain.PasserSkilt)
        {
            listOfCollider[0].enabled = true;
        }
    }

    /**
     * Moves the collision boxes relative to the signal so that it aligns with the rails only one is described
     * as only the box furthest away is needed to align both of them.
     * 
     * @param       newPos          Describes only the position that is the diagonal to the signal
     */
    public void MoveBoxColliders(Vector3 newPos)
    {
        listOfCollider = transform.GetChild(0).GetComponents<BoxCollider>();

        listOfCollider[0].center = new Vector3(newPos.x, transform.position.y + 1.0f, 0);
        listOfCollider[1].center = new Vector3(newPos.x, transform.position.y + 1.0f, newPos.z);
    }

    /**
     * The function handles detecting the train when interacting with the signal
     */
    public void CollisionDetcted(CollisionDetection script)
    {
        trainTrigger = true;
    }
}
