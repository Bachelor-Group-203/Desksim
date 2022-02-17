using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInputController : MonoBehaviour
{

    /***************\
    | Static values |
    \***************/
    //private static float pressureMax     = 10; // bar https://en.wikipedia.org/wiki/Railway_air_brake#Working_pressures
    //private static float accelerationMax = 1;  // 0-1 %
    private static float pressureModificationMagnitude     = 0.02f; // What the joystick value will be multiplied with before being added to the end-value
    private static float accelerationModificationMagnitude = 0.01f; // What the joystick value will be multiplied with before being added to the end-value

    /*************\
    | Core values |
    \*************/
    private float pressure;     // 0 -> 1, apply to curve in train controller or in separate value
    public float acceleration; // 0 -> 1

    /***************\
    | Helper values |
    \***************/
    private float pressureAbsolute_value;
    private Vector2 pressureModifier_value;
    private float accelerationAbsolute_value;
    private Vector2 accelerationModifier_value;

    /********************\
    | Input action asset |
    \********************/
    private UserInputActions userInputActions; // Not static or global, this is this scripts own instance of the input action asset

    /*********************\
    | TRAIN input actions |
    \*********************/
    private InputActionMap userInputActionMap_Train;
    // Pressure
    private InputAction pressureAbsolute; // For compatible joysticks, use when available
    private InputAction pressureModifier; // For alternative inputs, keyboard & gamepads
    // TODO make pressure controllable with in-game slider too
    // Acceleration
    private InputAction accelerationAbsolute; // For compatible joysticks, use when available
    private InputAction accelerationModifier; // For alternative inputs, keyboard & gamepads
    // TODO make acceleration controllable with in-game slider too


    /**
     * Called when object is instantiated (?), instantiates user input action asset
     **/
    private void Awake()
    {
        // Creating a new instance of our input action asset
        userInputActions = new UserInputActions();
    }


    /**
     * Called when object is enabled, enables and binds action inputs.
     **/
    private void OnEnable()
    {
        Debug.Log("InputController: OnEnable called");
        // Caching input actions to local variables
        pressureAbsolute = userInputActions.Train.PressureAbsolute;
        pressureModifier = userInputActions.Train.PressureModifier;
        accelerationAbsolute = userInputActions.Train.AccelerationAbsolute;
        accelerationModifier = userInputActions.Train.AccelerationModifier;

        // Subscribing input actions "performed" to functions
        userInputActions.Train.Menu.performed += Pause;
        userInputActions.Train.FocusPanel.performed += Train_FocusPanel;

        // Enabling the action inputs
        pressureAbsolute.Enable();
        pressureModifier.Enable();
        accelerationAbsolute.Enable();
        accelerationModifier.Enable();
        userInputActions.Train.Menu.Enable();
        userInputActions.Train.FocusPanel.Enable();


    }


    /**
     * Called when object is disabled, disables all input actions.
     **/
    private void OnDisable()
    {
        Debug.Log("InputController: OnEnable called");
        // Enabling the action inputs, so they won't call
        pressureAbsolute.Disable();
        pressureModifier.Disable();
        accelerationAbsolute.Disable();
        accelerationModifier.Disable();
        userInputActions.Train.Menu.Disable();
        userInputActions.Train.FocusPanel.Disable();
    }


    /**
     * Called every *#!%(!#&#)
     **/
    private void FixedUpdate()
    {

        // All controller vector2 values are assumed to be from -1 to 1, or 0 to 1 for joysticks, this might vary from device to device, hopefully not. Can be checked in window->analysis->input debugger

        /**********\
        | PRESSURE |
        \**********/
        // Check if absolute pressure input is available, if a compatible joystick is being used.
        // Should be bound in settings to ensure correct functionality, maybe a bool if custom joystick has been set in settings.
        pressureAbsolute_value = pressureAbsolute.ReadValue<float>();
        pressureModifier_value = pressureModifier.ReadValue<Vector2>();
        if (pressureAbsolute_value != 0) {
            // Absolute pressure input is available
            pressure = (pressureAbsolute_value+1)/2; // Axises go from -1 to 1, changes it to 0 -> 1
            Debug.Log("Input: pressureAbsolute value = " + pressureAbsolute_value + " -> " + pressure);

        }
        else if (pressureModifier_value.y != 0) {

            // Absolute pressure input not available, using modifier input
            pressure += pressureModifier_value.y * pressureModificationMagnitude;
            pressure = Mathf.Clamp(pressure, 0.0f, 1.0f);
            Debug.Log("Input: pressureAbsolute value = " + pressureModifier_value + " == " + pressure);
        }

        /**************\
        | ACCELERATION |
        \**************/
        // Check if absolute acceleration input is available, if a compatible joystick is being used.
        // Should be bound in settings to ensure correct functionality, maybe a bool if custom joystick has been set in settings.
        accelerationAbsolute_value = accelerationAbsolute.ReadValue<float>();
        accelerationModifier_value = accelerationModifier.ReadValue<Vector2>();
        if (accelerationAbsolute_value != 0) {
            acceleration = accelerationAbsolute_value;
            // Absolute acceleration input is available
            acceleration = (accelerationAbsolute_value+1)/2;
            Debug.Log("Input: accelerationAbsolute value = " + accelerationAbsolute_value + " -> " + acceleration);

        } else if(accelerationModifier_value.y != 0) {

            // Absolute acceleration input not available, using modifier 
            acceleration += accelerationModifier_value.y * accelerationModificationMagnitude;
            acceleration = Mathf.Clamp(acceleration, 0.0f, 1.0f);
            Debug.Log("Input: accelerationAbsolute value = " + accelerationModifier_value + " == " + acceleration);
        }
    }


    /**********************************************************************\
    |                      SECTION: INPUT ACTION FUNCTIONS                 |
    \**********************************************************************/
    private void Train_FocusPanel(InputAction.CallbackContext obj)
    {
        Debug.Log("Input: Train - FocusPanel");
    }


    private void Pause(InputAction.CallbackContext obj)
    {
        Debug.Log("Input: Pause");
    }


}
