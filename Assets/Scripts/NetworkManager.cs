using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

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

    [Header("Inside Room Panel")]
    public GameObject insideRoomUIPanel;

    [Header("Join Random Room Panel")]
    public GameObject joinRandomRoomUIPanel;


    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        // Activate login Panel when the game starts
        ActivatePanel(loginUIPanel.name);
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion

    #region Public Methods

    // Activate relevant panel
    public void ActivatePanel(string panelNameToBeActivated)
    {
        loginUIPanel.SetActive(loginUIPanel.name.Equals(panelNameToBeActivated));
        connectingInfoUIPanel.SetActive(connectingInfoUIPanel.name.Equals(panelNameToBeActivated));
        creatingRoomInfoUIPanel.SetActive(creatingRoomInfoUIPanel.name.Equals(panelNameToBeActivated));
        createRoomUIPanel.SetActive(createRoomUIPanel.name.Equals(panelNameToBeActivated));
        gameOptionsUIPanel.SetActive(gameOptionsUIPanel.name.Equals(panelNameToBeActivated));
        joinRandomRoomUIPanel.SetActive(joinRandomRoomUIPanel.name.Equals(panelNameToBeActivated));
    }

    #endregion

    #region UI Callback methods

    // Create room button is located in Create Room Panel
    public void OnCreateRoomButtonClicked()
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
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable()
            {{"gameMode", "racing"}};

        roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
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
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        // Check if room contains a game mode (e.g. Racing, DeathMatch)
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gameMode"))
        {
            object gameModeName;
            // Get the reference to the game mode name and assign it to gameModeName object
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out gameModeName))
            {
                Debug.Log(gameModeName.ToString());
            }
        }
    }

    #endregion
}
