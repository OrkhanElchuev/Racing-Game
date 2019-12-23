using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public GameObject loginUIPanel;
    public InputField playerNameInput;

    [Header("Connecting Info Panel")]
    public GameObject connectingInfoUIPanel;

    [Header("Creating Room Info Panel")]
    public GameObject creatingRoomInfoUIPanel;

    [Header("GameOptions  Panel")]
    public GameObject gameOptionsUIPanel;

    [Header("Create Room Panel")]
    public GameObject createRoomUIPanel;

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

    // Login button is located in Login Panel 
    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            ActivatePanel(connectingInfoUIPanel.name);
            if (!PhotonNetwork.IsConnected)
            {
                // Set the player name in network and connect 
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

    #endregion
}
