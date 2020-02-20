using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

// Death race mode game manager
public class DeathRaceGameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] carPrefabs;

    // Start is called before the first frame update
    void Start ()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object carSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue (MultiplayerRacingGame.CAR_SELECTION_NUMBER, out carSelectionNumber))
            {
                // Instantiating players on random position in a range of coordinates(-16, 16)
                int randomPosition = Random.Range (-16, 16);
                PhotonNetwork.Instantiate (carPrefabs[(int) carSelectionNumber].name, new Vector3 (randomPosition, 0, randomPosition), Quaternion.identity);
            }
        }
    }

    // Quit Match button is located inside Death Match Scene
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