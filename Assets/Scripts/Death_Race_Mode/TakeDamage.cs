using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TakeDamage : MonoBehaviour
{
    public float startHealth = 100f;
    private float health;
    public Image healthBar;

    // Start is called before the first frame update
    void Start ()
    {
        health = startHealth;

        healthBar.fillAmount = health / startHealth;
    }

    [PunRPC]
    public void DealDamage (float damage)
    {
        health -= damage;
        healthBar.fillAmount = health / startHealth;

        if (health <= 0f)
        {
            Die ();
        }
    }

    private void Die ()
    {

    }
}