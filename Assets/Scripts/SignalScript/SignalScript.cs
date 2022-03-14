using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalScript : MonoBehaviour
{
    [SerializeField] protected Transform lights;
    [SerializeField] protected Material signalOffMaterial;

    protected List<Transform> listOfLights = new List<Transform>();

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
