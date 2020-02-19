using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header ("Login UI")]
    public GameObject loginUIPanel;
    public InputField playerNameInputField;

    [Header ("Connecting Info Panel")]
    public GameObject connectingInfoUIPanel;

    [Header ("Creating Room Info Panel")]
    public GameObject creatingRoomInfoUIPanel;

    [Header ("GameOptions  Panel")]
    public GameObject gameOptionsUIPanel;

    [Header ("Create Room Panel")]
    public GameObject createRoomUIPanel;
    public InputField roomNameInputField;
    private string gameMode;

    [Header ("Inside Room Panel")]
    public GameObject insideRoomUIPanel;
    public Text roomInfoText;
    public GameObject playerListPrefab;
    public GameObject playerListContent;
    public GameObject startGameButton;
    public Text GameModeText;
    public Image PanelBackground;
    public Sprite RacingBackground;
    public Sprite DeathRaceBackground;

    [Header ("Join Random Room Panel")]
    public GameObject joinRandomRoomUIPanel;

    private Dictionary<int, GameObject> playerListGameObjects;

    // Methods

    #region Unity Methods

    // Start is called before the first frame update
    void Start ()
    {
        // Activate login Panel when the game starts
        ActivatePanel (loginUIPanel.name);
        // Synchronize the scene in network for all players
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #endregion

    #region Private Methods

    // To (enable/disable) the status of "Start button" according to the players' ready status
    private bool CheckPlayersReadyStatus ()
    {
        // If not a master client
        if (!PhotonNetwork.IsMasterClient)
        {
            // return false to disable "Start button"
            return false;
        }
        // Check if all players are ready, if one of them is not ready, return false to disable "Start button"
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (player.CustomProperties.TryGetValue (MultiplayerRacingGame.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool) isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    #endregion

    #region Public Methods

    // Update text information about room name. current player count in Inside Room Panel
    public void UpdateRoomInfoText ()
    {
        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " +
            " Players/Max.Players: " + PhotonNetwork.CurrentRoom.PlayerCount + " / " +
            PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    // Update background image and text according to the game mode "racing", "death race"
    public void UpdateBackgroundAndText ()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue ("rc"))
        {
            GameModeText.text = "LET'S RACE!";
            PanelBackground.sprite = RacingBackground;
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue ("dr"))
        {
            GameModeText.text = "DEATH RACE!";
            PanelBackground.sprite = DeathRaceBackground;
        }
    }

    public void SetGameMode (string newGameMode)
    {
        gameMode = newGameMode;
    }

    // Activate relevant panel
    public void ActivatePanel (string panelNameToBeActivated)
    {
        loginUIPanel.SetActive (loginUIPanel.name.Equals (panelNameToBeActivated));
        connectingInfoUIPanel.SetActive (connectingInfoUIPanel.name.Equals (panelNameToBeActivated));
        creatingRoomInfoUIPanel.SetActive (creatingRoomInfoUIPanel.name.Equals (panelNameToBeActivated));
        createRoomUIPanel.SetActive (createRoomUIPanel.name.Equals (panelNameToBeActivated));
        gameOptionsUIPanel.SetActive (gameOptionsUIPanel.name.Equals (panelNameToBeActivated));
        joinRandomRoomUIPanel.SetActive (joinRandomRoomUIPanel.name.Equals (panelNameToBeActivated));
        insideRoomUIPanel.SetActive (insideRoomUIPanel.name.Equals (panelNameToBeActivated));
    }

    #endregion

    #region UI Callback methods

    // Join Room button is located in Game Options Panel
    public void OnJoinRandomRoomButtonClicked (string gameModeArg)
    {
        gameMode = gameModeArg;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties =
            new ExitGames.Client.Photon.Hashtable ()
            {
                {
                "gm",
                gameModeArg
                }
            };
        PhotonNetwork.JoinRandomRoom (expectedCustomRoomProperties, 0);
    }

    // Create room button is located in Create Room Panel
    public void OnCreateRoomButtonClicked ()
    {
        // Check if game mode exists
        if (gameMode != null)
        {
            ActivatePanel (creatingRoomInfoUIPanel.name);
            // Assign roomName to the room name entered by user
            string roomName = roomNameInputField.text;
            if (string.IsNullOrEmpty (roomName))
            {
                // If room name is not entered generate random room name(e.g. Room 243)
                roomName = "Room " + Random.Range (1, 1000);
            }
            // Set room configurations
            RoomOptions roomOptions = new RoomOptions ();
            roomOptions.MaxPlayers = 3;
            string[] roomPropertiesInLobby = {
                "gm"
            }; // gm - game mode

            // Create new hashtable in Photon server
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable ()
            {
                {
                "gm",
                gameMode
                }
            }; // rc - racing
            // Assign custom room properties
            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;
            // Create room with relevant name and option
            PhotonNetwork.CreateRoom (roomName, roomOptions);
        }
    }

    // Cancel button is located in Create Room Panel
    public void OnCancelButtonClicked ()
    {
        ActivatePanel (gameOptionsUIPanel.name);
    }

    // Login button is located in Login Panel 
    public void OnLoginButtonClicked ()
    {
        // Assign playerName to the player name entered by user
        string playerName = playerNameInputField.text;
        if (!string.IsNullOrEmpty (playerName))
        {
            // Make transition to Connecting panel till connection to Photon Server is established
            ActivatePanel (connectingInfoUIPanel.name);
            if (!PhotonNetwork.IsConnected)
            {
                // Set the player name in server and connect 
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings ();
            }
        }
        else
        {
            Debug.Log ("Player name is Invalid");
        }
    }

    // Back button is located in Join Random Room Panel
    public void OnBackButtonClicked ()
    {
        ActivatePanel (gameOptionsUIPanel.name);
    }

    // Leave Game button is located in Inside Room Panel
    public void OnLeaveGameButtonClicked ()
    {
        PhotonNetwork.LeaveRoom ();
    }

    // Start Game button is located in Inside Room panel
    public void OnStartGameButtonClicked ()
    {
        // Check if game mode is selected and load relevant game scene
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey ("gm"))
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue ("rc"))
            {
                PhotonNetwork.LoadLevel("RacingScene");
            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue ("dr"))
            {
                PhotonNetwork.LoadLevel("DeathRaceScene");
            }
        }
    }

    #endregion

    #region Photon Callbacks

    // On connected to the Internet
    public override void OnConnected ()
    {
        Debug.Log ("Connected to the Internet");
    }

    // On connected to the Photon Server
    public override void OnConnectedToMaster ()
    {
        ActivatePanel (gameOptionsUIPanel.name);
        Debug.Log (PhotonNetwork.LocalPlayer.NickName + " is connected to the server");
    }

    // Updating player "Ready" status and making it visible for all the players in room
    public override void OnPlayerPropertiesUpdate (Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        GameObject playerListGameObject;
        if (playerListGameObjects.TryGetValue (targetPlayer.ActorNumber, out playerListGameObject))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue (MultiplayerRacingGame.PLAYER_READY, out isPlayerReady))
            {
                playerListGameObject.GetComponent<PlayerListEntryInitializer> ().SetPlayerReady ((bool) isPlayerReady);
            }
        }
        startGameButton.SetActive (CheckPlayersReadyStatus ());
    }

    public override void OnCreatedRoom ()
    {
        Debug.Log (PhotonNetwork.CurrentRoom.Name + " is created.");
    }

    // When player joins room
    public override void OnJoinedRoom ()
    {
        Debug.Log (PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name +
            "Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        ActivatePanel (insideRoomUIPanel.name);
        // Check if room contains a game mode (e.g. Racing, DeathMatch)
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey ("gm"))
        {
            // Updating information and background picture
            UpdateRoomInfoText ();
            UpdateBackgroundAndText ();

            if (playerListGameObjects == null)
            {
                playerListGameObjects = new Dictionary<int, GameObject> ();
            }

            // List newly joined players in the table
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                GameObject playerListGameObject = Instantiate (playerListPrefab);
                playerListGameObject.transform.SetParent (playerListContent.transform);
                playerListGameObject.transform.localScale = Vector3.one;
                // ActorNumber stands for ID number of player
                playerListGameObject.GetComponent<PlayerListEntryInitializer> ().Initialize (player.ActorNumber, player.NickName);

                // Update the status of players in list (ready? / ready!)
                object isPlayerReady;
                if (player.CustomProperties.TryGetValue (MultiplayerRacingGame.PLAYER_READY, out isPlayerReady))
                {
                    playerListGameObject.GetComponent<PlayerListEntryInitializer> ().SetPlayerReady ((bool) isPlayerReady);
                }
                // Add player to the list of player game objects
                playerListGameObjects.Add (player.ActorNumber, playerListGameObject);
            }
        }
        // Start button is activated only if all players are ready(initially deactivated)
        startGameButton.SetActive (false);
    }

    // Another player joins room 
    public override void OnPlayerEnteredRoom (Player newPlayer)
    {
        UpdateRoomInfoText ();
        GameObject playerListGameObject = Instantiate (playerListPrefab);
        playerListGameObject.transform.SetParent (playerListContent.transform);
        playerListGameObject.transform.localScale = Vector3.one;
        playerListGameObject.GetComponent<PlayerListEntryInitializer> ().Initialize (newPlayer.ActorNumber, newPlayer.NickName);

        playerListGameObjects.Add (newPlayer.ActorNumber, playerListGameObject);
        startGameButton.SetActive (CheckPlayersReadyStatus ());
    }

    // Another player leaves the room 
    public override void OnPlayerLeftRoom (Player otherPlayer)
    {
        UpdateRoomInfoText ();
        // Destroy game object of player, who left from our room, and remove him from player list
        Destroy (playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove (otherPlayer.ActorNumber);
        startGameButton.SetActive (CheckPlayersReadyStatus ());
    }

    // We leave the room
    public override void OnLeftRoom ()
    {
        ActivatePanel (gameOptionsUIPanel.name);
        // Destroy every game object in player list
        foreach (GameObject playerListGameObject in playerListGameObjects.Values)
        {
            Destroy (playerListGameObject);
        }
        // Clear the content of player list objects 
        playerListGameObjects.Clear ();
        playerListGameObjects = null;
    }

    public override void OnJoinRandomFailed (short returnCode, string message)
    {
        // If room doesnt exist, create one
        if (gameMode != null)
        {
            // Assign roomName to the room name entered by user
            string roomName = roomNameInputField.text;
            if (string.IsNullOrEmpty (roomName))
            {
                // If room name is not entered generate random room name(e.g. Room 243)
                roomName = "Room " + Random.Range (1, 1000);
            }
            // Set room configurations
            RoomOptions roomOptions = new RoomOptions ();
            roomOptions.MaxPlayers = 3;

            string[] roomPropertiesInLobby = {
                "gm"
            };
            // Create new hashtable in Photon server
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable ()
            {
                {
                "gm",
                gameMode
                }
            };
            // Assign custom room properties
            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;
            // Create room with relevant name and option
            PhotonNetwork.CreateRoom (roomName, roomOptions);
        }
    }

    // Method is called when master client leaves the room, hence another player becomes master client
    public override void OnMasterClientSwitched (Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            startGameButton.SetActive (CheckPlayersReadyStatus ());
        }
    }

    #endregion
}