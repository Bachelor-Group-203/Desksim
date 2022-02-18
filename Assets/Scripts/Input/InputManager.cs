using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class InputManager : MonoBehaviour
{
    
    public static UserInputActions userInputActions; // Reference to input action asset

    public static event Action rebindComplete;
    public static event Action rebindCancelled;
    public static event Action<InputAction, int> rebindStarted;

    /**
     * 
     **/
    private void Awake()
    {
        if (userInputActions == null) // Safety check to prevent race condition, there should only be one of this.
        {
            userInputActions = new UserInputActions();
        }
    }

    /**
     * Called by UI component
     * From Unity's rebinding sample
     **/
    public static void StartRebind(string actionName, int bindingIndex, Text statusText, bool excludeMouse)
    {
        InputAction action = userInputActions.asset.FindAction(actionName); // Find action by name from our input action asset instance
        if (action == null || action.bindings.Count <= bindingIndex) {
            Debug.Log("Input: Couldn't find action or binding");
            return;
        }

        // If binding to rebind is a composite
        if (action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            // If not composite, rebind normally
            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
            {
                DoRebind(action, firstPartIndex, statusText, true, excludeMouse);
            }
                
        } // Else, rebind normally
        else DoRebind(action, bindingIndex, statusText, false, excludeMouse);

    }

    /**
     * 
     **/
    private static void DoRebind(InputAction actionToRebind, int bindingIndex, Text statusText, bool allCompositeParts, bool excludeMouse) 
    {
        if (actionToRebind == null || bindingIndex < 0) return;

        statusText.text = $"Press a {actionToRebind.expectedControlType}";

        actionToRebind.Disable(); // Action must be disabled before rebinding

        // Create instance of object that is going to do the rebinding for us
        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);

        // On completion of rebinding process
        rebind.OnComplete(operation =>
        {
            actionToRebind.Enable(); // Re-enable action after rebinding is complete
            operation.Dispose(); // Delete this behavior to prevent memory leak

            // If composite binding
            if(allCompositeParts)
            {
                var nextBindingIndex = bindingIndex + 1;
                // Recursively increase index until next binding isn't a composite
                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isPartOfComposite)
                    DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts, excludeMouse);
            }

            SaveBindingOverride(actionToRebind);
            rebindComplete?.Invoke(); // Invoke if something is subscribed to it

        });

        // On cancellation of rebinding process
        rebind.OnCancel(operation =>
        {
            actionToRebind.Enable(); // Re-enable action after rebinding is cancelled
            operation.Dispose(); // Delete this behavior to prevent memory leak
            rebindCancelled?.Invoke(); // Invoke if something is subscribed to it
        });

        // Button to cancel rebinding
        rebind.WithCancelingThrough("<Keyboard>/escape");
        rebind.WithCancelingThrough("<Mouse>/rightButton");
        rebind.WithCancelingThrough("<Mouse>/leftButton");
        if (excludeMouse) rebind.WithControlsExcluding("Mouse");

        // Start the rebinding process
        rebindStarted?.Invoke(actionToRebind, bindingIndex);
        rebind.Start(); 

    }


    /**
     * 
     **/
    public static string GetBindingName(string actionName, int bindingIndex)
    {
        if (userInputActions == null) userInputActions = new UserInputActions(); // Stop racing condition

        InputAction action = userInputActions.asset.FindAction(actionName);
        return action.GetBindingDisplayString(bindingIndex);
    }


    /**
     * 
     **/
    private static void SaveBindingOverride(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath); // Overridepath is what it looks for during runtime
        }
    }

    /**
     * 
     * Public because it will be called by the RebindUI
     **/
    public static void LoadBindingOverride(string actionName)
    {
        if (userInputActions == null) userInputActions = new UserInputActions();
        InputAction action = userInputActions.asset.FindAction(actionName);
        
        // Loop through all bindings in action
        for (int i=0; i<action.bindings.Count; i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
                action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
        }
    }

    /**
     * 
     **/
    public static void ResetBinding(string actionName, int bindingIndex)
    {
        InputAction action = userInputActions.asset.FindAction(actionName);
        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("InputManager: Could not find action or binding");
            return;
        }

        // if composite
        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex; i < action.bindings.Count && (action.bindings[i].isPartOfComposite || action.bindings[i].isComposite); i++)
            {
                action.RemoveBindingOverride(i);
            }
        }
        else
            action.RemoveBindingOverride(bindingIndex);


        SaveBindingOverride(action);
    }

}
