using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
Subobjectives: SO_6, SO_9.
*/

public class FriendRequestComp : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI friendName;
    [SerializeField]
    private Image sprite;
	/* 
    Initializes the subject component with the given parameters, it also assigns the Menu which holds the list of selcted
    subjects.
    */
	public void Initialize(string name, Color col)
    {
		friendName.text = name;
        sprite.color = col;
    }
	/* 
    When the user accepts the friend request, this method is called. It sends a friend request to the other user to
	represent that they have both accepted on being friends with eachother.
    */
	public void OnAcceptRequest()
	{
		RequestHandler.Post("acceptFriend", "{\"name\":\"" + friendName.text + "\"}", (result) =>
		{
			if (result.isNetworkError)
			{
				Debug.Log("Error Adding Friend: " + result.error);
			}
			else
			{

			}
		});
	}
	/* 
    When the user rejects the friend request, this method is called. It sends a petition for the server to remove
	the friend request.
    */
	public void OnRejectRequest()
	{
		RequestHandler.Post("rejectFriend", "{\"name\":\"" + friendName.text + "\"}", (result) =>
		{
			if (result.isNetworkError)
			{
				Debug.Log("Error Adding Friend: " + result.error);
			}
			else
			{

			}
		});
	}
}