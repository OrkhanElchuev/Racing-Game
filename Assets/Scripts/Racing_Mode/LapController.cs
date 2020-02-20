using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

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
            object[] data = (object[]) photonEvent.CustomData;
            string nickNameOfFinishedPlayer = (string) data[0];
            finishOrder = (int) data[1]; // data[1] represents finish order
            int viewID = (int) data[2]; // data[2] represents viewID

            // Set the player winning order and activate the UI when player finishes the race
            GameObject orderUITextGameObject = RacingModeGameManager.instance.FinishOrderUIGameObjects[finishOrder - 1];
            orderUITextGameObject.SetActive (true);

            // The player who has finished the game is myself
            if (viewID == photonView.ViewID)
            {
                orderUITextGameObject.GetComponent<Text> ().text = finishOrder + ". " + nickNameOfFinishedPlayer + " (YOU)";
                orderUITextGameObject.GetComponent<Text> ().color = Color.green;
            }
            // Another player has finished the race 
            else
            {
                orderUITextGameObject.GetComponent<Text> ().text = finishOrder + ". " + nickNameOfFinishedPlayer;
            }
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
        int viewID = photonView.ViewID;
        // Event data
        object[] data = new object[] { nickName, finishOrder, viewID };

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