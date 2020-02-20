using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Lap Controller with 6 triggers located on a racing track, each of them has to be triggered in order to finish the lap
public class LapController : MonoBehaviourPun
{
    private List<GameObject> LapTriggers = new List<GameObject> ();
    private int finishOrder = 0;
    public enum RaisEventsCode
    {
        WhoFinishedEventCode = 0
    }

    // Start is called before the first frame update
    void Start ()
    {
        foreach (GameObject lapTrigger in RacingModeGameManager.instance.lapTriggers)
        {
            LapTriggers.Add (lapTrigger);
        }
    }

    #region Private Methods

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

    // This method is called when object LapController is being enabled and active
    private void OnEnable ()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }
    
    // This method is called when object LapController is being disabled and inactive
    private void OnDisable ()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent (EventData photonEvent)
    {
        if (photonEvent.Code == (byte) RaisEventsCode.WhoFinishedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string nickNameOfFinishedPlayer = (string)data[0];
            finishOrder = (int)data[1]; // 1 for winner, 2 for second place and etc.
            Debug.Log(nickNameOfFinishedPlayer + " " + finishOrder);
        }
    }

    // When all lap triggers are passed game is finished
    private void GameFinished ()
    {
        // Disable player movement and set car following camera to null
        GetComponent<PlayerSetup> ().playerCamera.transform.parent = null;
        GetComponent<CarMovement> ().enabled = false;

        finishOrder += 1;
        string nickName = photonView.Owner.NickName;
        object[] data = new object[] { nickName, finishOrder };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            // This event will be sent to all players in the current room
            Receivers = ReceiverGroup.All,
            // Event is kept in the room cache
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            // When set to false it overrides any current value
            Reliability = false
        };
        // Raising event to identify who won the race
        PhotonNetwork.RaiseEvent ((byte) RaisEventsCode.WhoFinishedEventCode, data, raiseEventOptions, sendOptions);
    }
    
    #endregion
}