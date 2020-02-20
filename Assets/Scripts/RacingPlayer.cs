using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To be able to create scriptable objects via Unity Editor
[CreateAssetMenu(fileName = "New Racing Player")]
public class RacingPlayer : ScriptableObject
{
    public string playerName;
    public Sprite playerSprite;
}
