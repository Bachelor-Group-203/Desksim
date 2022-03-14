using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NohsScript : SignalScript
{
    [SerializeField] private NohsSignal signalStatus;
    [SerializeField] private Material signalRedMaterial;
    [SerializeField] private Material signalGreenMaterial;

    enum NohsSignal
    {
        Av,
        Stop,
        KjørMedRedusertHastighet,
        Kjør
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
                case 1: ActiveSignal(1, signalRedMaterial); break;
                case 2: ActiveSignal(2, signalGreenMaterial); break;
                case 3: ActiveSignal(0, signalGreenMaterial, 2); break;
                default: Debug.LogError("Not a valid sign number: " + i); break;
            }
        }
    }

    private void ActiveSignal(int i, Material material, int j = -1)
    {
        listOfLights[i].GetComponent<MeshRenderer>().material = material;
        if (j >= 0)
        {
            listOfLights[j].GetComponent<MeshRenderer>().material = material;
        }
    }
}
