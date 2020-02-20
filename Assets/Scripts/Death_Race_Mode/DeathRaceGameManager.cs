using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DeathRaceGameManager : MonoBehaviour
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
                int randomPosition = Random.Range(-16, 16);
                PhotonNetwork.Instantiate(carPrefabs[(int)carSelectionNumber].name, new Vector3(randomPosition, 0, randomPosition), Quaternion.identity);
            }
        }
    }
}