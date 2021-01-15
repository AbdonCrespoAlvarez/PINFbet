using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;
using Newtonsoft.Json;

/*
Subobjectives: SO_4, SO_9.
*/

public class InputMarksMenu : SimpleMenu<InputMarksMenu>
{
	public List<MarkComp> marksObtained;

	[SerializeField]
	private Transform marksList;
	[SerializeField]
	private GameObject markPrefab;
	[SerializeField]
	private Color[] colorPattern;
	private int colorIndex;


	List<string> subjects;
	/* 
    Retrieve the user's data about it's subjects in order to let the user set the corresponding marks.
    */
	public void Start()
	{
		RequestHandler.Get("getSubjectsToMark", (result) => {
			if (result.isNetworkError)
			{
				Debug.Log("Error retreiving your subjects: " + result.error);
			}
			else
			{
				subjects = JsonConvert.DeserializeObject<List<string>>(result.downloadHandler.text);
				foreach(string subject in subjects)
				{
					MarkComp markComp = Instantiate(markPrefab, marksList).GetComponent<MarkComp>();
					markComp.Initialize(subject, colorPattern[colorIndex++]);
					if (colorIndex == colorPattern.Length) colorIndex = 0;

					marksObtained.Add(markComp);
				}
			}
		});
	}
	/* 
    When the user presses the confirm button, a request is sent with the marks obtained in each of the user's subjects.
	Marks not provided will be treated as waiting to be qualified.
    */
	public void OnConfirmPressed()
	{
		List<KeyValuePair<string, float>> marks = new List<KeyValuePair<string, float>>(12);
		foreach(MarkComp mark in marksObtained)
		{
			if(float.TryParse(mark.mark.text, out float markNum)) 
				marks.Add(new KeyValuePair<string, float>(mark.subjName.text, markNum));
		}
		string jsonMarks = JsonConvert.SerializeObject(marks);
		RequestHandler.Post("addMarks", jsonMarks, (result) =>{
			Close();
			ProfileMenu.Open();
		});
	}
}