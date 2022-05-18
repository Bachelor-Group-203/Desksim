using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SignalEnum;

/**
* This class contains the functionality spesific to the "Hoved Signal" signal
*/
public class HovedSignalScript : SignalScript
{
    [SerializeField] private HovedSignal startStatus = HovedSignal.Av;
    [SerializeField] private HovedSignal endStatus = HovedSignal.Av;
    [Header("Signal Spesifics")]
    [SerializeField] private HovedSignal signalStatus;
    [SerializeField] private Material signalRedMaterial;
    [SerializeField] private Material signalGreenMaterial;

    private int activeSignal = 0;
    private Coroutine routine;

    public HovedSignal StartStatus
    {
        get
        {
            return startStatus;
        }
    }

    public HovedSignal EndStatus
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
            signalStatus = (HovedSignal)value;
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
        // If there is a new active signal
        if (i != activeSignal)
        {
            activeSignal = i;
            // Stops the coroutine of the current showing signalpattern
            if (routine != null)
            {
                StopCoroutine(routine);
            }

            TurnOffAllLights();

            // Makes the new signalpattern show
            switch (i)
            {
                case 0: break;
                case 1: Blink(1, signalRedMaterial); break;
                case 2: ActiveSignal(1, signalRedMaterial); break;
                case 3: ActiveSignal(2, signalGreenMaterial); break;
                case 4: ActiveSignal(0, signalGreenMaterial, 2); break;
                default: Debug.LogError("Not a valid sign number: " + i); break;
            }
        }
    }

    /**
     * This function starts the coroutine for showing the signalpattern
     * 
     * @param       i               The light that is used in the pattern
     * @param       material       The material that the light (i) is using
     */
    private void Blink(int i, Material material)
    {
        routine = StartCoroutine(ActiveBlinking(i, material));
    }

    /**
     * This function is a coroutine and alternates between the material selected and the default off material set
     * in the inspector on this object. This blinks the light selected and will blink 60 times a minuite.
     * 
     * @param       i               The light that is used in the pattern
     * @param       material       The material that the light (i) is using
     */
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

    /**
     * This function activates the light or lights needed to display the signalpattern with the given material
     * 
     * @param       i               The light that is used in the pattern
     * @param       material       The material that the light (i) is using
     * @param       j               The second light that is used in the pattern    (optional)
     */
    private void ActiveSignal(int i, Material material, int j = -1)
    {
        listOfLights[i].GetComponent<MeshRenderer>().material = material;
        if (j >= 0)
        {
            listOfLights[j].GetComponent<MeshRenderer>().material = material;
        }
    }
}
