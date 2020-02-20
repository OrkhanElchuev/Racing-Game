using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Initializing bullet 
    public void Initialize (Vector3 direction, float speed, float damage)
    {
        transform.forward = direction;
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.velocity = direction * speed;
    }
}