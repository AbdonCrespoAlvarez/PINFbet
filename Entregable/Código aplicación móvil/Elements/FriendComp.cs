using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
Subobjectives: SO_6, SO_7, SO_9.
*/

public class FriendComp : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI friendName, coins;
    [SerializeField]
    private Image sprite;
    /* 
    Initializes the friend component with the given parameters.
    */
    public void Initialize(FriendData friend, Color col)
    {
        friendName.text = friend.username;
        coins.text = friend.coins.ToString();
        sprite.color = col;
    }
}