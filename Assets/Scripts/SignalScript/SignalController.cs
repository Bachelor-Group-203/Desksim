using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SignalEnum;

public class SignalController : MonoBehaviour
{
    private List<Transform> listOfSignals = new List<Transform>();

    private int layer = 0;

    private void Awake()
    {
        foreach (Transform signal in this.transform)
        {
            listOfSignals.Add(signal);
        }

        InitalizeSignalState();
    }

    private void Update()
    {
        foreach (Transform item in listOfSignals)
        {
            int type = (int)item.GetComponent<SignalScript>().SignalType;
            
            switch (type)
            {
                case 0: if (item.GetComponent<DvergScript>().TrainTrigger)
                    {
                        SenarioManager();
                        item.GetComponent<DvergScript>().TrainTrigger = false;
                    }
                    break;
                case 1: if (item.GetComponent<ForSignalScript>().TrainTrigger)
                    {
                        SenarioManager();
                        item.GetComponent<ForSignalScript>().TrainTrigger = false;
                    }
                    break;
                case 2: if (item.GetComponent<HovedSignalScript>().TrainTrigger)
                    {
                        SenarioManager();
                        item.GetComponent<HovedSignalScript>().TrainTrigger = false;
                    }
                    break;
                default: Debug.LogError("Not a valid sign number: " + type); break;
            }
        }
    }

    /**
     * Describes the initial values of the signals and has to be manualy edited
     */
    private void InitalizeSignalState()
    {
        // initial signals
        ChangeSignalStatus(listOfSignals[0], (int)HovedSignal.Stop);
        ChangeSignalStatus(listOfSignals[1], (int)HovedSignal.KjørMedRedusertHastighet);
        ChangeSignalStatus(listOfSignals[2], (int)HovedSignal.Stop);
    }

    /**
     * Describes what the signal is supposed to do after getting triggered and has to be manualy edited
     */
    private void SenarioManager()
    {
        if (layer == 2)
        {
            ChangeSignalStatusTime(listOfSignals[1], (int)HovedSignal.StopBlink, 2.0f);
            layer++;
        }
        if (layer == 1)
        {
            ChangeSignalStatusTime(listOfSignals[1], (int)HovedSignal.StopBlink, 2.0f);
            layer++;
        }
        if (layer == 0)
        {
            ChangeSignalStatusTime(listOfSignals[0], (int)HovedSignal.Kjør, 2.0f);
            layer++;
        }
    }

    /**
     * Changes the signal state of the selected signal
     * 
     * @param       signal          The signal that should change
     * @param       lightPattern    The signal pattern that the signal should change to
     */
    private void ChangeSignalStatus(Transform signal, int lightPattern)
    {
        int type = (int)signal.GetComponent<SignalScript>().SignalType;

        switch (type)
        {
            case 0: signal.GetComponent<DvergScript>().SignalStatus = lightPattern; break;
            case 1: signal.GetComponent<ForSignalScript>().SignalStatus = lightPattern; break;
            case 2: signal.GetComponent<HovedSignalScript>().SignalStatus = lightPattern; break;
            default: Debug.LogError("Not a valid sign number: " + type); break;
        }
    }

    /**
     * Changes the signal state of the selected signal after some time
     * 
     * @param       signal          The signal that should change
     * @param       lightPattern    The signal pattern that the signal should change to
     * @param       time            The time before changing 
     */
    private void ChangeSignalStatusTime(Transform signal, int lightPattern, float time)
    {
        int type = (int)signal.GetComponent<SignalScript>().SignalType;

        switch (type)
        {
            case 0: StartCoroutine(DvergChangeTime(signal, lightPattern, time)); break;
            case 1: StartCoroutine(ForSignalChangeTime(signal, lightPattern, time)); break;
            case 2: StartCoroutine(HovedSignalChangeTime(signal, lightPattern, time)); break;
            default: Debug.LogError("Not a valid sign number: " + type); break;
        }
    }

    /**
     * Changes the signal state of the selected signal after some time for dverg signals
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
     * Changes the signal state of the selected signal after some time for for signals
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
     * Changes the signal state of the selected signal after some time for hoved signals
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
