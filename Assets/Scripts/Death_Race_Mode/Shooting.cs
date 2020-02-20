using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePosition;
    public Camera playerCamera;
    public DeathRacePlayer deathRacePlayerProperties;
    public LineRenderer lineRenderer;

    private float fireRate;
    private float fireTime = 0.0f;
    private bool useLaser;

    void Start ()
    {
        // Set the fire rate from scriptable object
        fireRate = deathRacePlayerProperties.fireRate;
        // Check if selected weapon is laser (it has different behaviour comparing to the rest weapons)
        if (deathRacePlayerProperties.weaponName == "Laser Gun")
        {
            useLaser = true;
        }
        else
        {
            useLaser = false;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        // If "space" key is pressed then shoot
        if (Input.GetKey ("space"))
        {
            if (fireTime > fireRate)
            {
                Fire ();
                fireTime = 0.0f;
            }
        }

        if (fireTime < fireRate)
        {
            fireTime += Time.deltaTime;
        }
    }

    // Method for instantiating bullets / rays
    public void Fire ()
    {
        if (useLaser)
        {
            // Laser code
            RaycastHit hit;
            // Ray is created from the middle of the screen
            Ray ray = playerCamera.ViewportPointToRay (new Vector3 (0.5f, 0.5f));
            // Shooting distance is set to 200
            if (Physics.Raycast (ray, out hit, 200))
            {
                if (!lineRenderer.enabled)
                {
                    lineRenderer.enabled = true;
                }

                // Laser's ray size will gradually decrease approaching the end
                lineRenderer.startWidth = 0.3f;
                lineRenderer.endWidth = 0.1f;

                // Fire from the fire position
                lineRenderer.SetPosition (0, firePosition.position);
                lineRenderer.SetPosition (1, hit.point);

                StopAllCoroutines ();
                // Disable laser ray after 0.3 seconds 
                StartCoroutine (DisableLaserAfterSecs (0.3f));
            }
        }
        else
        {
            // 0.5f, 0.5f for allocating ray in the middle of the screen
            Ray ray = playerCamera.ViewportPointToRay (new Vector3 (0.5f, 0.5f));
            GameObject bulletGameObject = Instantiate (bulletPrefab, firePosition.position, Quaternion.identity);
            bulletGameObject.GetComponent<Bullet> ().Initialize (ray.direction, deathRacePlayerProperties.bulletSpeed, deathRacePlayerProperties.damage);
        }
    }

    IEnumerator DisableLaserAfterSecs (float seconds)
    {
        yield return new WaitForSeconds (seconds);
        lineRenderer.enabled = false;
    }
}