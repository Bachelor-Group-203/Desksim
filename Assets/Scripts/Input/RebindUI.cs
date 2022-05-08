using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindUI : MonoBehaviour
{
    [SerializeField]
    // Reference to an action from from the scriptable object InputActionAsset
    private InputActionReference inputActionReference; 
    
    [Range(0, 10)]
    [SerializeField]
    // Index for the binding, lets us scroll between the bindings for the current action
    private int selectedBinding;

    [SerializeField]
    // A built-in enum to format the names of the bindings
    private InputBinding.DisplayStringOptions displayStringOptions;

    // Information about the binding
    [Header("Read-only binding info")]
    [SerializeField]
    private InputBinding inputBinding; 
    private int bindingIndex;

    // The action name stored as a string, from the scriptable object.
    // Instead of rebind the scriptable object we look for the action in the C# input action class, the compiled class the InputActionAsset generates, by its original binding, and then return that action.
    private string actionName; 
    
    [Header("UI Fields")]
    [SerializeField]
    private TMPro.TMP_Text actionText;
    [SerializeField]
    private Button rebindButton;
    [SerializeField]
    private TMPro.TMP_Text rebindText;
    [SerializeField]
    private Button resetButton;

    /**
     * 
     **/
    private void OnEnable()
    {

        // Add listeners for DoRebind and ResetBinding
        rebindButton.onClick.AddListener(() => DoRebind());
        resetButton.onClick.AddListener(() => ResetBinding());

        
        // Make sure values are represented correctly in play mode 
        if(inputActionReference != null)
        {
            // Load binding override and update info
            if (actionName == null) GetBindingInfo();
            InputManager.LoadBindingOverride(actionName);
            GetBindingInfo();
            UpdateUI();
        }

        // Bind events
        InputManager.rebindComplete  += UpdateUI;
        InputManager.rebindCancelled += UpdateUI;

        // Select a UI element default, to navigate with a controller
        if(GameObject.FindGameObjectsWithTag("UI_SelectedByDefault")[0] != null) GameObject.FindGameObjectsWithTag("UI_SelectedByDefault")[0].GetComponent<Button>().Select();
    }
    
    private void OnDisable()
    {
        // Unbind events
        InputManager.rebindComplete  -= UpdateUI;
        InputManager.rebindCancelled -= UpdateUI;
    }

    /**
     * Called when something in the inspector changes
     **/
    private void OnValidate()
    {
        // Return if no input action reference selected in edit mode
        if (inputActionReference == null) return; 

        // Get info about binding and update the UI
        GetBindingInfo();
        UpdateUI();
    }


    /**
     * 
     * 
     * 
     **/
    private void GetBindingInfo()
    {
        // See that action exists and rebind
        if (inputActionReference.action != null) actionName = inputActionReference.action.name;


        // Check that selected binding index (under an action) is within the number of bindings
        if (inputActionReference.action.bindings.Count > selectedBinding)
        {
            inputBinding = inputActionReference.action.bindings[selectedBinding]; // For visual feedback
            bindingIndex = selectedBinding;
        }

    }


    private void UpdateUI()
    {

        if (actionText != null) actionText.text = actionName;
        if (rebindText != null)
        {
            // Gets from C# generated class if in play mode
            if (Application.isPlaying)
            {
                // Grab info from Input Manager
                // When app is in play mode, we need to grab the information off the C# script, not the scriptable object (SO)
                rebindText.text = InputManager.GetBindingName(actionName, bindingIndex);

            }
            else // Gets from scriptable object if in editor mode
                rebindText.text = inputActionReference.action.GetBindingDisplayString(bindingIndex);

        }
    }


    private void DoRebind()
    {
        InputManager.StartRebind(actionName, bindingIndex, rebindText);
    }

    private void ResetBinding()
    {
        InputManager.ResetBinding(actionName, bindingIndex);
        UpdateUI();
    }

}
