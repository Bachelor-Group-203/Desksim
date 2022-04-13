using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SignalEnum;

/**
 * This class controlls the behaviour of all the signals for this senario
 */
public class SignalController : MonoBehaviour
{
    private List<Transform> listOfSignals = new List<Transform>();
    private List<int> listOfSignalType = new List<int>();

    /**
     * This function starts first when the object is compiled and initializes values
     */
    private void Awake()
    {
        foreach (Transform signal in transform)
        {
            listOfSignals.Add(signal);
            listOfSignalType.Add((int)signal.GetComponent<SignalScript>().SignalType);
        }

        InitalizeSignalState();
    }

    /**
     * This function runns per frame
     */
    private void Update()
    {
        // Checks if one of the lights in has been triggered
        for (int i = 0; i < listOfSignals.Count; i++)
        {
            int type = listOfSignalType[i];

            Transform signal = listOfSignals[i];

            // If the signal has a detection mode then test if the appropriate signal has been triggered
            if (signal.GetComponent<SignalScript>().TrainDetectionType != DetectTrain.Ingen)
            {
                switch (type)
                {
                    case 0:
                        if (signal.GetComponent<DvergScript>().TrainTrigger)
                        {
                            SenarioManager(signal, type);
                            signal.GetComponent<DvergScript>().TrainTrigger = false;
                        }
                        break;
                    case 1:
                        if (signal.GetComponent<ForSignalScript>().TrainTrigger)
                        {
                            SenarioManager(signal, type);
                            signal.GetComponent<ForSignalScript>().TrainTrigger = false;
                        }
                        break;
                    case 2:
                        if (signal.GetComponent<HovedSignalScript>().TrainTrigger)
                        {
                            SenarioManager(signal, type, i - 1);
                            signal.GetComponent<HovedSignalScript>().TrainTrigger = false;
                        }
                        break;
                    default: Debug.LogError("Not a valid sign number: " + type); break;
                }
            }
        }
    }

    /**
     * This function gives all the signals their starting signalpattern
     * (Here the "For Signal" signal mimicks the hovedsignal after it. 
     * That is how i asume it works but can also be manually set by changing the code)
     */
    private void InitalizeSignalState()
    {
        for (int i = 0; i < listOfSignals.Count; i++)
        {
            int type = listOfSignalType[i];

            Transform signal = listOfSignals[i];

            switch (type)
            {
                case 0: signal.GetComponent<DvergScript>().SignalStatus = (int)signal.GetComponent<DvergScript>().StartStatus; break;
                case 1: signal.GetComponent<ForSignalScript>().SignalStatus = (int)listOfSignals[i+1].GetComponent<HovedSignalScript>().StartStatus - 1; break;
                case 2: signal.GetComponent<HovedSignalScript>().SignalStatus = (int)signal.GetComponent<HovedSignalScript>().StartStatus; break;
                default: Debug.LogError("Not a valid sign number: " + type); break;
            }
        }
    }

    /**
     * This function changes the signalpattern of the signal 
     * 
     * @param       signal          The signal that should change
     * @param       type            The signal type 
     * @param       forIndex        The index of the signal before this object, if a forsignal infront of the hovedsignal 
     */
    private void SenarioManager(Transform signal, int type, int forIndex = -1)
    {
        int time = signal.GetComponent<SignalScript>().Timer;

        if (time <= 0)
        {
            switch (type)
            {
                case 0: signal.GetComponent<DvergScript>().SignalStatus = (int)signal.GetComponent<DvergScript>().EndStatus; break;
                case 1: signal.GetComponent<ForSignalScript>().SignalStatus = (int)signal.GetComponent<ForSignalScript>().EndStatus; break;
                case 2: 
                    if (forIndex < 0)
                        signal.GetComponent<HovedSignalScript>().SignalStatus = (int)signal.GetComponent<HovedSignalScript>().EndStatus;
                    else
                    {
                        signal.GetComponent<HovedSignalScript>().SignalStatus = (int)signal.GetComponent<HovedSignalScript>().EndStatus;
                        listOfSignals[forIndex].GetComponent<ForSignalScript>().SignalStatus = (int)signal.GetComponent<HovedSignalScript>().EndStatus - 1;
                    } 
                    break;
                default: Debug.LogError("Not a valid sign number: " + type); break;
            }
        }
        else
        {
            switch (type)
            {
                case 0: StartCoroutine(DvergChangeTime(signal, (int)signal.GetComponent<DvergScript>().EndStatus, time)); break;
                case 1: StartCoroutine(ForSignalChangeTime(signal, (int)signal.GetComponent<ForSignalScript>().EndStatus, time)); break;
                case 2: StartCoroutine(HovedSignalChangeTime(signal, (int)signal.GetComponent<HovedSignalScript>().EndStatus, time)); break;
                default: Debug.LogError("Not a valid sign number: " + type); break;
            }
        }
       
    }

    /**
     * Changes the signal state of the selected signal after some time for "Dverg Signal" signals
     * 
     * @param       signal          The signal that should change
     * @param       lightPattern    The signal pattern that the signal should change to
     * @param       time            The time before changing 
     */
    IEnumerator DvergChangeTime(Transform signal, int lightPattern, float time)
    {
        yield return new WaitForSeconds(time);
        signal.GetComponent<DvergScript>().SignalStatus = lightPattern;
    }

    /**
     * Changes the signal state of the selected signal after some time for "For Signal" signals
     * 
     * @param       signal          The signal that should change
     * @param       lightPattern    The signal pattern that the signal should change to
     * @param       time            The time before changing 
     */
    IEnumerator ForSignalChangeTime(Transform signal, int lightPattern, float time)
    {
        yield return new WaitForSeconds(time);
        signal.GetComponent<ForSignalScript>().SignalStatus = lightPattern;
    }

    /**
     * Changes the signal state of the selected signal after some time for "Hoved Signal" signals
     * 
     * @param       signal          The signal that should change
     * @param       lightPattern    The signal pattern that the signal should change to
     * @param       time            The time before changing 
     */
    IEnumerator HovedSignalChangeTime(Transform signal, int lightPattern, float time)
    {
        yield return new WaitForSeconds(time);
        signal.GetComponent<HovedSignalScript>().SignalStatus = lightPattern;
    }

}
