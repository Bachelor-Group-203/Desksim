using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SignalEnum;

public class HovedSignalScript : SignalScript
{
    [SerializeField] private HovedSignal signalStatus;
    [SerializeField] private Material signalRedMaterial;
    [SerializeField] private Material signalGreenMaterial;

    private int activeSignal = 0;
    private Coroutine routine;

    public int SignalStatus
    {
        set
        {
            signalStatus = (HovedSignal)value;
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

            switch (i)
            {
                case 0: break;
                case 1: ActiveSignal(1, signalRedMaterial); break;
                case 2: Blink(1, signalRedMaterial); break;
                case 3: ActiveSignal(2, signalGreenMaterial); break;
                case 4: ActiveSignal(0, signalGreenMaterial, 2); break;
                default: Debug.LogError("Not a valid sign number: " + i); break;
            }
        }
    }

    private void Blink(int i, Material material1)
    {
        routine = StartCoroutine(ActiveBlinking(i, material1));
    }

    IEnumerator ActiveBlinking(int i, Material material)
    {
        while (true)
        {
            listOfLights[i].GetComponent<MeshRenderer>().material = material;
            yield return new WaitForSeconds(0.5f);
            listOfLights[i].GetComponent<MeshRenderer>().material = signalOffMaterial;
            yield return new WaitForSeconds(0.5f);
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
