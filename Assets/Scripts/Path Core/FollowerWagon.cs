using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class FollowerWagon : MonoBehaviour
{
    private Transform parent;
    void Start()
    {
        parent = this.transform.parent;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            assignWagons(i, i - 1);
        }
    }

    private void assignWagons (int index, int target)
    {
        GameObject child = transform.GetChild(index).gameObject;
        if (index == 0 && target == -1)
        {
            child.GetComponent<Follower>().frontAttachment = parent.GetComponent<Follower>();
        }
        else 
        {
            child.GetComponent<Follower>().frontAttachment = transform.GetChild(target).GetComponent<Follower>();
        }
        child.GetComponent<Follower>().PathCreator = parent.GetComponent<Follower>().PathCreator;
    } 
}
