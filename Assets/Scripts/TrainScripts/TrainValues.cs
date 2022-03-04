using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is for specifying variables for a train to simulate a real life demo. 
 */
public class TrainValues : MonoBehaviour
{
    [Header("Mass of the Train in kg")]
    [SerializeField] private float mass = 1000.0f;                  // default 1000 ton
    [Header("Acceleration of the train in m/s")]
    [SerializeField] private float maxAcceleration = 5.0f;          // default 5 m/s
    [Header("Breaking force of the train in N")]
    [SerializeField] private float maxBreakForce = 50.0f;           // default 50 N
    [Header("Max velocity of the train in km/h")]
    [SerializeField] private float maxVelocity = 120.0f;            // default 120 km/h

    /*
     * Get method for mass
     */
    public float Mass
    {
        get
        {
            return mass;
        }
    }

    /*
     * Get method for maxAcceleration
     */
    public float MaxAcceleration
    {
        get
        {
            return maxAcceleration;
        }
    }

    /*
     * Get method for maxBreakForce
     */
    public float MaxBreakForce
    {
        get
        {
            return maxBreakForce;
        }
    }

    /*
     * Get method for maxVelocity and converts the valur to m/s
     */
    public float MaxVelocity
    {
        get
        {
            return (maxVelocity / 3.6f);
        }
    }
}
