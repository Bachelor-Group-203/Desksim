using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2.0f;

    private Rigidbody rb;
    private UserInputActions userInput;

    [SerializeField] private LayerMask groundLayer;
    private float checkDistance = 50.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        userInput = new UserInputActions();
        userInput.PlayerAvatar.Enable();
    }

    void Update()
    {
        Debug.Log(GetGroundAngle());
    }

    private void FixedUpdate()
    {
        // Reading the input of the user multiple times each frame 
        Vector2 movDir = userInput.PlayerAvatar.Movement.ReadValue<Vector2>();
        rb.velocity = new Vector3(movDir.x, 0, movDir.y) * movementSpeed;
    }

    private Vector3 MovementVector(Vector3 moveDir)
    {
        return Vector3.zero;
    }

    private float GetGroundAngle()
    {
        RaycastHit hit;
        float groundAngle = -1.0f;

        // Generate a ray that pints down
        Ray ray = new Ray(transform.position, -transform.up);

        // If the Ray collides with an object in the layer specified
        if(Physics.Raycast(ray.origin, ray.direction, out hit, checkDistance, groundLayer))
        {
            // Finds the angle between the ground normal and the up vector
            groundAngle = Vector3.Angle(transform.up, hit.normal);
        }
        return groundAngle;
    }
}
