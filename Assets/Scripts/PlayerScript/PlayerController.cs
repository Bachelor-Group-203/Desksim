using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2.0f;

    private Rigidbody rb;
    private UserInputActions userInput;
    private PlayerInput input;

    Vector3 newMoveDir;
    Vector3 moveDir;

    [SerializeField] private LayerMask groundLayer;
    private float checkDistance = 50.0f;

    private bool onGround = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if(GameObject.FindGameObjectWithTag("InputScripts"))
        {
            input = GameObject.FindGameObjectWithTag("InputScripts").GetComponent<PlayerInput>();
        } else
        {
            Debug.LogWarning("!!! InputScripts game object not found !!!");
        }

    }

    void Update()
    {
        //Debug.Log(GetGroundAngle());

    }

    private void FixedUpdate()
    {
        // Reading the input of the user multiple times each frame and turning it into the xz 
        moveDir = new Vector3(input.movement.x, 0, input.movement.y).normalized;

        //Debug.DrawLine(transform.position, transform.position + MovementVector(moveDir), Color.blue);
        newMoveDir = PlaneDownwardsDirection(moveDir);

        rb.velocity = newMoveDir * movementSpeed;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.transform.tag == "Ground")
    //    {
    //        onGround = true;
    //    }
    //}

    //private Vector3 MovementVector(Vector3 moveDir)
    //{
    //    float slopeAngle = GetGroundAngle();
    //    Vector3 slopeDir = GetSlopeDirection();


    //    if (slopeAngle == 0)
    //    {
    //        return new Vector3(moveDir.x, 0, moveDir.y) * movementSpeed;
    //    }
    //    if (slopeAngle > 0)
    //    {
    //        if (slopeDir.x + moveDir.x > slopeDir.x)
    //        {

    //        }
    //        else if (slopeDir.x + moveDir.x < slopeDir.x)
    //        {

    //        }
    //        else
    //        {

    //        }
    //    }

    //    return ;
    //}

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

    private Vector3 GetSlopeDirection()
    {
        RaycastHit hit;

        // Generate a ray that points down
        Ray ray = new Ray(transform.position, -transform.up);

        // If the Ray collides with an object in the layer specified
        if (Physics.Raycast(ray.origin, ray.direction, out hit, checkDistance, groundLayer))
        {
            // Returns the slope direction but no Y direction
            return new Vector3(hit.normal.x, 0, hit.normal.z);
        }
        else
        {
            return Vector3.up;
        }
    }

    /*
     * Finds the vector that is parallel with the plane and pointing in walking direction
     */
    private Vector3 PlaneDownwardsDirection(Vector3 moveDir)
    {
        // Making two rays
        RaycastHit hit1, hit2;

        // Making a guide for the ray to find another point on the plane
        Vector3 down = new Vector3(0.1f, -1, 0).normalized;

        // Make the rays project down on th plane
        Ray ray1 = new Ray(transform.position, -transform.up);
        Ray ray2 = new Ray(transform.position, down);

        float groundAngle = GetGroundAngle();

        // If both rays hit find the angle that the velocity vector should have
        if (Physics.Raycast(ray1.origin, ray1.direction, out hit1, checkDistance, groundLayer) && 
            Physics.Raycast(ray2.origin, ray2.direction, out hit2, checkDistance, groundLayer) &&
            groundAngle > 0 &&
            moveDir != Vector3.zero)
        {
            // Finds the vector parallel to the plane
            Vector3 planeVector = (hit1.point - hit2.point).normalized;

            // Find the downwards path by finding the angle between projected plane vector and the plane normal vector in 2d without y
            float angle = Vector2.Angle(new Vector2(planeVector.x, planeVector.z), new Vector2(hit1.normal.x, hit1.normal.z));
            Vector3 downVector = (Quaternion.AngleAxis(angle, hit1.normal) * planeVector).normalized;

            float moveDirAngle = Vector2.Angle(new Vector2(downVector.x, downVector.z), new Vector2(moveDir.x, moveDir.z));
            Vector3 newMoveDir = (Quaternion.AngleAxis(moveDirAngle, hit1.normal) * downVector).normalized;

            Vector2 a = new Vector2(newMoveDir.x, newMoveDir.z).normalized;
            Vector2 b = new Vector2(moveDir.x, moveDir.z).normalized;

            //Debug.Log(moveDirAngle);
            Debug.DrawLine(transform.position, transform.position + moveDir, Color.red);
            Debug.DrawLine(transform.position, transform.position + newMoveDir, Color.blue);

            // For finding rotatin the vector in the negative degree
            if (Mathf.Round(a.x * 10) != Mathf.Round(b.x * 10) || Mathf.Round(a.y * 10) != Mathf.Round(b.y * 10))
            {
                Debug.Log("Behandling");
                Debug.Log(a + "\t" + b);
                newMoveDir = (Quaternion.AngleAxis(-moveDirAngle, hit1.normal) * downVector).normalized;
            }

            return newMoveDir;
        }
        return moveDir;
    }
}
