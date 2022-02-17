using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Traincontroller : MonoBehaviour
{

    private TrainValues tValues;
    private Rigidbody rBody;
    private UserInputController inputController;


    // TODO!!! Find out how to get the train head direction
    private Vector3 tDirection = new Vector3(1, 0, 0);

    private void Awake()
    {
        tValues = GetComponent<TrainValues>();
        rBody = GetComponent<Rigidbody>();
        inputController = GetComponent<UserInputController>();
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

        /******************
         * Notes and uefull things
         ******************/

        // For tracking the speed of the train in km/h
        //Debug.Log(Vector3.Magnitude(rigidbody.velocity) * 3.6 + " km/h");
        //Debug.Log(inputController.acceleration);

    }

    // FixedUpdate is called many times per frame
    private void FixedUpdate()
    {
        /***************************
         * Execute train acceleraton
         ***************************/
        if (Vector3.Magnitude(rBody.velocity) >= tValues.GetMaxVelocityAsMS())
        {
            //Debug.LogWarning("Train exceeds the speedlimit!!!");
            rBody.velocity = tDirection.normalized * tValues.GetMaxVelocityAsMS();
            rBody.AddForce(0, 0, 0);
        }
        else
        {
            rBody.AddForce(tValues.maxAcceleration, 0, 0);
        }

        /**********************
         * Execute train breaks
         **********************/
        //rigidbody.AddForce(-tValues.maxBreakForce, 0, 0, ForceMode.Force);

    }

    public void Acceleration()
    {
        Debug.Log("Value");
    }
}
