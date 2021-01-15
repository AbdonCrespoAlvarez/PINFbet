using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
Subobjectives: SO_4, SO_5, SO_9.
*/

public class MarkComp : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI subjName;
    [SerializeField]
    public TMP_InputField mark;
    [SerializeField]
    private Image sprite;
    /* 
    Initializes the subject mark component with the given parameters.
    */
    public void Initialize(string name, Color col)
    {
        subjName.text = name;
        sprite.color = col;
    }
}
