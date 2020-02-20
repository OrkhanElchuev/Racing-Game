using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Bullet configurations
public class Bullet : MonoBehaviour
{
    float bulletDamage;

    // Initializing bullet 
    public void Initialize (Vector3 direction, float speed, float damage)
    {
        bulletDamage = damage;

        transform.forward = direction;
        Rigidbody rb = GetComponent<Rigidbody> ();

        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter (Collider collision)
    {
        // If bullet hits something, destroy itself
        Destroy (gameObject);
        // In case collision object is player then deal damage
        if (collision.gameObject.CompareTag ("Player"))
        {
            if (collision.gameObject.GetComponent<PhotonView> ().IsMine)
            {
                collision.gameObject.GetComponent<PhotonView> ().RPC ("DealDamage", RpcTarget.AllBuffered, bulletDamage);
            }
        }
    }
}