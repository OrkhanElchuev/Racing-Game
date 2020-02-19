using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RacingModeGameManager : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public Transform[] instantiatePositions;

    // Start is called before the first frame update
    void Start ()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            // Identifying the selected car number (0, 1, 2)
            object carSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue (MultiplayerRacingGame.CAR_SELECTION_NUMBER, out carSelectionNumber))
            {
                Debug.Log((int)carSelectionNumber);
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                // Instantiating players on different spots 
                Vector3 instantiatePos = instantiatePositions[actorNumber - 1].position;
                PhotonNetwork.Instantiate(carPrefabs[(int)carSelectionNumber].name, instantiatePos, Quaternion.identity);
            }
        }
    }
}