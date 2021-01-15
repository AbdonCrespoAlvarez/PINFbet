using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;
using Newtonsoft.Json;

/*
Subobjectives: SO_6, SO_7, SO_9.
*/

public class FriendListMenu : SimpleMenu<FriendListMenu>
{
	[SerializeField]
	private Transform friendList;
	[SerializeField]
	private GameObject friendPrefab, friendReqPrefab;
	[SerializeField]
	private TMP_InputField friendName;
	[SerializeField]
	private Color[] colorPattern;
	private int colorIndex;

	/* 
    This method updates the friendlist when the friend menu is opened.
    */
	public void Start()
	{
		UpdateFriendList();
	}
	/* 
    It makes a request to represent a friend request from the user to the username given.
    */
	public void OnAddFriendPressed()
	{
		RequestHandler.Post("addFriend", "{\"name\":\""+ friendName.text + "\"}", (result) =>
		{
			if (result.isNetworkError || result.isHttpError)
			{
				Debug.Log("Error Adding Friend: " + result.error);
			}
			else
			{
				
			}
			UpdateFriendList();
		});
	}
	/* 
   	Return to the profile menu.
    */
	public void OnProfilePressed()
	{
		Close();
		ProfileMenu.Open();
	}
	/* 
    Updates the friend list, first requests the friend-requests to be on top and then initializes the friends of the user,
	ranking them by their coins obtaine up to this moment.
    */
	private void UpdateFriendList() {
		colorIndex = 0;
		RequestHandler.Get("getFriendsRequests", (requestResult) =>
		{
			foreach (Transform child in friendList)
			{
				Destroy(child.gameObject);
			}
			if (requestResult.isNetworkError)
			{
				Debug.Log("Error Updating Friend Requests: " + requestResult.error);
			}
			else
			{
				Debug.LogWarning("Friends Req" + requestResult.downloadHandler.text);
				if (requestResult.downloadHandler.text == "\"none\"")
				{
					//Just no friend requests. Do nothing
				}
				else
				{
					List<string> requests = JsonConvert.DeserializeObject<List<string>>(requestResult.downloadHandler.text);
					foreach (string request in requests)
					{
						FriendRequestComp friendReqComp = Instantiate(friendReqPrefab, friendList).GetComponent<FriendRequestComp>();
						friendReqComp.Initialize(request, colorPattern[colorIndex++]);
						if (colorIndex == colorPattern.Length) colorIndex = 0;
					}
				}
				//Rest of friends
				RequestHandler.Get("getFriendsData", (friendsResult) =>
				{
					if (friendsResult.isNetworkError)
					{
						Debug.Log("Error Updating Friends: " + friendsResult.error);
					}
					else
					{
						Debug.LogWarning("Friends" + friendsResult.downloadHandler.text);
						FriendData[] friends = JsonConvert.DeserializeObject<FriendData[]>(friendsResult.downloadHandler.text);
						foreach (FriendData friend in friends)
						{
							FriendComp friendData = Instantiate(friendPrefab, friendList).GetComponent<FriendComp>();
							friendData.Initialize(friend, colorPattern[colorIndex++]);
							if (colorIndex == colorPattern.Length) colorIndex = 0;
						}
					}
				});
			}
		});
	}
}