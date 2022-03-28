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
        foreach (Transform signal in transform)
        {
            listOfSignals.Add(signal);
        }
    }

    private void Start()
    {
        
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
                        test();
                        item.GetComponent<DvergScript>().TrainTrigger = false;
                    }
                    break;
                case 1: if (item.GetComponent<ForSignalScript>().TrainTrigger)
                    {
                        test();
                        item.GetComponent<ForSignalScript>().TrainTrigger = false;
                    }
                    break;
                case 2: if (item.GetComponent<HovedSignalScript>().TrainTrigger)
                    {
                        test();
                        item.GetComponent<HovedSignalScript>().TrainTrigger = false;
                    }
                    break;
                default: Debug.LogError("Not a valid sign number: " + type); break;
            }
        }
    }

    private void InitalizeSignalState()
    {
        // initial signals
        ChangeSignalStatus(listOfSignals[0], SignalType.HovedSignal, (int)HovedSignal.Stop);
        ChangeSignalStatus(listOfSignals[1], SignalType.HovedSignal, (int)HovedSignal.KjørMedRedusertHastighet);
        ChangeSignalStatus(listOfSignals[2], SignalType.HovedSignal, (int)HovedSignal.Stop);
    }


    private void test()
    {
        if (layer == 0)
        {
            ChangeSignalStatusTime(listOfSignals[0], SignalType.HovedSignal, (int)HovedSignal.Kjør, 10.0f);
            layer++;
        }
        if (layer == 1)
        {
            ChangeSignalStatusTime(listOfSignals[0], SignalType.HovedSignal, (int)HovedSignal.Kjør, 10.0f);
            layer++;
        }
    }

    private bool TrainIsHere()
    {
        return true;
    }

    /*
     * Changes the signal state of the selected signal
     * 
     * @param       signal          The signal that should change
     * @param       signalType      The signals type
     * @param       lightPattern    The signal pattern that the signal should change to
     */
    private void ChangeSignalStatus(Transform signal, SignalType signalType, int lightPattern)
    {
        switch ((int)signalType)
        {
            case 0: signal.GetComponent<DvergScript>().SignalStatus = lightPattern; break;
            case 1: signal.GetComponent<ForSignalScript>().SignalStatus = lightPattern; break;
            case 2: signal.GetComponent<HovedSignalScript>().SignalStatus = lightPattern; break;
            default: Debug.LogError("Not a valid sign number: " + (int)signalType); break;
        }
    }

    /*
     * Changes the signal state of the selected signal after some time
     * 
     * @param       signal          The signal that should change
     * @param       signalType      The signals type
     * @param       lightPattern    The signal pattern that the signal should change to
     * @param       time            The time before changing 
     */
    private void ChangeSignalStatusTime(Transform signal, SignalType signalType, int lightPattern, float time)
    {
        switch ((int)signalType)
        {
            case 0: StartCoroutine(DvergChangeTime(signal, lightPattern, time)); break;
            case 1: StartCoroutine(ForSignalChangeTime(signal, lightPattern, time)); break;
            case 2: StartCoroutine(HovedSignalChangeTime(signal, lightPattern, time)); break;
            default: Debug.LogError("Not a valid sign number: " + (int)signalType); break;
        }
    }

    IEnumerator DvergChangeTime(Transform signal, int lightPattern, float time)
    {
        yield return new WaitForSeconds(time);
        signal.GetComponent<DvergScript>().SignalStatus = lightPattern;
    }

    IEnumerator ForSignalChangeTime(Transform signal, int lightPattern, float time)
    {
        yield return new WaitForSeconds(time);
        signal.GetComponent<ForSignalScript>().SignalStatus = lightPattern;
    }

    IEnumerator HovedSignalChangeTime(Transform signal, int lightPattern, float time)
    {
        yield return new WaitForSeconds(time);
        signal.GetComponent<HovedSignalScript>().SignalStatus = lightPattern;
    }

}
