using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    private const float END_LABEL_ANGLE = -50;
    private const float START_LABEL_ANGLE = 230;

    [SerializeField] private Transform velocityNeedleTransform;
    [SerializeField] private Transform pressureNeedleTransform;
    [SerializeField] private Transform labelTransform;

    private Traincontroller trainController;
    private TrainValues trainValues;

    private int maxViewVelocity;
    private int maxViewPressure;
    private float totalAngleSize;

    private void Awake()
    {
        trainController = GetComponent<Traincontroller>();
        trainValues = GetComponent<TrainValues>();

        totalAngleSize = START_LABEL_ANGLE - END_LABEL_ANGLE;

        // Creates the labels for the Speedometer
        maxViewVelocity = ((int)((trainValues.maxVelocity * 1.1f) / 20f)) * 20;
        CreateLabels(maxViewVelocity / 20, maxViewVelocity, velocityNeedleTransform.parent);
        velocityNeedleTransform.SetAsLastSibling();

        // Creates the labels for the Barometer
        maxViewPressure = 8;
        CreateLabels(maxViewPressure, maxViewPressure, pressureNeedleTransform.parent);
        pressureNeedleTransform.SetAsLastSibling();
    }

    private void Update()
    {
        // Rotates the velocity needle
        velocityNeedleTransform.eulerAngles = new Vector3(0, 0, GetValueToAngle(trainController.Velocity, maxViewVelocity / 3.6f));

        // Rotates the pressure needle
        pressureNeedleTransform.eulerAngles = new Vector3(0, 0, GetValueToAngle(trainController.Pressure, maxViewPressure));
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
     * @return          Returns the angle to rotate the object
     */
    private void CreateLabels(int labelAmount, float maxLabelValue, Transform parent)
    {
        float totalAngleSize = START_LABEL_ANGLE - END_LABEL_ANGLE;

        for (int i = 0; i <= labelAmount; i++)
        {
            Transform label = Instantiate(labelTransform, parent);
            float labelNormalized = (float)i / labelAmount;
            float labelAngle = START_LABEL_ANGLE - labelNormalized * totalAngleSize;
            label.eulerAngles = new Vector3(0, 0, labelAngle);
            label.GetComponentInChildren<Text>().text = Mathf.RoundToInt(labelNormalized * maxLabelValue).ToString();
            label.Find("Text").eulerAngles = Vector3.zero;
            label.gameObject.SetActive(true);
        }
    }

}
