using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * This class is for controlling the train and contains all the logic for how the train moves
 */
public class Traincontroller : MonoBehaviour
{

    private TrainValues tValues;
    private Rigidbody rBody;
    private UserInputController input;


    // TODO!!! Find out how to get the train head direction
    private Vector3 tDirection = new Vector3(1, 0, 0);

    private float velocity = 0;
    private float pressure = 0;

    /*
     * Get method for velocity
     */
    public float Velocity
    {
        get
        {
            return velocity;
        }
    }

    /*
     * Get method for pressure
     */
    public float Pressure
    {
        get
        {
            return pressure;
        }
    }

    /*
     * Awake is called first when the object is instantiated
     */
    private void Awake()
    {
        tValues = GetComponent<TrainValues>();
        rBody = GetComponent<Rigidbody>();
        input = GetComponent<UserInputController>();
        //inputController = GetComponent<UserInputController>();
    }

    /*
     * Update is called once per frame
     */
    void Update()
    {
        /************************
         * Acceleration conroller
         ************************/
        // Input train controller here 
        // controller 0 - 100% * maxAcceleration = current acceleration 
        velocity = Vector3.Magnitude(rBody.velocity);

        /******************
         * Break controller
         ******************/
        // Breaks go from 0 to 5 bar
        // controller 0 - 100% * maxBreakForce = current BreakForce
        UpdatePressure();

        /******************
         * Notes and uefull things
         ******************/
        // For tracking the speed of the train in km/h
        //Debug.Log(Vector3.Magnitude(rigidbody.velocity) * 3.6 + " km/h");
        //Debug.Log("\tBar: " + bar);
        //Debug.Log("Acceleration: " + GetAccelerationForce() + "\tVelocity: " + Vector3.Magnitude(rBody.velocity) + "\tBar: " + pressure);

    }

    /*
     * FixedUpdate is called many times per frame
     */
    private void FixedUpdate()
    {
        /***************************
         * Execute train acceleraton
         ***************************/
        if (velocity >= tValues.MaxVelocity)
        {
            rBody.velocity = tDirection.normalized * tValues.MaxVelocity;
            rBody.AddForce(0, 0, 0);
        }
        else if (pressure >= 5.0f)
        {
            rBody.AddForce(GetAccelerationForce(), 0, 0);
        }

        /**********************
         * Execute train breaks
         **********************/
        if (pressure <= 4.5f)
        {
            rBody.AddForce(GetBreakForce(), 0, 0);
        }

    }

    /*
     * Calculates the force needed to stop the train as kinetic energy over time
     * 
     * TODO!!! Maybe fix so that it is not dependent on time but on stick position. little force when close to 0
     */
    private float GetBreakForce()
    {
        return (0.5f * tValues.Mass * (Mathf.Pow(velocity, 2))) / -50;
    }

    /*
     * Calculates the acceleration force to be applied on the train
     * 
     * @return                      Returns the acceleration force
     */
    private float GetAccelerationForce()
    {
        return tValues.MaxAcceleration * input.acceleration * tValues.Mass;
    }

    /*
     * Adds or subtracts pressurevalues based on the input.
     */
    private void UpdatePressure()
    {
        if (input.pressure <= 0 && pressure >= 0)
        {
            pressure -= 0.007f;
        }
        else if (input.pressure > 0 && pressure < 5.0f)
        {
            pressure += 0.005f * input.pressure;
        }
    }

}
