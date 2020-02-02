using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class PlayerListEntryInitializer : MonoBehaviour
{
    [Header("UI Reference")]
    public Text playerNameText;
    public Button playerReadyButton;
    public Image playerReadyImage;

    private bool isPlayerReady = false;

    // Initialize players
    public void Initialize(int playerID, string playerName)
    {
        playerNameText.text = playerName;
        // Show the relevant ready button according to the player ID
        if (PhotonNetwork.LocalPlayer.ActorNumber != playerID)
        {
            playerReadyButton.gameObject.SetActive(false);
        }
        else
        {
            ExitGames.Client.Photon.Hashtable initialProps =
                new ExitGames.Client.Photon.Hashtable() { { MultiplayerRacingGame.PLAYER_READY, isPlayerReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
            // Adding listener to Ready button and set room properties accordingly
            playerReadyButton.onClick.AddListener(() =>
            {
                isPlayerReady = !isPlayerReady;
                SetPlayerReady(isPlayerReady);

                ExitGames.Client.Photon.Hashtable newProps =
                    new ExitGames.Client.Photon.Hashtable() { { MultiplayerRacingGame.PLAYER_READY, isPlayerReady } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(newProps);
            });
        }
    }

    public void SetPlayerReady(bool playerReady)
    {
        playerReadyImage.enabled = playerReady;
        // Change the state text of Ready button 
        if (playerReady == true)
        {
            playerReadyButton.GetComponentInChildren<Text>().text = "Ready!";
        }
        else
        {
            playerReadyButton.GetComponentInChildren<Text>().text = "Ready?";
        }
    }
}

