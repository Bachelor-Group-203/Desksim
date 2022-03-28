using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SignalEnum;

public class ForSignalScript : SignalScript
{
    [SerializeField] private ForSignal signalStatus;
    [SerializeField] private Material signalYellowMaterial;
    [SerializeField] private Material signalGreenMaterial;

    private int activeSignal = 0;
    private Coroutine routine;

    public int SignalStatus
    {
        set
        {
            signalStatus = (ForSignal)value;
        }
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
        if (i != activeSignal)
        {
            activeSignal = i;
            if (routine != null)
            {
                StopCoroutine(routine);
            }

            TurnOffAllLights();

            // Makes the new light show
            switch (i)
            {
                case 0: break;
                case 1: Blink(1, signalYellowMaterial); break;
                case 2: Blink(1, signalYellowMaterial, 0, signalGreenMaterial); break;
                case 3: Blink(0, signalGreenMaterial); break;
                default: Debug.LogError("Not a valid sign number: " + i); break;
            }
        }
    }

    private void Blink(int i, Material material1, int j = -1, Material material2 = default)
    {
        if (j >= 0)
        {
            routine = StartCoroutine(ActiveBlinkingTwo(i, material1, j, material2));
        }
        else
        {
            routine = StartCoroutine(ActiveBlinkingOne(i, material1));
        }
    }

    IEnumerator ActiveBlinkingOne(int i, Material material)
    {
        while (true)
        {
            listOfLights[i].GetComponent<MeshRenderer>().material = material;
            yield return new WaitForSeconds(0.5f);
            listOfLights[i].GetComponent<MeshRenderer>().material = signalOffMaterial;
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator ActiveBlinkingTwo(int i, Material material1, int j, Material material2)
    {
        while (true)
        {
            listOfLights[i].GetComponent<MeshRenderer>().material = material1;
            listOfLights[j].GetComponent<MeshRenderer>().material = material2;
            yield return new WaitForSeconds(0.5f);
            listOfLights[i].GetComponent<MeshRenderer>().material = signalOffMaterial;
            listOfLights[j].GetComponent<MeshRenderer>().material = signalOffMaterial;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
