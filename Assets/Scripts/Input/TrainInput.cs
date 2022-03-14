using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TrainInput : MonoBehaviour
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

    private float p;
    public float pressure       // 0 -> 1, apply to curve in train controller or in separate value
    {   get { return p; }
        set { } }

    private float a;
    public float acceleration   // 0 -> 1
    {   get { return a; }
        set { } }

    /***************\
    | UI components |
    \***************/
    [SerializeField]
    private Slider pressureSlider;
    [SerializeField]
    private Slider accelerationSlider;


    /***************\
    | Helper values |
    \***************/
    private float pressureAbsolute_value = 0;
    private float pressureAbsolute_delta = 0;
    private float pressureModifier_value;
    private float accelerationAbsolute_value = 0;
    private float accelerationAbsolute_delta = 0;
    private float accelerationModifier_value;
    private bool hasBeenEnabled = false;
    [SerializeField]
    private bool debug = false;
    [SerializeField]
    private bool superDebug = false;



    /********************\
    | Input action asset |
    \********************/
    private UserInputActions userInputActions; // Not static or global, this is this scripts own instance of the input action asset


    // Pressure
    private InputAction pressureAbsolute; // For compatible joysticks, use when available
    private InputAction pressureModifier; // For alternative inputs, keyboard & gamepads
    // TODO make pressure controllable with in-game slider too
    // Acceleration
    private InputAction accelerationAbsolute; // For compatible joysticks, use when available
    private InputAction accelerationModifier; // For alternative inputs, keyboard & gamepads
    // TODO make acceleration controllable with in-game slider too


    /**
     * Called when object is instantiated
     **/
    private void Awake()
    {
        // Creating a new instance of input action asset
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
        hasBeenEnabled = true;
        if(debug) Debug.Log("InputController: Enable() called");


        // Caching input actions to local variables
        pressureAbsolute = userInputActions.Train.PressureAbsolute;
        pressureModifier = userInputActions.Train.PressureModifier;
        accelerationAbsolute = userInputActions.Train.AccelerationAbsolute;
        accelerationModifier = userInputActions.Train.AccelerationModifier;

        // Subscribing input actions "performed" to functions
        userInputActions.Train.FocusPanel.performed += Train_FocusPanel;
        userInputActions.Train.Menu.performed += InputManager.Pause;
        userInputActions.Train.RebindMenu.performed += InputManager.RebindMenu;
        userInputActions.Train.ExitTrain.performed += Train_ExitTrain;
        userInputActions.Train.ChangePerspective.performed += ChangePerspective;



        // Enabling the action inputs
        pressureAbsolute.Enable();
        pressureModifier.Enable();
        accelerationAbsolute.Enable();
        accelerationModifier.Enable();
        userInputActions.Train.FocusPanel.Enable();
        userInputActions.Train.ExitTrain.Enable();
        userInputActions.Train.Menu.Enable();
        userInputActions.Train.RebindMenu.Enable();
        userInputActions.Train.ChangePerspective.Enable();

        InputManager.LoadExtraOptions();

    }


    /**
     * Called when object is disabled, disables all input actions.
     **/
    private void OnDisable()
    {
        if(debug) Debug.Log("<InputController Train> \tOnDisable called");
        // Enabling the action inputs, so they won't call
        pressureAbsolute.Disable();
        pressureModifier.Disable();
        accelerationAbsolute.Disable();
        accelerationModifier.Disable();
        userInputActions.Train.Menu.Disable();
        userInputActions.Train.FocusPanel.Disable();
        userInputActions.Train.RebindMenu.Disable();
        userInputActions.Train.ExitTrain.Disable();
        userInputActions.Train.ChangePerspective.Disable();
    }


    /**
     * Called every *#!%(!#&#)
     **/
    private void FixedUpdate()
    {
        // Wait for InputManager to have been initiated, before enabling controls.
        if (!hasBeenEnabled)
        {
            if (InputManager.userInputActions != null && userInputActions == null)
            {
                userInputActions = InputManager.userInputActions;
                Enable();
            }
        }
        // All controller vector2/axis values are assumed to be from -1 to 1, or 0 to 1 for joysticks, this might vary from device to device, hopefully not. Can be checked in window->analysis->input 


        if(superDebug) Debug.Log("<InputController Train> \t--PRESSURE ABSOLUTE-- = " + pressureAbsolute.ReadValue<float>()+" | "+pressureAbsolute.GetBindingDisplayString() + " | " +pressureAbsolute.enabled);
        

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
            if(debug) Debug.Log("<InputController Train> \tpressureABSOLUTE value = " + pressureAbsolute_value + " -> " + (pressureAbsolute_value + 1) / 2 +""+ (InputManager.invertAbsoluteP ? " <inverted>" : ""));
            p = (pressureAbsolute_value+1)/2; // Axises go from -1 to 1, changes it to 0 -> 
            if (InputManager.invertAbsoluteP) p = 1 - p;

        }
        else if (pressureModifier_value != 0) {

            // Absolute pressure input not available, using modifier input
            if (InputManager.invertModifiers) pressureModifier_value = ((pressureModifier_value > 0) ? 1 - pressureModifier_value : -(1 + pressureModifier_value));
            p += pressureModifier_value * pressureModificationMagnitude;
            p = Mathf.Clamp(p, 0.0f, 1.0f);
            if(debug) Debug.Log("<InputController Train> \tpressureMODIFIER value = " + pressureModifier_value + " == " + p);
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
            if(debug) Debug.Log("<InputController Train> \taccelerationABSOLUTE value = " + accelerationAbsolute_value + " -> " + (accelerationAbsolute_value+1)/2 +""+ (InputManager.invertAbsoluteA ?" <inverted>":""));
            a = (accelerationAbsolute_value+1)/2;
            if (InputManager.invertAbsoluteA) a = 1 - a;

        } else if(accelerationModifier_value != 0) {

            // Absolute acceleration input not available, using modifier
            if (InputManager.invertModifiers) accelerationModifier_value = ((accelerationModifier_value > 0) ? 1 - accelerationModifier_value : -(1 + accelerationModifier_value));
            a += accelerationModifier_value  * accelerationModificationMagnitude;
            a = Mathf.Clamp(a, 0.0f, 1.0f);
            if(debug) Debug.Log("<InputController Train> \taccelerationMODIFIER value = " + accelerationModifier_value + " == " + a);
        }

        /*********\
        | SLIDERS |
        \*********/
        // Update value of sliders (Sliders update the value in separate OnValueChanged event)
        if (accelerationSlider != null) accelerationSlider.value = a;
        if (pressureSlider != null) pressureSlider.value = p;
        // May cause jittering, if so only update this when the value wasn't changed by the slider

    }

    /**
     * 
     **/
    public void OnSliderAccelerationValueChanged(System.Single value)
    {
        a = value;
    }

    /**
     * 
     **/
    public void OnSliderPressureValueChanged(System.Single value)
    {
        p = value;
    }

    /**
     * 
     **/
    private void Train_FocusPanel(InputAction.CallbackContext obj)
    {
        Debug.Log("<InputController Train> \tFocusPanel");
    }
    
    /**
     * 
     **/
    private void Train_ExitTrain(InputAction.CallbackContext obj)
    {
        // Do checks like, only leaving if the train is standing still, here
        Debug.Log("<InputController Train> \tExitTrain");
        InputManager.ExitTrain();
    }

    /**
     * 
     **/
    private void ChangePerspective(InputAction.CallbackContext obj)
    {
        
    }

}
