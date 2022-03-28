using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SignalEnum;

public class SignalScript : MonoBehaviour
{
    [SerializeField] private SignalType signalType;
    [SerializeField] protected Transform lights;
    [SerializeField] protected Material signalOffMaterial;

    protected List<Transform> listOfLights = new List<Transform>();

    [SerializeField] protected bool trainTrigger = false;

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

    /*
     * Turns off all the lights on the object
     */
    protected void TurnOffAllLights()
    {
        foreach (Transform light in listOfLights)
        {
            light.GetComponent<MeshRenderer>().material = signalOffMaterial;
        }
    }

    /*
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
}
