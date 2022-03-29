using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    /*
    [SerializeField]
    public GameObject cam;

    private Vector3 cameraPositions;

    // Start is called before the first frame update
    void Start()
    {

        
        
    }

    /**
     * 
     * 
     * @param lerpSpeed     Duration for camera to move
     *
    IEnumerator LerpToPosition(float lerpSpeed, Vector3 newPosition, bool useRelativeSpeed = false)
    {
        if (useRelativeSpeed)
        {
            float totalDistance = farRight.position.x - farLeft.position.x;
            float diff = transform.position.x - farLeft.position.x;
            float multiplier = diff / totalDistance;
            lerpSpeed *= multiplier;
        }

        float t = 0.0f;
        Vector3 startingPos = transform.position;
        while (t < 1.0f)
        {
            t += Time.deltaTime * (Time.timeScale / lerpSpeed);

            transform.position = Vector3.Lerp(startingPos, newPosition, t);
            yield return 0;
        }
    }

    public void FirstPerson()
    {
        StartCoroutine(LerpToPosition(4, .position, true));
    }

    public void ThirdPerson()
    {

    }

    public void FocusOnPanel()
    {

    }
    */


}
