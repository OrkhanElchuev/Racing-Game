using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapController : MonoBehaviour
{
    private List<GameObject> LapTriggers = new List<GameObject> ();

    // Start is called before the first frame update
    void Start ()
    {
        foreach (GameObject lapTrigger in RacingModeGameManager.instance.lapTriggers)
        {
            LapTriggers.Add (lapTrigger);
        }
    }

    // Check if player has passed through lap triggers
    private void OnTriggerEnter (Collider other)
    {
        if (LapTriggers.Contains (other.gameObject))
        {
            int indexOfTrigger = LapTriggers.IndexOf (other.gameObject);
            LapTriggers[indexOfTrigger].SetActive (false);
            // If last lap trigger is passed game is finished
            if (other.name == "FinishTrigger")
            {
                // Game is finished
                GameFinished ();
            }
        }
    }

    // When all lap triggers are passed game is finished
    void GameFinished ()
    {
        GetComponent<PlayerSetup>().playerCamera.transform.parent = null;
        GetComponent<CarMovement>().enabled = false;
    }
}