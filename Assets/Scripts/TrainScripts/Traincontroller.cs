using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traincontroller : MonoBehaviour
{

    private TrainValues tValues;
    private Rigidbody rigidbody;

    private void Awake()
    {
        tValues = GetComponent<TrainValues>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        /************************
         * Acceleration conroller
         ************************/
        // Input train controller here 
        // controller 0 - 100% * maxAcceleration = current acceleration 

        /******************
         * Break controller
         ******************/
        // Breaks go from 0 to 5 bar
        // controller 0 - 100% * maxBreakForce = current BreakForce
    }

    // FixedUpdate is called many times per frame
    private void FixedUpdate()
    {
        /***************************
         * Execute train acceleraton
         ***************************/
        rigidbody.AddForce(tValues.mass * tValues.maxAcceleration, 0, 0, ForceMode.Force);

        /**********************
         * Execute train breaks
         **********************/
        rigidbody.AddForce(-tValues.maxBreakForce, 0, 0, ForceMode.Force);
    }
}
