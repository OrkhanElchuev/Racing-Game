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

    public void Initialize(int playerID, string playerName)
    {
        playerNameText.text = playerName;
    }

}
