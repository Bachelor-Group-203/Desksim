using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class contains enums used to earier understand the selection of signaltype, signalstatus/signalpattern 
 * and mwthod of detecting the train. 
 */
public class SignalEnum : MonoBehaviour
{
    public enum DvergSignal
    {
        Av,
        SkiftingForbudt,
        VarsomSkifting,
        SkiftingTillat,
        FrigittSkifting
    }

    public enum ForSignal
    {
        Av,
        ForventStopp,
        ForventKj�rRedusert,
        ForventKj�r,
    }

    public enum HovedSignal
    {
        Av,
        StopBlink,
        Stop,
        Kj�rMedRedusertHastighet,
        Kj�r
    }

    public enum SignalType
    {
        Dverg,
        ForSignal,
        HovedSignal
    }

    public enum DetectTrain
    {
        Ingen,
        ForanSkilt,
        PasserSkilt
    }
}
