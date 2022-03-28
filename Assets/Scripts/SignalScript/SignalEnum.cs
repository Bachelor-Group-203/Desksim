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
        ForvendKjørRedusert,
        ForventKjør,
    }

    public enum HovedSignal
    {
        Av,
        Stop,
        StopBlink,
        KjørMedRedusertHastighet,
        Kjør
    }
}
