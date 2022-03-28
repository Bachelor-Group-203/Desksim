using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalController : MonoBehaviour
{
    
    private List<Transform> listOfSignals = new List<Transform>();

    enum SignalType
    {
        Dverg,
        ForSignal,
        HovedSignal
    }

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
            case 0: StartCoroutine(ChangeDvergTime(signal, lightPattern, time)); break;
            case 1: StartCoroutine(ChangeForSignalTime(signal, lightPattern, time)); break;
            case 2: StartCoroutine(ChangeHovedSignalTime(signal, lightPattern, time)); break;
            default: Debug.LogError("Not a valid sign number: " + (int)signalType); break;
        }
    }

    IEnumerator ChangeDvergTime(Transform signal, int lightPattern, float time)
    {
        yield return new WaitForSeconds(time);
        signal.GetComponent<DvergScript>().SignalStatus = lightPattern;
    }

    IEnumerator ChangeForSignalTime(Transform signal, int lightPattern, float time)
    {
        yield return new WaitForSeconds(time);
        signal.GetComponent<ForSignalScript>().SignalStatus = lightPattern;
    }

    IEnumerator ChangeHovedSignalTime(Transform signal, int lightPattern, float time)
    {
        yield return new WaitForSeconds(time);
        signal.GetComponent<HovedSignalScript>().SignalStatus = lightPattern;
    }

}
