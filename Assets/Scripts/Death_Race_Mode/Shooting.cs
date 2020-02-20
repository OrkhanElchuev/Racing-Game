using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePosition;
    public Camera playerCamera;
    public DeathRacePlayer deathRacePlayerProperties;

    // Update is called once per frame
    void Update ()
    {
        // If "space" key is pressed then shoot
        if (Input.GetKey ("space"))
        {
            Fire ();
        }
    }

    // Method for instantiating bullets
    public void Fire ()
    {
        // 0.5f, 0.5f for allocating ray in the middle of the screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        GameObject bulletGameObject = Instantiate(bulletPrefab, firePosition.position, Quaternion.identity);
        bulletGameObject.GetComponent<Bullet>().Initialize(ray.direction, deathRacePlayerProperties.bulletSpeed, deathRacePlayerProperties.damage);
    }
}