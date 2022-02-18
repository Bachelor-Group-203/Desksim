using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    public float pressure;     // 0 -> 1, apply to curve in train controller or in separate value
    public float acceleration; // 0 -> 1


    /***************\
    | UI components |
    \***************/

    [SerializeField]
    private Slider pressureSlider;
    [SerializeField]
    private Slider accelerationSlider;
    [SerializeField]
    private Toggle invertModifiersToggle;
    [SerializeField]
    private Toggle invertAbsolutesToggle;
    private bool invertModifiers = false;
    private bool invertAbsolutes = false;


    /***************\
    | Helper values |
    \***************/
    private float pressureAbsolute_value = 0;
    private float pressureAbsolute_delta = 0;
    private float pressureModifier_value;
    private float accelerationAbsolute_value = 0;
    private float accelerationAbsolute_delta = 0;
    private float accelerationModifier_value;
    private bool enabled = false;
    [SerializeField]
    private bool debug = false;



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
        // userInputActions = new UserInputActions();
    }


    /**
     * Called when object is enabled, checks if InputManager.userInputActions is ready.
     **/
    private void OnEnable()
    {

        if(debug) Debug.Log("InputController: OnEnable called");

        if (InputManager.userInputActions != null && userInputActions == null)
        {
            userInputActions = InputManager.userInputActions;
            Enable();
        }
        else if (userInputActions != null) Enable();
        
    }

    /**
     * Enables and binds action inputs
     **/
    private void Enable()
    {
        enabled = true;
        if(debug) Debug.Log("InputController: Enable() called");


        // Caching input actions to local variables
        pressureAbsolute = userInputActions.Train.PressureAbsolute;
        pressureModifier = userInputActions.Train.PressureModifier;
        accelerationAbsolute = userInputActions.Train.AccelerationAbsolute;
        accelerationModifier = userInputActions.Train.AccelerationModifier;

        // Subscribing input actions "performed" to functions
        userInputActions.Train.Menu.performed += Pause;
        userInputActions.Train.FocusPanel.performed += Train_FocusPanel;
        userInputActions.Train.RebindMenu.performed += RebindMenu;

        // Enabling the action inputs
        pressureAbsolute.Enable();
        pressureModifier.Enable();
        accelerationAbsolute.Enable();
        accelerationModifier.Enable();
        userInputActions.Train.Menu.Enable();
        userInputActions.Train.FocusPanel.Enable();
        userInputActions.Train.RebindMenu.Enable();

        LoadExtraOptions();

    }


    /**
     * Called when object is disabled, disables all input actions.
     **/
    private void OnDisable()
    {
        if(debug) Debug.Log("InputController: OnEnable called");
        // Enabling the action inputs, so they won't call
        pressureAbsolute.Disable();
        pressureModifier.Disable();
        accelerationAbsolute.Disable();
        accelerationModifier.Disable();
        userInputActions.Train.Menu.Disable();
        userInputActions.Train.FocusPanel.Disable();
        userInputActions.Train.RebindMenu.Disable();
    }


    /**
     * Called every *#!%(!#&#)
     **/
    private void FixedUpdate()
    {
        // Wait for InputManager to have been initiated, before enabling controls.
        if (!enabled) {
            if (InputManager.userInputActions != null && userInputActions == null)
            {
                userInputActions = InputManager.userInputActions;
                Enable();
            }
        };
        // All controller vector2/axis values are assumed to be from -1 to 1, or 0 to 1 for joysticks, this might vary from device to device, hopefully not. Can be checked in window->analysis->input 


        /**********\
        | PRESSURE |
        \**********/
        // Check if absolute pressure input is available, if a compatible joystick is being used.
        // Should be bound in settings to ensure correct functionality, maybe a bool if custom joystick has been set in settings.
        pressureAbsolute_delta = pressureAbsolute_value - pressureAbsolute.ReadValue<float>();
        pressureAbsolute_value = pressureAbsolute.ReadValue<float>();
        pressureModifier_value = pressureModifier.ReadValue<float>();
        if (pressureAbsolute_delta != 0) {
            // Absolute pressure input is available, and changed
            if(debug) Debug.Log("Input: pressureABSOLUTE value = " + pressureAbsolute_value + " -> " + (pressureAbsolute_value + 1) / 2 +""+ (invertAbsolutes ? " <inverted>" : ""));
            pressure = (pressureAbsolute_value+1)/2; // Axises go from -1 to 1, changes it to 0 -> 
            if (invertAbsolutes) pressure = 1 - pressure;

        }
        else if (pressureModifier_value != 0) {

            // Absolute pressure input not available, using modifier input
            if (invertModifiers) pressureModifier_value = ((pressureModifier_value > 0) ? 1 - pressureModifier_value : -(1 + pressureModifier_value));
            pressure += pressureModifier_value * pressureModificationMagnitude;
            pressure = Mathf.Clamp(pressure, 0.0f, 1.0f);
            if(debug) Debug.Log("Input: pressureMODIFIER value = " + pressureModifier_value + " == " + pressure);
        }

        /**************\
        | ACCELERATION |
        \**************/
        // Check if absolute acceleration input is available, if a compatible joystick is being used.
        // Should be bound in settings to ensure correct functionality, maybe a bool if custom joystick has been set in settings.
        accelerationAbsolute_delta = accelerationAbsolute_value - accelerationAbsolute.ReadValue<float>();
        accelerationAbsolute_value = accelerationAbsolute.ReadValue<float>();
        accelerationModifier_value = accelerationModifier.ReadValue<float>();
        // If absolute value has changed
        if (accelerationAbsolute_delta != 0) {
            // Absolute acceleration input is available, and changed
            if(debug) Debug.Log("Input: accelerationABSOLUTE value = " + accelerationAbsolute_value + " -> " + (accelerationAbsolute_value+1)/2 +""+ (invertAbsolutes?" <inverted>":""));
            acceleration = (accelerationAbsolute_value+1)/2;
            if (invertAbsolutes) acceleration = 1 - acceleration;

        } else if(accelerationModifier_value != 0) {

            // Absolute acceleration input not available, using modifier
            if (invertModifiers) accelerationModifier_value = ((accelerationModifier_value > 0) ? 1 - accelerationModifier_value : -(1 + accelerationModifier_value));
            acceleration += accelerationModifier_value  * accelerationModificationMagnitude;
            acceleration = Mathf.Clamp(acceleration, 0.0f, 1.0f);
            if(debug) Debug.Log("Input: accelerationMODIFIER value = " + accelerationModifier_value + " == " + acceleration);
        }

        /*********\
        | SLIDERS |
        \*********/
        // Update value of sliders (Sliders update the value in separate OnValueChanged event)
        if (accelerationSlider != null) accelerationSlider.value = acceleration;
        if (pressureSlider != null) pressureSlider.value = pressure;
        // May cause jittering, if so only update this when the value wasn't changed by the slider
        

    }



    /**
     * 
     **/
    public void OnSliderAccelerationValueChanged(System.Single value)
    {
        acceleration = value;
    }

    /**
     * 
     **/
    public void OnSliderPressureValueChanged(System.Single value)
    {
        pressure = value;
    }

    /**
     * 
     **/
    public void OnInvertModifiersToggleValueChanged(bool value)
    {
        invertModifiers = value;
        SaveExtraOptions();
    }

    /**
     * 
     **/
    public void OnInvertAbsoluteToggleValueChanged(bool value)
    {
        invertAbsolutes = value;
        SaveExtraOptions();
    }

    /**
     * 
     **/
    public void LoadExtraOptions()
    {
        Debug.Log("LoadExtraOptions called");
        invertModifiers = PlayerPrefs.GetInt("invertModifiers") == 1 ? true : false;
        invertAbsolutes = PlayerPrefs.GetInt("invertAbsolutes") == 1 ? true : false;
        // If a ui element for selecting modifierInvert exists, update it to reflect the saved setting
        if (invertModifiersToggle != null) invertModifiersToggle.GetComponent<Toggle>().isOn = invertModifiers;
        if (invertAbsolutesToggle != null) invertAbsolutesToggle.GetComponent<Toggle>().isOn = invertAbsolutes;
    }

    /**
     * 
     * Called by the UI elements that change these options
     **/
    public void SaveExtraOptions()
    {
        Debug.Log("SaveExtraOptions called");
        PlayerPrefs.SetInt("invertModifiers", invertModifiers ? 1 : 0);
        PlayerPrefs.SetInt("invertAbsolutes", invertAbsolutes ? 1 : 0);
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
    private void RebindMenu(InputAction.CallbackContext obj)
    {
        Debug.Log("Input: RebindMenu");
    }



}
