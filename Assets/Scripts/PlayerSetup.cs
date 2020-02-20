using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera playerCamera;

    // Start is called before the first frame update
    void Start ()
    {
        // Avoid controlling other players in the room, make sure to control only your own player
        if (photonView.IsMine)
        {
            GetComponent<CarMovement> ().enabled = true;
            playerCamera.enabled = true;
        }
        else
        {
            GetComponent<CarMovement> ().enabled = false;
            playerCamera.enabled = false;
        }
    }
}