using System.Collections;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;
using Newtonsoft.Json;

/*
Subobjectives: SO_4, SO_5, SO_9.
*/

public class ProfileMenu : SimpleMenu<ProfileMenu>
{
	[SerializeField]
	private Transform betList;
	[SerializeField]
	private GameObject betPrefab;
	[SerializeField]
	private TextMeshProUGUI tempCoins;
	public static TextMeshProUGUI coinsText;
	[SerializeField]
	private float UpdateInterval;
	[SerializeField]
	private Color[] colorPattern;
	private static int colorIndex;

	/* 
    Starts the Update coroutine, making it repeat itself until the menu is closed.
    */
	public void Start()
	{
		coinsText = tempCoins;
		StartCoroutine(CheckUpdates(UpdateInterval, true));
	}

	/* 
    Opens the Menu to create and send a new Bet.
    */
	public void OnNewBetPressed()
	{
		NewBetMenu.Show(coinsText.text);
	}
	/* 
    Opens the friend list and menu.
    */
	public void OnFriendsPressed()
	{
		Close();
		FriendListMenu.Open();
	}
	/* 
    Opens the multiple menus which allows the user to inform of new subjects and marks obtained.
    */
	public void OnUpdateSubjectsPressed()
	{
		Close();
		InputSubjectsMenu.Open();
	}
	/* 
    Requests information about the user's data and bets made to display them on the screen.
	It can be used as a repetitive coroutine based on an interval to keep the data updated.
    */
	private IEnumerator CheckUpdates(float RepeatInterval, bool repeat)
    {
		while (true)
		{
			Debug.Log("Updating");
			RequestHandler.Get("getCoins", (result) => {
				if (result.isNetworkError)
				{
					coinsText.text = "error cargando perfil";
				}
				else
				{
					coinsText.text = result.downloadHandler.text;
				}
			});
			RequestHandler.Get("getBets", (result) => {
				if (result.isNetworkError)
				{
					Debug.Log("Error Updating Bets: " + result.error);
				}
				else
				{
					colorIndex = 0;
					foreach (Transform child in betList)
					{
						Destroy(child.gameObject);
					}
					BetData[] bets = JsonConvert.DeserializeObject<BetData[]>(result.downloadHandler.text);
					foreach (BetData bet in bets)
					{
						BetComp betData = Instantiate(betPrefab, betList).GetComponent<BetComp>();
						betData.Initialize(bet, colorPattern[colorIndex++]);
						if (colorIndex == colorPattern.Length) colorIndex = 0;
					}
				}
			});
			if(repeat)
				yield return new WaitForSeconds(RepeatInterval);
			else
				yield return null;
		}
    }
}
