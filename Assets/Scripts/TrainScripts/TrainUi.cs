using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class contains all the HUD elements and visual displays that relays information to the user.
 */
public class TrainUi : MonoBehaviour
{
    private const float END_LABEL_ANGLE = -50;
    private const float START_LABEL_ANGLE = 230;

    [Header("UI Objects")]
    [Header("Velocity and pressure")]
    [SerializeField] private Transform velocityNeedleTransform;
    [SerializeField] private Transform pressureNeedleTransform;
    [SerializeField] private Transform labelTransform;
    [Header("Reverse Button")]
    [SerializeField] private GameObject reverseBackground;
    [SerializeField] private Color reverseOffCollor;
    [SerializeField] private Color reverseOnCollor;
    [Header("Slpoe Finder")]
    [SerializeField] private Text angleDisplay;

    private TrainController trainController;
    private TrainValues trainValues;

    private int maxViewVelocity;
    private int maxViewPressure;
    private float totalAngleSize;

    private bool reverse = false;

    /*
     * Get method for reverse
     */
    public bool Reverse
    {
        get
        {
            return reverse;
        }
    }

    /*
     * Awake is called first when the object is instantiated
     */
    private void Awake()
    {
        // Gets the refrence to components needed
        trainController = GetComponent<TrainController>();
        trainValues = GetComponent<TrainValues>();

        // Finds the total angle to use on the gauge
        totalAngleSize = START_LABEL_ANGLE - END_LABEL_ANGLE;

        // Creates the labels for the Speedometer
        maxViewVelocity = ((int)((trainValues.MaxVelocity * 3.6 * 1.1f) / 20f)) * 20;
        CreateLabels(maxViewVelocity / 20, maxViewVelocity, velocityNeedleTransform.parent);
        velocityNeedleTransform.SetAsLastSibling();

        // Creates the labels for the Barometer
        maxViewPressure = 8;
        CreateLabels(maxViewPressure, maxViewPressure, pressureNeedleTransform.parent);
        pressureNeedleTransform.SetAsLastSibling();
    }

    /*
     * Update is called once per frame
     */
    private void Update()
    {
        // Rotates the velocity needle
        velocityNeedleTransform.eulerAngles = new Vector3(0, 0, GetValueToAngle(Mathf.Abs(trainController.Velocity), maxViewVelocity / 3.6f));

        // Rotates the pressure needle
        pressureNeedleTransform.eulerAngles = new Vector3(0, 0, GetValueToAngle(trainController.Pressure, maxViewPressure));

        // Displays the slope angle
        angleDisplay.text = trainController.Slope.ToString();
    }

    /*
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

    /*
     * This converts the velocity from the train into degrees for rotating the needle
     * 
     * @param       labelAmount     The amount of labels on the gauge
     * @param       maxLabelValue   The highest value shown on the gauge
     * @param       parent          The parent transform that contains the instatiated objects
     * @return                      Returns the angle to rotate the object
     */
    private void CreateLabels(int labelAmount, float maxLabelValue, Transform parent)
    {
        for (int i = 0; i <= labelAmount; i++)
        {
            // Instatiate new label object
            Transform label = Instantiate(labelTransform, parent);
            
            // Find angle of the label and rotate it accordingly 
            float labelNormalized = (float)i / labelAmount;
            float labelAngle = START_LABEL_ANGLE - labelNormalized * totalAngleSize;
            label.eulerAngles = new Vector3(0, 0, labelAngle);

            // Set the text to the apropriate number 
            label.GetComponentInChildren<Text>().text = Mathf.RoundToInt(labelNormalized * maxLabelValue).ToString();
            // Makes the numer not rotate with the tranform
            label.Find("Text").eulerAngles = Vector3.zero;
            // Activate the label so it is visible
            label.gameObject.SetActive(true);
        }
    }

    /*
     * This function swithes the moving direction of the train and the color of the button.
     * Called by Unity event under UI -> ReverseButton in hierarchy
     */
    public void EngageReverse()
    {
        if (reverse)
        {
            reverse = false;
            reverseBackground.GetComponent<Image>().color = reverseOffCollor;
        }
        else
        {
            reverse = true;
            reverseBackground.GetComponent<Image>().color = reverseOnCollor;
        }
    }

}
