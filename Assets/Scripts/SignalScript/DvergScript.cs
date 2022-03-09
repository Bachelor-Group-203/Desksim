using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DvergScript : MonoBehaviour
{
    [SerializeField] private DvergSignal signalStatus;
    [SerializeField] private Transform lights;
    [SerializeField] private Material signalOffMaterial;
    [SerializeField] private Material signalOnMaterial;

    private List<Transform> listOfLights = new List<Transform>();

    enum DvergSignal
    {
        SkiftingForbudt,
        VarsomSkifting,
        SkiftingTillat,
        FrigittSkifting
    }

    private void Awake()
    {
        GetAllLights();
    }

    private void Update()
    {
        ActivateNewSignal((int)signalStatus);
    }

    private void GetAllLights()
    {
        foreach (Transform light in lights)
        {
            light.GetComponent<MeshRenderer>().material = signalOffMaterial;
            listOfLights.Add(light);
        }
    }

    private void ActivateNewSignal(int i)
    {
        // Turns off all the lights
        foreach (Transform light in listOfLights)
        {
            light.GetComponent<MeshRenderer>().material = signalOffMaterial;
        }

        // Makes the new light show
        switch (i)
        {
            case 0: ActiveSignal(0, 1); break;
            case 1: ActiveSignal(0, 2); break;
            case 2: ActiveSignal(0, 3); break;
            case 3: ActiveSignal(1, 3); break;
            default: Debug.LogError("Not a valid sign number: " + i); break;
        }
    }

    private void ActiveSignal(int i, int j)
    {
        listOfLights[i].GetComponent<MeshRenderer>().material = signalOnMaterial;
        listOfLights[j].GetComponent<MeshRenderer>().material = signalOnMaterial;
    }
}
