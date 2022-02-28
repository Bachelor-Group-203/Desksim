# [Back to index](./index.md)





## Takeaway (PR [`2a496df`](https://github.com/Bachelor-Group-203/Desksim/commit/2a496dfd91586d46792f8c6930b64eedc69ab599))

#### Scripts:

- **UserInputController** - Manages and interprets the input action map and its actions
- **InputManager** - Deals with rebinding and presistent loading and storing of rebinds.
- **RebindUI** - Attached to the Rebind Prefab, rebinds keys in coordination with InputManager

#### Prefabs:

- **InputPack** - Simplified way of importing inputs into a scene, has two objects with UserInputController and InputManager, everything necessary for input in a scene. 
- **Rebind Prefab** - A rebinding prompt for a single binding, selected through the action range and binding index slider in the inspector
- **Control Slider Prefab** - A slider used to show acceleration and pressure. To use, subscribe its OnChange Unity event to appropriate method under the scene's UserInput script

#### Scenes:

- **Menu_RebindScreen** - A screen that lets you rebind every action under the Train action map


## Notes
Input re-binding is made persistent by storing the the action-binding string to playerPrefs.
Re-binding is done with the Rebind Prefab, other features are demonstrated in the Menu_RebindScreen scene.
Import the InputPack prefab to use the input system, read its values using GetComponent<UserInputController> on the UserInput GameObject. Can also load the two scripts individually.