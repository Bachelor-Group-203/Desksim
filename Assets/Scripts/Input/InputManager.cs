using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class InputManager : MonoBehaviour
{
    
    public static UserInputActions userInputActions; // Reference to input action asset
    public static event Action<InputActionMap> actionMapChanged;

    public static event Action rebindComplete;
    public static event Action rebindCancelled;
    public static event Action<InputAction, int> rebindStarted;

    // User-changeable values, set by usingMultipleIdenticalSticksToggle
    public static bool usingMultipleIdenticalSticks = false; 
    public static bool invertModifiers = false;
    public static bool invertAbsoluteA = false;
    public static bool invertAbsoluteP = false;

    [SerializeField]
    private static bool debug = false;

    /***************\
    | UI components |
    \***************/
    [SerializeField]
    private static Toggle invertModifiersToggle;
    [SerializeField]
    private static Toggle invertAbsoluteAToggle;
    [SerializeField]
    private static Toggle invertAbsolutePToggle;
    [SerializeField]
    private static Toggle usingMultipleIdenticalSticksToggle;


    /**
     * Called before the first frame
     **/
    private void Start()
    {
        userInputActions.Disable(); // Disable all action maps
        userInputActions.Train.Disable();
        userInputActions.Player.Disable();
        SwitchToActionMap(userInputActions.Train);
        GetUIElements();
    }



    /**
     * 
     **/
    private void Awake()
    {
        if (userInputActions == null) // Safety check to prevent race condition, there should only be one of this.
        {
            userInputActions = new UserInputActions();
        }
        GetUIElements();
        LoadAllBindingOverrides();
    }


    /**
     * 
     **/
    private static void SwitchToActionMap (InputActionMap actionMap)
    {
        if(debug) Debug.Log("<InputManager> \tSWITCHING TO ACTIONMAP: " + actionMap.name);
        if (actionMap.enabled)
        {
            if(debug) Debug.Log("\t\t\t- RETURNING, action map is already enabled");
            return;
            // If the selected action is already enabled do nothing
        }
        if(debug) Debug.Log("\t\t\t- Disabling all action maps");
        userInputActions.Disable(); // Disable all action maps
        userInputActions.Train.Disable();
        userInputActions.Player.Disable();

        actionMapChanged?.Invoke(actionMap); // Triggers a c# event that can be subscribed to in other scripts to see the currently active action map, e.g. when invoked read its InputActionMap.name

        if (debug) Debug.Log("\t\t\t- Enabling action map: is now "+actionMap.enabled);
        actionMap.Enable(); // Enable new action map
        if (debug) Debug.Log("\t\t\t- Enabled action map:  is now " + actionMap.enabled);
        if (debug) Debug.Log("\t\t\t= Trainmap="+userInputActions.Train.enabled+"   Playermap="+userInputActions.Player.enabled);

    }

    // Use this when leaving the train, to change action maps. To keep it centralized
    public static void ExitTrain()
    {
        Debug.Log("<InputManager> \tExitTrain() - Changing action map to Player");
        SwitchToActionMap(userInputActions.Player);
    }

    // Use this when entering the train, to change action maps. To keep it centralized
    public static void EnterTrain()
    {
        Debug.Log("<InputManager> \tEnterTrain() - Changing action map to Train");
        SwitchToActionMap(userInputActions.Train);
    }


    /**
     * Called by UI component
     * From Unity's rebinding sample
     **/
    public static void StartRebind(string actionName, int bindingIndex, TMPro.TMP_Text statusText)
    {
        InputAction action = userInputActions.asset.FindAction(actionName); // Find action by name from our input action asset instance
        if (action == null || action.bindings.Count <= bindingIndex) {
            Debug.Log("<InputManager> \tCouldn't find action or binding");
            return;
        }

        // If binding to rebind is a composite
        if (action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            // If not composite, rebind normally
            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
            {
                DoRebind(action, firstPartIndex, statusText, true);
            }
                
        } // Else, rebind normally
        else DoRebind(action, bindingIndex, statusText, false);

    }

    /**
     * 
     **/
    private static void DoRebind(InputAction actionToRebind, int bindingIndex, TMPro.TMP_Text statusText, bool allCompositeParts) 
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
                    DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts);
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
         rebind.WithControlsExcluding("Mouse");

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
    public static void LoadAllBindingOverrides()
    {
        if (userInputActions == null) userInputActions = new UserInputActions();
        var actionMaps = userInputActions.asset.actionMaps;
        for(int i=0; i<actionMaps.Count; i++)
        {
            var actions = actionMaps[i].actions;
            for(int j=0; j<actions.Count; j++)
            {
                LoadBindingOverride(actions[j].name);
            }
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
            Debug.Log("<InputManager> \tCould not find action or binding");
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
    public void OninvertAbsoluteAToggleValueChanged(bool value)
    {
        invertAbsoluteA = value;
        SaveExtraOptions();
    }

    /**
     * 
     **/
    public void OninvertAbsolutePToggleValueChanged(bool value)
    {
        invertAbsoluteP = value;
        SaveExtraOptions();
    }

    /**
     * 
     **/
    public static void OnUsingMultipleIdenticalSticksToggleValueChanged(bool value)
    {
        usingMultipleIdenticalSticks = value;
        SaveExtraOptions();
    }


    private static void GetUIElements()
    {
        if (invertModifiersToggle == null) invertModifiersToggle = GameObject.FindGameObjectWithTag("Rebinding_ToggleModifiers")    != null ? GameObject.FindGameObjectWithTag("Rebinding_ToggleModifiers").GetComponent<Toggle>() : null;
        if (invertAbsoluteAToggle == null) invertAbsoluteAToggle = GameObject.FindGameObjectWithTag("Rebinding_ToggleAbsAcc")       != null ? GameObject.FindGameObjectWithTag("Rebinding_ToggleAbsAcc").GetComponent<Toggle>() : null;
        if (invertAbsolutePToggle == null) invertAbsolutePToggle = GameObject.FindGameObjectWithTag("Rebinding_ToggleAbsPre")       != null ? GameObject.FindGameObjectWithTag("Rebinding_ToggleAbsPre").GetComponent<Toggle>() : null;
        if (usingMultipleIdenticalSticksToggle == null) usingMultipleIdenticalSticksToggle = GameObject.FindGameObjectWithTag("Rebinding_ToggleMultipleIdenticalSticks") != null ? GameObject.FindGameObjectWithTag("Rebinding_ToggleMultipleIdenticalSticks").GetComponent<Toggle>() : null;
    }

    /**
     * 
     **/
    public static void LoadExtraOptions()
    {
        GetUIElements();
        Debug.Log("<InputManager> \tLoadExtraOptions called");
        invertModifiers = PlayerPrefs.GetInt("invertModifiers") == 1 ? true : false;
        invertAbsoluteA = PlayerPrefs.GetInt("invertAbsoluteA") == 1 ? true : false;
        invertAbsoluteP = PlayerPrefs.GetInt("invertAbsoluteP") == 1 ? true : false;
        usingMultipleIdenticalSticks = PlayerPrefs.GetInt("usingMultipleIdenticalSticks") == 1 ? true : false;
        // If a ui element for selecting modifierInvert exists, update it to reflect the saved setting
        if (invertModifiersToggle != null)              invertModifiersToggle.GetComponent<Toggle>().isOn = invertModifiers;
        if (invertAbsoluteAToggle != null)              invertAbsoluteAToggle.GetComponent<Toggle>().isOn = invertAbsoluteA;
        if (invertAbsolutePToggle != null)              invertAbsolutePToggle.GetComponent<Toggle>().isOn = invertAbsoluteP;
        if (usingMultipleIdenticalSticksToggle != null)    usingMultipleIdenticalSticksToggle.GetComponent<Toggle>().isOn = usingMultipleIdenticalSticks;
        Debug.Log("<InputManager> \tLoadExtraOptions ended: invMods=" + invertModifiers + " invAbsA=" + invertAbsoluteA + " invAbsP=" + invertAbsoluteP + " usingMulti=" + usingMultipleIdenticalSticks);
    }

    /**
     * 
     * Called by the UI elements that change these options
     **/
    public static void SaveExtraOptions()
    {
        GetUIElements();
        Debug.Log("<InputManager> \tSaveExtraOptions called: invMods="+invertModifiers+" invAbsA="+invertAbsoluteA+" invAbsP="+invertAbsoluteP+" usingMulti="+usingMultipleIdenticalSticks);
        PlayerPrefs.SetInt("invertModifiers", invertModifiers ? 1 : 0);
        PlayerPrefs.SetInt("invertAbsoluteA", invertAbsoluteA ? 1 : 0);
        PlayerPrefs.SetInt("invertAbsoluteP", invertAbsoluteP ? 1 : 0);
        PlayerPrefs.SetInt("usingMultipleIdenticalSticks", usingMultipleIdenticalSticks ? 1 : 0);
    }

    /**********************************************************************\
    |                      SECTION: INPUT ACTION FUNCTIONS                 |
    \**********************************************************************/
    public static void Pause(InputAction.CallbackContext obj)
    {
        Debug.Log("Input: Pause");

    }
    public static void RebindMenu(InputAction.CallbackContext obj)
    {
        Debug.Log("Input: RebindMenu");
    }




}
