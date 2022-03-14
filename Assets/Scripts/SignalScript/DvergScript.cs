using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DvergScript : SignalScript
{
    [SerializeField] private DvergSignal signalStatus;
    [SerializeField] private Material signalOnMaterial;

    enum DvergSignal
    {
        Av,
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

    private void ActivateNewSignal(int i)
    {
        // Turns off all the lights
        TurnOffAllLights();

        if (i != 0)
        {
            // Makes the new light show
            switch (i)
            {
                case 1: ActiveSignal(0, 1); break;
                case 2: ActiveSignal(0, 2); break;
                case 3: ActiveSignal(0, 3); break;
                case 4: ActiveSignal(1, 3); break;
                default: Debug.LogError("Not a valid sign number: " + i); break;
            }
        }
    }

    private void ActiveSignal(int i, int j)
    {
        listOfLights[i].GetComponent<MeshRenderer>().material = signalOnMaterial;
        listOfLights[j].GetComponent<MeshRenderer>().material = signalOnMaterial;
    }
}
