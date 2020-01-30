using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public GameObject loginUIPanel;
    public InputField playerNameInputField;

    [Header("Connecting Info Panel")]
    public GameObject connectingInfoUIPanel;

    [Header("Creating Room Info Panel")]
    public GameObject creatingRoomInfoUIPanel;

    [Header("GameOptions  Panel")]
    public GameObject gameOptionsUIPanel;

    [Header("Create Room Panel")]
    public GameObject createRoomUIPanel;
    public InputField roomNameInputField;
    private string gameMode;

    [Header("Inside Room Panel")]
    public GameObject insideRoomUIPanel;
    public Text roomInfoText;
    public GameObject playerListPrefab;
    public GameObject playerListContent;

    [Header("Join Random Room Panel")]
    public GameObject joinRandomRoomUIPanel;

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        // Activate login Panel when the game starts
        ActivatePanel(loginUIPanel.name);
    }

    #endregion

    #region Public Methods

    public void SetGameMode(string newGameMode)
    {
        gameMode = newGameMode;
    }

    // Activate relevant panel
    public void ActivatePanel(string panelNameToBeActivated)
    {
        loginUIPanel.SetActive(loginUIPanel.name.Equals(panelNameToBeActivated));
        connectingInfoUIPanel.SetActive(connectingInfoUIPanel.name.Equals(panelNameToBeActivated));
        creatingRoomInfoUIPanel.SetActive(creatingRoomInfoUIPanel.name.Equals(panelNameToBeActivated));
        createRoomUIPanel.SetActive(createRoomUIPanel.name.Equals(panelNameToBeActivated));
        gameOptionsUIPanel.SetActive(gameOptionsUIPanel.name.Equals(panelNameToBeActivated));
        joinRandomRoomUIPanel.SetActive(joinRandomRoomUIPanel.name.Equals(panelNameToBeActivated));
        insideRoomUIPanel.SetActive(insideRoomUIPanel.name.Equals(panelNameToBeActivated));
    }

    #endregion

    #region UI Callback methods

    // Join Room button is located in Game Options Panel
    public void OnJoinRandomRoomButtonClicked(string gameModeArg)
    {
        gameMode = gameModeArg;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties =
            new ExitGames.Client.Photon.Hashtable() { { "gameMode", gameModeArg } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    // Create room button is located in Create Room Panel
    public void OnCreateRoomButtonClicked()
    {
        // Check if game mode exists
        if (gameMode != null)
        {
            ActivatePanel(creatingRoomInfoUIPanel.name);
            // Assign roomName to the room name entered by user
            string roomName = roomNameInputField.text;
            if (string.IsNullOrEmpty(roomName))
            {
                // If room name is not entered generate random room name(e.g. Room 243)
                roomName = "Room " + Random.Range(1, 1000);
            }
            // Set room configurations
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 3;

            string[] roomPropertiesInLobby = { "gameMode" };
            // Create new hashtable in Photon server
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gameMode", gameMode } };
            // Assign custom room properties
            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;
            // Create room with relevant name and option
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    // Cancel button is located in Create Room Panel
    public void OnCancelButtonClicked()
    {
        ActivatePanel(gameOptionsUIPanel.name);
    }

    // Login button is located in Login Panel 
    public void OnLoginButtonClicked()
    {
        // Assign playerName to the player name entered by user
        string playerName = playerNameInputField.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            // Make transition to Connecting panel till connection to Photon Server is established
            ActivatePanel(connectingInfoUIPanel.name);
            if (!PhotonNetwork.IsConnected)
            {
                // Set the player name in server and connect 
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("Player name is Invalid");
        }
    }

    // Back button is located in Join Random Room Panel
    public void OnBackButtonClicked()
    {
        ActivatePanel(gameOptionsUIPanel.name);
    }

    #endregion

    #region Photon Callbacks

    // On connected to the Internet
    public override void OnConnected()
    {
        Debug.Log("Connected to the Internet");
    }

    // On connected to the Photon Server
    public override void OnConnectedToMaster()
    {
        ActivatePanel(gameOptionsUIPanel.name);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to the server");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name +
            "Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        ActivatePanel(insideRoomUIPanel.name);
        // Check if room contains a game mode (e.g. Racing, DeathMatch)
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gameMode"))
        {
            roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " +
                " Players/Max.Players: " + PhotonNetwork.CurrentRoom.PlayerCount + " / " +
                PhotonNetwork.CurrentRoom.MaxPlayers;
            // List newly joined players in the table
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                GameObject playerListGameObject = Instantiate(playerListPrefab);
                playerListGameObject.transform.SetParent(playerListContent.transform);
                playerListGameObject.transform.localScale = Vector3.one;
                playerListGameObject.GetComponent<PlayerListEntryInitializer>().Initialize(player.ActorNumber, player.NickName);
            }
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // If room doesnt exist, create one
        if (gameMode != null)
        {
            // Assign roomName to the room name entered by user
            string roomName = roomNameInputField.text;
            if (string.IsNullOrEmpty(roomName))
            {
                // If room name is not entered generate random room name(e.g. Room 243)
                roomName = "Room " + Random.Range(1, 1000);
            }
            // Set room configurations
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 3;

            string[] roomPropertiesInLobby = { "gameMode" };
            // Create new hashtable in Photon server
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gameMode", gameMode } };
            // Assign custom room properties
            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;
            // Create room with relevant name and option
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    #endregion
}