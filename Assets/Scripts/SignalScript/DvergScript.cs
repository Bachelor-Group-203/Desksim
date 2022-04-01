using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SignalEnum;

/**
* This class contains the functionality spesific to the "Dverg Signal" signal
*/
public class DvergScript : SignalScript
{
    [SerializeField] private DvergSignal startStatus = DvergSignal.Av;
    [SerializeField] private DvergSignal endStatus = DvergSignal.Av;
    [Header("Signal Spesifics")]
    [SerializeField] private DvergSignal signalStatus;
    [SerializeField] private Material signalOnMaterial;

    public DvergSignal StartStatus
    {
        get
        {
            return startStatus;
        }
    }

    public DvergSignal EndStatus
    {
        get
        {
            return endStatus;
        }
    }

    public int SignalStatus
    {
        set
        {
            signalStatus = (DvergSignal)value;
        }
    }

    /**
     * This function starts first when the object is compiled and initializes values
     */
    private void Awake()
    {
        GetAllLights();
        ActivateBoxColliders();
    }

    /**
     * This function runns per frame
     */
    private void Update()
    {
        ActivateNewSignal((int)signalStatus);
    }

    /**
     * This function stops the signal that is currently displaying and turns on the new signal
     * 
     * @param       i               The value that represents the signalpattern to display for this signaltype
     */
    private void ActivateNewSignal(int i)
    {
        // Turns off all the lights
        TurnOffAllLights();

        // Makes the new signalpattern show
        switch (i)
        {
            case 0: break;
            case 1: ActiveSignal(0, 1); break;
            case 2: ActiveSignal(0, 2); break;
            case 3: ActiveSignal(0, 3); break;
            case 4: ActiveSignal(1, 3); break;
            default: Debug.LogError("Not a valid sign number: " + i); break;
        }
    }

    /**
     * This function activates the lights needed to display the signalpattern
     * 
     * @param       i               The light that is used in the pattern
     * @param       j               The second light that is used in the pattern
     */
    private void ActiveSignal(int i, int j)
    {
        listOfLights[i].GetComponent<MeshRenderer>().material = signalOnMaterial;
        listOfLights[j].GetComponent<MeshRenderer>().material = signalOnMaterial;
    }
}
