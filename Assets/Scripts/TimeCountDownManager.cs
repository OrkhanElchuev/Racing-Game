using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TimeCountDownManager : MonoBehaviourPunCallbacks
{
    private Text timeUIText;
    private float timeToStartRace = 5.0f;

    private void Awake ()
    {
        timeUIText = RacingModeGameManager.instance.TimeUIText;
    }

    // Update is called once per frame
    void Update ()
    {
        // Only owner of the room will execute countdown, the rest players will experience the synchronization
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeToStartRace >= 0.0f)
            {
                timeToStartRace -= Time.deltaTime;
                photonView.RPC ("SetTime", RpcTarget.AllBuffered, timeToStartRace);
            }
            else if (timeToStartRace < 0.0f)
            {
                photonView.RPC ("StartRace", RpcTarget.AllBuffered);
            }
        }
    }

    // RPC method is created to synchronize countdown for all the players in the race
    [PunRPC]
    public void SetTime (float time)
    {
        if (time > 0.0f)
        {
            timeUIText.text = time.ToString ("F1");
        }
        else
        {
            // The countdown is over
            timeUIText.text = " ";
        }
    }

    [PunRPC]
    public void StartRace ()
    {
        // Enable car movement when race starts
        GetComponent<CarMovement> ().controlsEnabled = true;
        // Disable countdown script for all players when countdown is over
        this.enabled = false;
    }
}