using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

/**
 * FollowerWagon automatically assigns child objects to path and attaches them to wagons or cabs
 */
public class FollowerWagon : MonoBehaviour
{
    private Transform parent;

    /**
     * Called on the first frame this script is enabled
     */
    void Awake()
    {
        parent = this.transform.parent;
        
        for (int i = 0; i < transform.childCount; i++)
            assignWagons(i, i - 1);
    }

    /**
     * Assigns children objects with path and what is attached in front of them
     */
    private void assignWagons (int index, int target)
    {
        GameObject child = transform.GetChild(index).gameObject;

        // attaches their front wagon or cabs
        if (index == 0 && target == -1)
            child.GetComponent<Follower>().frontAttachment = parent.GetComponent<Follower>();
        else 
            child.GetComponent<Follower>().frontAttachment = transform.GetChild(target).GetComponent<Follower>();

        // assigns path
        child.GetComponent<Follower>().PathCreator = parent.GetComponent<Follower>().PathCreator;
    } 
}
