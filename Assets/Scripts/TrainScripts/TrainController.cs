using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * This class is for controlling the train and contains all the logic for how the train moves
 */
public class TrainController : MonoBehaviour
{

    [SerializeField] private LayerMask railLayer;

    private TrainValues tValues;
    private Rigidbody rBody;
    private TrainInput input;
    private TrainUi tUi;


    // TODO!!! Find out how to get the train head direction
    private Vector3 tDirection = new Vector3(1, 0, 0);

    private float force = 0;
    private float vel = 0;
    private float pressure = 0;
    private float slope = 0;

    public float Slope
    {
        get
        {
            return slope;
        }
    }

    public float Velocity
    {
        get
        {
            return vel;
        }
    }

    public float Pressure
    {
        get
        {
            return pressure;
        }
    }

    /**
     * Awake is called first when the object is instantiated
     */
    private void Awake()
    {
        tValues = GetComponent<TrainValues>();
        rBody = GetComponent<Rigidbody>();
        tUi = GetComponent<TrainUi>();
        if (GameObject.FindGameObjectWithTag("InputScripts"))
        {
            input = GameObject.FindGameObjectWithTag("InputScripts").GetComponent<TrainInput>();
        }
        else
        {
            Debug.LogWarning("!!! InputScripts game object not found !!!");
        }
    }

    /**
     * Update is called once per frame
     */
    void Update()
    {
        // Acceleration force
        vel = Vector3.Magnitude(rBody.velocity) * force;

        // Breaking force
        UpdatePressure();

        // Reverse train
        if (Mathf.Abs(vel) <= 0)
        {
            if (tUi.Reverse)
            {
                force = -1;
            }
            else
            {
                force = 1;
            }
        }
    }

    /**
     * FixedUpdate is called many times per frame
     */
    private void FixedUpdate()
    {
        // Train Acceleration
        if (pressure >= 5.0f)
        {
            if (Mathf.Abs(vel) >= tValues.MaxVelocity)
            {
                rBody.velocity = tDirection.normalized * tValues.MaxVelocity;
                rBody.AddForce(0, 0, 0);
            }
            else
            {
                rBody.AddForce(force * GetAccelerationForce(), 0, 0);
            }
        }

        // Train brakes
        if (pressure <= 4.5f)
        {
            if (Mathf.Abs(vel) <= 0.01f)
            {
                rBody.velocity = Vector3.zero;
                rBody.AddForce(0, 0, 0);
            }
            else
            {
                rBody.AddForce(force * GetBreakForce(), 0, 0);
            }
        }
    }

    /**
     * Calculates the force needed to stop the train as kinetic energy over time
     * 
     * @return                      Returns the break force
     */
    private float GetBreakForce()
    {
        return -tValues.MaxPullingForce * (1 - (pressure / 5)) / tValues.Mass;
    }

    /**
     * Calculates the acceleration force to be applied on the train
     * 
     * @return                      Returns the acceleration force
     */
    private float GetAccelerationForce()
    {
        return tValues.MaxPullingForce * input.acceleration / tValues.Mass;
    }

    /**
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

    /**
     * Finds the slope-angle by finding the diffrence between the up vector and the plane normal
     * 
     * @return                      Retruns the angle of the slope if it can find the layer, if not retirns -1
     */
    private float GetGroundAngle()
    {
        RaycastHit hit;
        float groundAngle = -1.0f;

        // Generate a ray that pints down
        Ray ray = new Ray(transform.position, -transform.up);

        // If the Ray collides with an object in the layer specified
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 50.0f, railLayer))
        {
            // Finds the angle between the ground normal and the up vector
            groundAngle = Vector3.Angle(Vector3.up, hit.normal);
        }
        return groundAngle;
    }
}
