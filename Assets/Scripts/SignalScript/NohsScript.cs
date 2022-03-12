using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NohsScript : MonoBehaviour
{
    [SerializeField] private NohsSignal signalStatus;
    [SerializeField] private Transform lights;
    [SerializeField] private Material signalOffMaterial;

    private List<Transform> listOfLights = new List<Transform>();

    enum NohsSignal
    {
        Av
    }

    private void Awake()
    {
        GetAllLights();
    }

    private void GetAllLights()
    {
        foreach (Transform light in lights)
        {
            light.GetComponent<MeshRenderer>().material = signalOffMaterial;
            listOfLights.Add(light);
        }
    }
}
