using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SignalEnum;

/**
* This class contains the functionality spesific to the "For Signal" signal
*/
public class ForSignalScript : SignalScript
{
    [SerializeField] private ForSignal startStatus = ForSignal.Av;
    [SerializeField] private ForSignal endStatus = ForSignal.Av;
    [Header("Signal Spesifics")]
    [SerializeField] private ForSignal signalStatus;
    [SerializeField] private Material signalYellowMaterial;
    [SerializeField] private Material signalGreenMaterial;

    private int activeSignal = 0;
    private Coroutine routine;

    public ForSignal StartStatus
    {
        get
        {
            return startStatus;
        }
    }

    public ForSignal EndStatus
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
            signalStatus = (ForSignal)value;
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
                case 1: Blink(1, signalYellowMaterial); break;
                case 2: Blink(1, signalYellowMaterial, 0, signalGreenMaterial); break;
                case 3: Blink(0, signalGreenMaterial); break;
                default: Debug.LogError("Not a valid sign number: " + i); break;
            }
        }
    }

    /**
     * This function starts the coroutine for showing the signalpatterns
     * 
     * @param       i               The light that is used in the pattern
     * @param       material1       The material that the light (i) is using
     * @param       j               The second light that is used in the pattern    (optional)
     * @param       material2       The material that the second light (j) is using (optional)
     */
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

    /**
     * This function is a coroutine and alternates between the material selected and the default off material set
     * in the inspector on this object. This blinks the light selected and will blink 60 times a minuite.
     * 
     * @param       i               The light that is used in the pattern
     * @param       material1       The material that the light (i) is using
     */
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

    /**
     * This function is a coroutine and alternates between the materials selected and the default off material set
     * in the inspector on this object. This blinks the two lights selected and will blink 60 times a minuite.
     * 
     * @param       i               The light that is used in the pattern
     * @param       material1       The material that the light (i) is using
     * @param       j               The second light that is used in the pattern
     * @param       material2       The material that the second light (j) is using
     */
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
