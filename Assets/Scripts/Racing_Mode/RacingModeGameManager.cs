﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Configurations for Racing game mode
public class RacingModeGameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] carPrefabs;
    public Transform[] instantiatePositions;
    public Text TimeUIText;
    public List<GameObject> lapTriggers = new List<GameObject> ();
    public GameObject[] FinishOrderUIGameObjects;

    // Singleton implementation 
    public static RacingModeGameManager instance = null;
    private void Awake ()
    {
        // Check if instance already exists
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            // Destroy this. This enforces our singleton pattern, we can have only one singleton
            Destroy (gameObject);
        }
        // To not be destroyed when reloading scene
        DontDestroyOnLoad (gameObject);
    }

    // Start is called before the first frame update
    void Start ()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            // Identifying the selected car number (0, 1, 2)
            object carSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue (MultiplayerRacingGame.CAR_SELECTION_NUMBER, out carSelectionNumber))
            {
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                // Instantiating players on different spots 
                Vector3 instantiatePos = instantiatePositions[actorNumber - 1].position;
                PhotonNetwork.Instantiate (carPrefabs[(int) carSelectionNumber].name, instantiatePos, Quaternion.identity);
            }
        }

        // Disable winners order UI when the race starts
        foreach (GameObject gm in FinishOrderUIGameObjects)
        {
            gm.SetActive (false);
        }
    }

    // Quit Match button is located inside Racing Mode Scene
    public void OnQuitMatchButtonClicked ()
    {
        // Leave the room
        PhotonNetwork.LeaveRoom ();
    }

    // Load Lobby scene when player leaves the room
    public override void OnLeftRoom ()
    {
        SceneManager.LoadScene ("LobbyScene");
    }
}