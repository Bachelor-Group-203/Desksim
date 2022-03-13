using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInputController : MonoBehaviour
{

    /***************\
    | Static values |
    \***************/

    /*************\
    | Core values |
    \*************/

    /***************\
    | UI components |
    \***************/


    /***************\
    | Helper values |
    \***************/
    private bool hasBeenEnabled = false;
    [SerializeField]
    private bool debug = true;
    [SerializeField]
    private bool superDebug = false;



    /********************\
    | Input action asset |
    \********************/
    private UserInputActions userInputActions; // Not static or global, this is this scripts own instance of the input action asset

    /*********************\
    | TRAIN input actions |
    \*********************/
    private InputActionMap userInputActionMap_Player;

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

        if(debug) Debug.Log("<InputController Player> \tOnEnable called");

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
        if(debug) Debug.Log("<InputController Player> \tEnable() called");


        // Subscribing actions to functions
        userInputActions.Player.Menu.performed += InputManager.Pause;
        userInputActions.Player.RebindMenu.performed += InputManager.RebindMenu;
        userInputActions.Player.EnterTrain.performed += Train_EnterTrain;

        // Movement & Camera are both vec2
        // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.3/manual/Actions.html


        // Enabling the action inputs
        userInputActions.Player.Menu.Enable();
        userInputActions.Player.Camera.Enable();
        userInputActions.Player.Movement.Enable();
        userInputActions.Player.EnterTrain.Enable();
        userInputActions.Player.RebindMenu.Enable();

    }


    /**
     * Called when object is disabled, disables all input actions.
     **/
    private void OnDisable()
    {
        if(debug) Debug.Log("<InputController Player> \tOnDisable called");

        // Enabling the action inputs, so they won't call
        userInputActions.Player.Menu.Disable();
        userInputActions.Player.Camera.Disable();
        userInputActions.Player.Movement.Disable();
        userInputActions.Player.EnterTrain.Disable();
        userInputActions.Player.RebindMenu.Disable();
    }


    /**
     * Called every *#!%(!#&#)
     **/
    /**private void FixedUpdate()
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

        

    }**/

    private void Train_EnterTrain(InputAction.CallbackContext obj)
    {
        // Do checks like seeing if you're close enough to the train here
        if(debug) Debug.Log("<InputController Player> \tEnterTrain");
        InputManager.EnterTrain();
    }


}
