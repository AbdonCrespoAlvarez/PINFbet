using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Newtonsoft.Json;
using TMPro;

/*
Subobjectives: SO_5, SO_9.
*/

public class NewBetMenu : Menu<NewBetMenu>
{
	[SerializeField]
	private TMP_Dropdown studentsDrop, subjectsDrop;
	[SerializeField]
	private TMP_InputField coins, mark;
	[SerializeField]
	private TextMeshProUGUI coinsText;

	List<string> friendList;
	List<List<string>> optToSubj;

	/* 
    Requests information about the user's data and bets made to display them on the screen.
	It can be used as a repetitive coroutine based on an interval to keep the data updated.
    */
	public void Start()
	{
		optToSubj = new List<List<string>>();
		RequestHandler.Get("getPosibleBets/" + RequestHandler.sessionUsername, (myBets) =>
		{
			Debug.LogWarning("MyBets: " + myBets.downloadHandler.text);
			List<string> mySubjects = JsonConvert.DeserializeObject<List<string>>(myBets.downloadHandler.text);
			optToSubj.Add( mySubjects);
			RequestHandler.Get("getFriends", (friends) =>
			{
				Debug.LogWarning("Friends: " + friends.downloadHandler.text);
				if (friends.downloadHandler.text == "\"none\"") { }
				else
				{
					friendList = JsonConvert.DeserializeObject<List<string>>(friends.downloadHandler.text);
					studentsDrop.AddOptions(friendList);
					foreach (string friendName in friendList)
					{
						RequestHandler.Get("getPosibleBets/" + friendName, (friendBet) =>
						{
							Debug.LogWarning(friendName + "Bets " + friendBet.downloadHandler.text);
							List<string> friendSubjects = new List<string>();
							friendSubjects = JsonConvert.DeserializeObject<List<string>>(friendBet.downloadHandler.text);
							optToSubj.Add(friendSubjects);
						});
					}
				}
			});
			OnStudentChanged(0);
		});
	}

	/* 
    This method is called to open the Menu to create new bets, it also displays the coins of the user
    */
	public static void Show(string coins)
	{
		Open();
		Instance.coinsText.text = coins;
	}

	/* 
    It is called each time the user selects a new student or itselfs, it changes the posible subjects accordingly
    */
	public void OnStudentChanged(int opt)
	{
		subjectsDrop.ClearOptions();
		subjectsDrop.AddOptions(optToSubj[opt]);
	}
	/* 
    Creates a BetData object and sends it in a POST request, then handles the answer received
    */
	public void OnConfirmedPressed()
	{
		string username;
		string subject = subjectsDrop.captionText.text;
		if (studentsDrop.value == 0) username = RequestHandler.sessionUsername;
		else username = studentsDrop.captionText.text;
		BetData newBet = new BetData(username, subject, int.Parse(coins.text), float.Parse(mark.text));
		RequestHandler.Post("addBet", newBet, (result) =>
		{
			if (result.isNetworkError) { 

			}else if (result.isHttpError) {
				Debug.LogWarning("Not Coins!!");
			}
			else{
				optToSubj[subjectsDrop.value].Remove(subject);
				//Update local user data
				string newCoins = (int.Parse(coinsText.text) - newBet.coins).ToString();
				ProfileMenu.coinsText.text = newCoins;
				coinsText.text = newCoins;
			}
			ExitMenu();
		});
	}
	/* 
    Closes the new bet menu
    */
	public void ExitMenu()
	{
		ProfileMenu.Open();
	}
}
