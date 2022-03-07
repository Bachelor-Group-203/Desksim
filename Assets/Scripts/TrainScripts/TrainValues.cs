using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is for specifying variables for a train to simulate a real life demo. 
 */
public class TrainValues : MonoBehaviour
{
    [Header("Mass of the Train in tonn")]
    [SerializeField] private float mass = 1000.0f;                  // default 1000 ton
    [Header("Max pulling force of the train in kN")]
    [SerializeField] private float maxPullingForce = 5.0f;          // default 5 m/s
    [Header("Breaking force of the train in kN")]
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
    public float MaxPullingForce
    {
        get
        {
            return maxPullingForce;
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
