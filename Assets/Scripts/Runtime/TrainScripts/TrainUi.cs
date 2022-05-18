using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * This class contains all the HUD elements and visual displays that relays information to the user.
 */
public class TrainUi : MonoBehaviour
{
    private const float END_LABEL_ANGLE = -50;
    private const float START_LABEL_ANGLE = 230;

    [Header("UI Objects")]
    [SerializeField] private bool disableLabelGeneration = false;
    [Header("Velocity and pressure")]
    [SerializeField] private Transform velocityNeedleTransform;
    [SerializeField] private Transform pressureNeedleTransform;
    [SerializeField] private Transform labelTransform;
    [Header("Reverse Button")]
    [SerializeField] private GameObject reverseBackground;
    [SerializeField] private GameObject reverseBackground2;
    [SerializeField] private Color reverseOffCollor;
    [SerializeField] private Color reverseOnCollor;


    private TrainController trainController;
    private TrainValues trainValues;

    private int maxViewVelocity;
    private int maxViewPressure;
    private float totalAngleSize;

    private bool reverse = false;

    public bool Reverse
    {
        get
        {
            return reverse;
        }
    }

    /**
     * Awake is called first when the object is instantiated
     */
    private void Awake()
    {
        // Gets the refrence to components needed
        trainController = GetComponent<TrainController>();
        trainValues = GetComponent<TrainValues>();


        maxViewVelocity = ((int)((trainValues.MaxVelocity * 3.6 * 1.1f) / 20f)) * 20;
        maxViewPressure = 8;


        // Finds the total angle to use on the gauge
        totalAngleSize = START_LABEL_ANGLE - END_LABEL_ANGLE;

        if (disableLabelGeneration) return;

        //// Creates the labels for the Speedometer ////
        // Creates labels inside gameObject named "Labels" if found
        if (velocityNeedleTransform.parent.Find("Labels")) {
            // Create in container gameObject
            CreateLabels(maxViewVelocity / 20, maxViewVelocity, velocityNeedleTransform.parent.Find("Labels"), 180);
            velocityNeedleTransform.parent.Find("Labels").transform.localScale = new Vector3(-1f, 1f, 1f);
            velocityNeedleTransform.parent.Find("Labels").transform.localEulerAngles = new Vector3(0f, 90f, 0f);
        } else {
            // Create in parent root
            CreateLabels(maxViewVelocity / 20, maxViewVelocity, velocityNeedleTransform.parent, 0); 
        }
        velocityNeedleTransform.SetAsLastSibling();

        //// Creates the labels for the Barometer ////
        // Creates labels inside gameObject named "Labels" if found
        if (pressureNeedleTransform.parent.Find("Labels")) {
            // Create in container gameObject
            CreateLabels(maxViewPressure, maxViewPressure, pressureNeedleTransform.parent.Find("Labels"), 180);
            pressureNeedleTransform.parent.Find("Labels").transform.localScale = new Vector3(-1f, 1f, 1f);
            pressureNeedleTransform.parent.Find("Labels").transform.localEulerAngles = new Vector3(0f, 90f, 0f);
        } else {
            // Create in parent root
            CreateLabels(maxViewPressure, maxViewPressure, pressureNeedleTransform.parent, 0);
        }
        pressureNeedleTransform.SetAsLastSibling();
    }

    /**
     * Update is called once per frame
     */
    private void Update()
    {
        // Rotates the velocity needle
        velocityNeedleTransform.localEulerAngles = new Vector3(0, 0, GetValueToAngle(Mathf.Abs(trainController.Velocity), maxViewVelocity / 3.6f));

        // Rotates the pressure needle
        pressureNeedleTransform.localEulerAngles = new Vector3(0, 0, GetValueToAngle(trainController.Pressure, maxViewPressure));
    }

    /**
     * This converts the velocity from the train into degrees for rotating the needle
     * 
     * @param       currentValue    The value that the needel should point to
     * @param       maxValue        The value that the can maximum point to
     * @return                      Returns the angle to rotate the object
     */
    private float GetValueToAngle(float currentValue, float maxValue)
    {
        float valueNormalized = currentValue / maxValue; 

        return START_LABEL_ANGLE - valueNormalized * totalAngleSize;
    }

    /**
     * This converts the velocity from the train into degrees for rotating the needle
     * 
     * @param       labelAmount     The amount of labels on the gauge
     * @param       maxLabelValue   The highest value shown on the gauge
     * @param       parent          The parent transform that contains the instatiated objects
     * @return                      Returns the angle to rotate the object
     */
    private void CreateLabels(int labelAmount, float maxLabelValue, Transform parent, float offset)
    {
        for (int i = 0; i <= labelAmount; i++)
        {
            // Instatiate new label object
            Transform label = Instantiate(labelTransform, parent);
            
            // Find angle of the label and rotate it accordingly 
            float labelNormalized = (float)i / labelAmount;
            float labelAngle = START_LABEL_ANGLE - labelNormalized * totalAngleSize;
            label.eulerAngles = new Vector3(0, 0, labelAngle + offset);

            // Set the text to the apropriate number 
            label.GetComponentInChildren<Text>().text = Mathf.RoundToInt(labelNormalized * maxLabelValue).ToString();
            // Makes the numer not rotate with the tranform
            label.Find("Text").eulerAngles = Vector3.zero;
            // Activate the label so it is visible
            label.gameObject.SetActive(true);
        }
    }

    /**
     * This function swithes the moving direction of the train and the color of the button.
     * Called by Unity event under UI -> ReverseButton in hierarchy
     */
    public void EngageReverse()
    {
        if (reverse)
        {
            reverse = false;
            if(reverseBackground)  reverseBackground.GetComponent<Image>().color  = reverseOffCollor;
            if(reverseBackground2) reverseBackground2.GetComponent<Image>().color = reverseOffCollor;
        }
        else
        {
            reverse = true;
            if(reverseBackground)  reverseBackground.GetComponent<Image>().color  = reverseOnCollor;
            if(reverseBackground2) reverseBackground2.GetComponent<Image>().color = reverseOnCollor;
        }
    }

}
