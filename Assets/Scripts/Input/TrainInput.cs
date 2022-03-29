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
    private static float pressureModificationMagnitude = 0.02f; // What the joystick value will be multiplied with before being added to the end-value
    private static float accelerationModificationMagnitude = 0.01f; // What the joystick value will be multiplied with before being added to the end-value

    /*************\
    | Core values |
    \*************/

    private float p;
    public float pressure       // 0 -> 1, apply to curve in train controller or in separate value
    {
        get { return p; }
        set { }
    }

    private float a;
    public float acceleration   // 0 -> 1
    {
        get { return a; }
        set { }
    }

    /***************\
    | UI components |
    \***************/
    [SerializeField]
    private Slider[] pressureSliders;
    [SerializeField]
    private Slider[] accelerationSliders;


    /***************\
    | Helper values |
    \***************/
    private float pressureAbsolute_value = 0;
    private float pressureAbsolute_value_prev = 0;
    private float pressureAbsolute_delta = 0;
    private float pressureModifier_value;
    private float accelerationAbsolute_value = 0;
    private float accelerationAbsolute_value_prev = 0;
    private float accelerationAbsolute_delta = 0;
    private float accelerationModifier_value;
    private bool hasBeenEnabled = false;
    private bool multipleIdenticalSticks_areSetup = false;
    [SerializeField]
    private bool debug = true;
    [SerializeField]
    private bool superDebug = false;


    // These can be serialized to avoid using tags, but then the script should be a part of the train, in which case the way it accesses InputManager might need to be updated too.
    // [SerializeField]
    private GameObject camera_1P;
    // [SerializeField]
    private GameObject camera_3P; // Optional
    // [SerializeField]
    private GameObject camera_FocusOnPanel;
    [SerializeField]
    public bool allowThirdPerson = true;


    private GameObject canvasUI_overlay;
    private GameObject canvasUI_physical_interactive;
    private GameObject canvasUI_physical_decorative;



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

    private InputAction leftStick;
    private InputAction rightStick;


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

        if (debug) Debug.Log("<InputController Train> \tOnEnable called");

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
        if (debug) Debug.Log("<InputController Train> \tEnable() called");

        // Shouldn't have to do this but in case inputmanager has disabled the entire action map and it overrides something, doing this.
        InputManager.EnterTrain();

        // Caching input actions to local variables
        pressureAbsolute = userInputActions.Train.PressureAbsolute;
        pressureModifier = userInputActions.Train.PressureModifier;
        accelerationAbsolute = userInputActions.Train.AccelerationAbsolute;
        accelerationModifier = userInputActions.Train.AccelerationModifier;

        // Subscribing input actions "performed" to functions
        userInputActions.Train.FocusOnPanel.performed += Train_FocusOnPanel;
        userInputActions.Train.Menu.performed += InputManager.TogglePause;
        userInputActions.Train.ExitTrain.performed += Train_ExitTrain;
        userInputActions.Train.ChangePerspective.performed += ChangePerspective;



        // Enabling the action inputs
        pressureAbsolute.Enable();
        pressureModifier.Enable();
        accelerationAbsolute.Enable();
        accelerationModifier.Enable();
        userInputActions.Train.FocusOnPanel.Enable();
        userInputActions.Train.ExitTrain.Enable();
        userInputActions.Train.Menu.Enable();
        userInputActions.Train.ChangePerspective.Enable();

        InputManager.LoadExtraOptions();

        if (InputManager.usingMultipleIdenticalSticks) SetupMultipleIdenticalSticks();

        if (InputManager.IsPaused) InputManager.Unpause();

        InitiatePerspective();
    }


    /**
     * Called when object is disabled, disables all input actions.
     **/
    private void OnDisable()
    {
        if (debug) Debug.Log("<InputController Train> \tOnDisable called");
        // Enabling the action inputs, so they won't call
        pressureAbsolute.Disable();
        pressureModifier.Disable();
        accelerationAbsolute.Disable();
        accelerationModifier.Disable();
        userInputActions.Train.Menu.Disable();
        userInputActions.Train.FocusOnPanel.Disable();
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
        if (multipleIdenticalSticks_areSetup && superDebug && InputManager.usingMultipleIdenticalSticks) Debug.Log(leftStick.ReadValue<Vector2>() + "     &&&&      " + rightStick.ReadValue<Vector2>());
        if (InputManager.usingMultipleIdenticalSticks && (!multipleIdenticalSticks_areSetup || (leftStick == null || rightStick == null)))
        {
            Debug.Log("<InputController Train> \tusingMultipleIdenticalSticks is true, but they're not set up properly or one or more of the sticks are null, trying to fix that.");
            SetupMultipleIdenticalSticks();
        }

        //if (superDebug) Debug.Log("<InputController Train> \t--PRESSURE ABSOLUTE-- = " + pressureAbsolute.ReadValue<float>()+" | "+pressureAbsolute.GetBindingDisplayString() + " | " +pressureAbsolute.enabled);


        /**********\
        | PRESSURE |
        \**********/
        // Check if absolute pressure input is available, if a compatible joystick is being used.
        // Should be bound in settings to ensure correct functionality, maybe a bool if custom joystick has been set in settings.
        if (pressureAbsolute != null && pressureAbsolute.enabled && pressureAbsolute.ReadValue<float>() != null)
        {
            // Store previous absolute value
            pressureAbsolute_value_prev = pressureAbsolute_value;

            // If multiple identical sticks are enabled and recognized
            if (InputManager.usingMultipleIdenticalSticks && multipleIdenticalSticks_areSetup)
            {
                // Read value from right stick
                pressureAbsolute_value = rightStick.ReadValue<Vector2>().y;
            }
            else
            {
                // Otherwise, read the value set in the rebinding menu
                pressureAbsolute_value = pressureAbsolute.ReadValue<float>();
            }

            // If absolute value has changed
            pressureModifier_value = pressureModifier.ReadValue<float>();
            pressureAbsolute_delta = pressureAbsolute_value_prev - pressureAbsolute_value;
            if (pressureAbsolute_delta != 0)
            {
                // Absolute pressure input is available, and changed
                if (debug) Debug.Log("<InputController Train> \tpressureABSOLUTE value = " + pressureAbsolute_value + " -> " + (pressureAbsolute_value + 1) / 2 + "" + (InputManager.invertAbsoluteP ? " <inverted>" : ""));
                p = (pressureAbsolute_value + 1) / 2; // Axises go from -1 to 1, changes it to 0 -> 
                if (InputManager.invertAbsoluteP) p = 1 - p;

            }
            else if (pressureModifier_value != 0)
            {

                // Absolute pressure input not available, using modifier input
                if (InputManager.invertModifiers) pressureModifier_value = ((pressureModifier_value > 0) ? 1 - pressureModifier_value : -(1 + pressureModifier_value));
                p += pressureModifier_value * pressureModificationMagnitude;
                p = Mathf.Clamp(p, 0.0f, 1.0f);
                if (debug) Debug.Log("<InputController Train> \tpressureMODIFIER value = " + pressureModifier_value + " == " + p);
            }
        }

        /**************\
        | ACCELERATION |
        \**************/
        // Check if absolute acceleration input is available, if a compatible joystick is being used.
        // Should be bound in settings to ensure correct functionality, maybe a bool if custom joystick has been set in settings.
        if (pressureAbsolute != null && pressureAbsolute.enabled && pressureAbsolute.ReadValue<float>() != null)
        {
            // Store previous absolute value
            accelerationAbsolute_value_prev = accelerationAbsolute_value;

            // If multiple identical sticks are enabled and recognized
            if (InputManager.usingMultipleIdenticalSticks && multipleIdenticalSticks_areSetup)
            {
                // Read value from left stick
                accelerationAbsolute_value = leftStick.ReadValue<Vector2>().y;
            }
            else
            {
                // Otherwise, read the value set in the rebinding menu
                accelerationAbsolute_value = accelerationAbsolute.ReadValue<float>();
            }


            // If absolute value has changed
            accelerationModifier_value = accelerationModifier.ReadValue<float>();
            accelerationAbsolute_delta = accelerationAbsolute_value_prev - accelerationAbsolute_value;
            if (accelerationAbsolute_delta != 0)
            {
                // Absolute acceleration input is available, and changed
                if (debug) Debug.Log("<InputController Train> \taccelerationABSOLUTE value = " + accelerationAbsolute_value + " -> " + (accelerationAbsolute_value + 1) / 2 + "" + (InputManager.invertAbsoluteA ? " <inverted>" : ""));
                a = (accelerationAbsolute_value + 1) / 2;
                if (InputManager.invertAbsoluteA) a = 1 - a;

            }
            else if (accelerationModifier_value != 0)
            {

                // Absolute acceleration input not available, using modifier
                if (InputManager.invertModifiers) accelerationModifier_value = ((accelerationModifier_value > 0) ? 1 - accelerationModifier_value : -(1 + accelerationModifier_value));
                a += accelerationModifier_value * accelerationModificationMagnitude;
                a = Mathf.Clamp(a, 0.0f, 1.0f);
                if (debug) Debug.Log("<InputController Train> \taccelerationMODIFIER value = " + accelerationModifier_value + " == " + a);
            }
        }

        /*********\
        | SLIDERS |
        \*********/
        // Update value of sliders (Sliders update the value in separate OnValueChanged event attached to them)
        if (accelerationSliders.Length > 0) {
            for (int i = 0; i < accelerationSliders.Length; i++)
            { accelerationSliders[i].value = a; } // Set slider value to current acceleration
        }

        if (pressureSliders.Length > 0) {
            for (int i = 0; i < pressureSliders.Length; i++) 
            { pressureSliders[i].value = p; } // Set slider value to current pressure
        }
        // May cause jittering, if so only update this when the value wasn't changed by the slider

    }


    /**
     * Creates aliases for the two first joysticks
     **/
    private void SetupMultipleIdenticalSticks()
    {
        Debug.Log("<InputController Train> \tTrying to set up for multiple identical sticks");
        leftStick = new InputAction(binding: "<Joystick>{LeftHand}/stick");
        rightStick = new InputAction(binding: "<Joystick>{RightHand}/stick");
        leftStick.Enable();
        rightStick.Enable();

        if (Joystick.all.Count < 2)
        {
            Debug.LogWarning("<InputController Train> \tLess than two controllers attached, but the usingMultipleIdenticalSticks option is enabled! ");
        }
        else
        {
            InputSystem.AddDeviceUsage(Joystick.all[0], CommonUsages.LeftHand);
            InputSystem.AddDeviceUsage(Joystick.all[1], CommonUsages.RightHand);
            multipleIdenticalSticks_areSetup = true;
        }
    }

    /**
     * 
     **/
    public void OnSliderAccelerationValueChanged(System.Single value)
    {
        a = value;
        Debug.Log("\t\t\t\tAcceler slider changed: " + a + " (should be " + value + ")");
    }

    /**
     * 
     **/
    public void OnSliderPressureValueChanged(System.Single value)
    {
        p = value;
        Debug.Log("\t\t\t\tPressur slider changed: " + p + " (should be " + value + ")");
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
        if (!allowThirdPerson) return;
        Debug.Log("<InputController Train> \tChangePerspective");

        if(debug) Debug.Log("Begin [1P=" + camera_1P.activeSelf + " 3P=" + camera_3P.activeSelf + " 3Penabled=" + allowThirdPerson + "]");

        if (camera_1P != null && camera_3P != null)
        {
            if (camera_1P.activeSelf)
            {
                camera_1P.SetActive(false);
                camera_3P.SetActive(true);
            }
            else
            {
                camera_1P.SetActive(true);
                camera_3P.SetActive(false);
            }
        } 
        else
        {
            Debug.Log("\t\t\t One of the Camera_3P ("+camera_3P+") or Camera_1P ("+camera_1P+") cameras are disabled, they need to be enabled by default so they can be changed by the script.");
        }
        if(debug) Debug.Log("Endin [1P=" + camera_1P.activeSelf + " 3P=" + camera_3P.activeSelf + " 3Penabled=" + allowThirdPerson + "]");
        UpdateUI();
    }



    private void InitiatePerspective()
    {
        Debug.Log("<InputController Train> \tInitiatePerspective");
        camera_3P = GameObject.FindGameObjectWithTag("Camera_3P");
        camera_1P = GameObject.FindGameObjectWithTag("Camera_1P");
        camera_FocusOnPanel = GameObject.FindGameObjectWithTag("Camera_FocusOnPanel");

        if (camera_1P != null && camera_3P != null)
        {
            camera_1P.SetActive(true);
            camera_3P.SetActive(false);
            camera_FocusOnPanel.SetActive(false);
        }
        else
        {
            if (allowThirdPerson) Debug.Log("\t\t\t One of the Camera_3P or Camera_1P cameras are disabled, if not intentional, both need to be enabled by default so they can be changed by the script.");
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        Debug.Log("<InputController Train> \tUpdateUI");
        if (!canvasUI_overlay)              canvasUI_overlay                = GameObject.FindGameObjectWithTag("UI_Train");
        if (!canvasUI_physical_interactive) canvasUI_physical_interactive   = GameObject.FindGameObjectWithTag("UI_Train_VR-interactive");
        if (!canvasUI_physical_decorative)  canvasUI_physical_decorative    = GameObject.FindGameObjectWithTag("UI_Train_VR-decorative");
        if (!canvasUI_overlay)              Debug.Log("\t\t\t Overlay Train UI not found! An element with tag \"UI_Train\" needs to exist and be enabled.");
        if (!canvasUI_physical_interactive) Debug.Log("\t\t\t VR Train UI not found! An element with tag \"UI_Train_VR-interactive\" needs to exist and be enabled.");
        if (!canvasUI_physical_decorative)  Debug.Log("\t\t\t Decorative VR Train UI not found! An element with tag \"UI_Train_VR-decorative\" needs to exist and be enabled.");


        // Which perspective is used
        if ((camera_1P.activeSelf && allowThirdPerson) || camera_FocusOnPanel.activeSelf)
        {
            // Current state =
            //  First person + third person mode enabled
            //  |OR| Focusing on panel
            // 
            // => Disable overlay UI, enable interactive VR UI 
            canvasUI_overlay.SetActive(false);
            canvasUI_physical_interactive.SetActive(true);
            canvasUI_physical_decorative.SetActive(false);
            Debug.Log("\t\t\t Disabling overlay & enabling interactive VR UI [1P=" + camera_1P.activeSelf + " 3P=" + camera_3P.activeSelf + " 3Penabled=" + allowThirdPerson + "]");
        }
        else if((camera_1P.activeSelf && !allowThirdPerson) || (camera_3P && camera_3P.activeSelf) || !allowThirdPerson)
        {
            // Current state = 
            //  First person + third person mode disabled
            //  |OR| Third person
            // 
            // => Enable overlay UI && disable interactive VR UI
            canvasUI_overlay.SetActive(true);
            canvasUI_physical_interactive.SetActive(false);
            canvasUI_physical_decorative.SetActive(true);
            Debug.Log("\t\t\t Enabling overlay & disabling interactive VR UI [1P="+camera_1P.activeSelf+" 3P="+camera_3P.activeSelf+" 3Penabled="+allowThirdPerson+"]");
        } 
        else if(camera_1P.activeSelf && allowThirdPerson)
        {

            // Current state =
        }
    }


    /**
     * 
     **/
    private void Train_FocusOnPanel(InputAction.CallbackContext obj)
    {
        // Switch to camera focusing on panel
        Debug.Log("<InputController Train> \tFocusOnPanel");


        // Enabling the focus cam
        if (camera_1P.activeSelf || (camera_3P && camera_3P.activeSelf))
        {
            camera_1P.SetActive(false);
            if (camera_3P) camera_3P.SetActive(false);
            camera_FocusOnPanel.SetActive(true);
        } 
        // Disabling the focus cam
        else if (camera_FocusOnPanel.activeSelf)
        {
            camera_1P.SetActive(true);
            camera_FocusOnPanel.SetActive(false);
        }
    }

}


