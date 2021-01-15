using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
Subobjectives: SO_4, SO_5, SO_9.
*/

public class SubjectComp : MonoBehaviour
{
    private InputSubjectsMenu inputMenu;
    [SerializeField]
    private TextMeshProUGUI subjName;
    [SerializeField]
    private Image sprite;
    /* 
    Initializes the subject component with the given parameters, it also assigns the Menu which holds the list of selcted
    subjects.
    */
    public void Initialize(string name, Color col, InputSubjectsMenu myMenu)
    {
        subjName.text = name;
        sprite.color = col;
        inputMenu = myMenu;
    }
    /* 
    Each time the box is checked or unchecked, add or remove the subjects name from the list of selected subjects
    */
    public void Toggled(bool isOn) {
        if (isOn)
		{
            inputMenu.subjectSelected.Add(subjName.text);
		}
		else
		{
            inputMenu.subjectSelected.Remove(subjName.text);
        }
    }
}