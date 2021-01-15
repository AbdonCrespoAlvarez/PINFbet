using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.UI;

/*
Subobjectives: SO_4, SO_9.
*/

public class InputSubjectsMenu : SimpleMenu<InputSubjectsMenu>
{
	public List<string> subjectSelected;

	[SerializeField]
	private GameObject subjListPrefab, subjPrefab, scrollViewport;
	[SerializeField]
	private ScrollRect scrollRect;
	[SerializeField]
	private TMP_Dropdown coursesDrop;
	[SerializeField]
	private Color[] colorPattern;
	private int colorIndex;

	private List<GameObject> subjectsParentList;
	Dictionary<string, List<string>> subjects;

	/* 
    When the menu is opened, this method gets the subjects which the student hasn't registered already and displays them
	on the screen, then the user can check them to introduce the data into the server system.
    */
	public void Start()
	{
		subjectsParentList = new List<GameObject>();
		RequestHandler.Get("getAvailableSubjects", (result) => {
			if (result.isNetworkError)
			{
				Debug.Log("Error Updating Bets: " + result.error);
			}
			else
			{
				List<string> courses = new List<string>();
				subjects = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(result.downloadHandler.text);
				foreach(KeyValuePair<string, List<string>> pair in subjects)
				{
					courses.Add(pair.Key);
					GameObject newList = Instantiate(subjListPrefab, scrollViewport.transform);
					subjectsParentList.Add(newList);
					foreach (string subject in pair.Value)
					{
						SubjectComp subjComp = Instantiate(subjPrefab, newList.transform).GetComponent<SubjectComp>();
						subjComp.Initialize(subject, colorPattern[colorIndex++], this);
						if (colorIndex == colorPattern.Length) colorIndex = 0;
					}
				}
				coursesDrop.AddOptions(courses);
				OnCourseChanged(0);
			}
		});
	}
	/* 
    Each time the course dropwdown changes it's value, the subjects options are set accordingly to the course selected.
    */
	public void OnCourseChanged(int course)
	{
		foreach(GameObject parent in subjectsParentList)
		{
			parent.SetActive(false);
		}
		subjectsParentList[course].SetActive(true);
		scrollRect.content = subjectsParentList[course].GetComponent<RectTransform>();
	}
	/* 
    When the user presses the confirm button, a request is sent with information about the selected subjects.
    */
	public void OnConfirmPressed()
	{
		RequestHandler.Post("addSubjects", new SubjectListData(subjectSelected), (result) => {
			Close();
			InputMarksMenu.Open();
		});
	}
}

