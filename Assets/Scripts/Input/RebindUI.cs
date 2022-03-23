using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindUI : MonoBehaviour
{
    [SerializeField]
    private InputActionReference inputActionReference; // This is part of the SO (scriptable object)

    [Range(0, 10)]
    [SerializeField]
    private int selectedBinding;

    [SerializeField]
    private InputBinding.DisplayStringOptions displayStringOptions; // Built in enum that lets us format the binding name

    [Header("Binding Info - DON'T EDIT")]
    [SerializeField]
    private InputBinding inputBinding; // To help see what binding we have 
    private int bindingIndex;

    private string actionName; // The action name stored as a string, from the SO, we dont want to rebind the SO so we're sending it over to the input manager, which looks up the action on the C# input action class, we find it with the original binding, and return that action.

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

        rebindButton.onClick.AddListener(() => DoRebind());
        resetButton.onClick.AddListener(() => ResetBinding());

        
        // Make sure values are represented correctly in play mode 
        if(inputActionReference != null)
        {
            if (actionName == null) GetBindingInfo();
            InputManager.LoadBindingOverride(actionName);
            GetBindingInfo();
            UpdateUI();
        }

        InputManager.rebindComplete  += UpdateUI;
        InputManager.rebindCancelled += UpdateUI;

        if(GameObject.FindGameObjectsWithTag("UI_SelectedByDefault")[0] != null) GameObject.FindGameObjectsWithTag("UI_SelectedByDefault")[0].GetComponent<Button>().Select();
    }
    
    private void OnDisable()
    {
        InputManager.rebindComplete  -= UpdateUI;
        InputManager.rebindCancelled -= UpdateUI;
    }

    /**
     * Called when something in the inspector changes
     **/
    private void OnValidate()
    {

        if (inputActionReference == null) return; // Return if no input action reference selected in edit mode
        GetBindingInfo();
        UpdateUI();

    }

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
