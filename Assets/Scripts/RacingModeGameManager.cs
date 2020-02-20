using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RacingModeGameManager : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public Transform[] instantiatePositions;
    public Text TimeUIText;
    public List<GameObject> lapTriggers  = new List<GameObject>();

    // Singleton implementation 
    public static RacingModeGameManager instance = null;
    private void Awake ()
    {
        // Check if instance already exists
        if (instance == null)
        {
            instance = this;
        }
        // If instance already exists and it is not this
        else if (instance != this)
        {
            // Destroy this. This enforces our singleton pattern, we can have only one singleton
            Destroy(gameObject);
        }
        // To not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
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
    }
}