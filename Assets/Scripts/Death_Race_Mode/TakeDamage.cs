using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TakeDamage : MonoBehaviourPun
{
    public float startHealth = 100f;
    private float health;
    public Image healthBar;

    public GameObject playerGraphics;
    public GameObject playerUI;
    public GameObject playerWeaponHolder;
    public GameObject deathPanelUIPrefab;
    private GameObject deathPanelUIGameObject;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start ()
    {
        rb = GetComponent<Rigidbody> ();
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
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // Deactivate player components of player after death
        playerGraphics.SetActive (false);
        playerUI.SetActive (false);
        playerWeaponHolder.SetActive (false);

        if (photonView.IsMine)
        {
            StartCoroutine (Respawn ());
        }
    }

    IEnumerator Respawn ()
    {
        GameObject canvasGameObject = GameObject.Find ("Canvas");
        if (deathPanelUIGameObject == null)
        {
            deathPanelUIGameObject = Instantiate (deathPanelUIPrefab, canvasGameObject.transform);
        }
        else
        {
            deathPanelUIGameObject.SetActive (true);
        }

        Text respawnTimeText = deathPanelUIGameObject.transform.Find ("RespawnTimeText").GetComponent<Text> ();
        float respawnTime = 7.0f;
        respawnTimeText.text = respawnTime.ToString (".00");

        // Wait for respawn time
        while (respawnTime > 0.0f)
        {
            yield return new WaitForSeconds (1.0f);
            respawnTime -= 1.0f;
            respawnTimeText.text = respawnTime.ToString (".00");
            // Disable shooting and movement of player
            GetComponent<CarMovement> ().enabled = false;
            GetComponent<Shooting> ().enabled = false;
        }
        // Deactivate death panel UI
        deathPanelUIGameObject.SetActive(false);
        // Enable shooting and movement of player after respawn time has passed
        GetComponent<CarMovement> ().enabled = true;
        GetComponent<Shooting> ().enabled = true;
        // Generate random number for spawning on random position
        int randomPoint = Random.Range (-20, 20);
        transform.position = new Vector3 (randomPoint, 0, randomPoint);
        // Respawn player for all the players in current room
        photonView.RPC ("Reborn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void Reborn ()
    {
        // Reset the health points
        health = startHealth;
        healthBar.fillAmount = health /  startHealth;
        // Activate player components for reborn
        playerGraphics.SetActive(true);
        playerUI.SetActive(true);
        playerWeaponHolder.SetActive(true);
    }
}