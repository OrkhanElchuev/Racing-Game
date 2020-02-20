using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    Rigidbody rb;
    public Vector3 thrustForce = new Vector3 (0f, 0f, 45f);
    public Vector3 rotationTorque = new Vector3 (0f, 8f, 0f);

    // Start is called before the first frame update
    void Start ()
    {
        rb = GetComponent<Rigidbody> ();
    }

    // Update is called once per frame
    void Update ()
    {
        // Moving forward via w key
        if (Input.GetKey ("w"))
        {
            rb.AddRelativeForce(thrustForce);
        }

        // Moving backward via s key
        if (Input.GetKey ("s"))
        {
            rb.AddRelativeForce(-thrustForce);
        }

        // Turn left via a key
        if (Input.GetKey ("a"))
        {
            rb.AddRelativeTorque(-rotationTorque);
        }

        // Turn right via d key
        if (Input.GetKey ("d"))
        {
            rb.AddRelativeTorque(rotationTorque);

        }
    }
}