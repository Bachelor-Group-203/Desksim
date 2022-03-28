using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        ForvendKj�rRedusert,
        ForventKj�r,
    }

    public enum HovedSignal
    {
        Av,
        Stop,
        StopBlink,
        Kj�rMedRedusertHastighet,
        Kj�r
    }
}
