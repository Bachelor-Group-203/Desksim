using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    /**
     * The function handles detecting the train when interacting with the signal
     */
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Train")
        {
            transform.parent.GetComponent<SignalScript>().CollisionDetcted(this);
        }
    }
}
