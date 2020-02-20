using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Player initial setup
public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera playerCamera;
    public TextMeshProUGUI playerNameText;

    // Start is called before the first frame update
    void Start ()
    {
        // Avoid multiple camera audio listeners in one room issue
        FindObjectOfType<Camera> ().GetComponent<AudioListener> ().enabled = false;

        // Configurations for racing mode
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue ("rc"))
        {
            // Avoid controlling other players in the room, make sure to control only your own player
            if (photonView.IsMine)
            {
                GetComponent<CarMovement> ().enabled = true;
                GetComponent<LapController> ().enabled = true;
                playerCamera.enabled = true;
            }
            else
            {
                GetComponent<CarMovement> ().enabled = false;
                GetComponent<LapController> ().enabled = false;
                playerCamera.enabled = false;
            }
        }
        // Configurations for death race mode
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue ("dr"))
        {
            if (photonView.IsMine)
            {
                GetComponent<CarMovement> ().enabled = true;
                GetComponent<CarMovement> ().controlsEnabled = true;
                playerCamera.enabled = true;
            }
            else
            {
                GetComponent<CarMovement> ().enabled = false;
                playerCamera.enabled = false;
            }
        }

        SetPlayerUI ();
    }

    // Setting player nick name
    private void SetPlayerUI ()
    {
        // Avoid null reference exception
        if (playerNameText != null)
        {
            playerNameText.text = photonView.Owner.NickName;
            // Disable nickname field if player is myself
            if (photonView.IsMine)
            {
                playerNameText.gameObject.SetActive (false);
            }
        }
    }
}