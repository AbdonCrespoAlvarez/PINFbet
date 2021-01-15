using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
Subobjectives: SO_5, SO_9.
*/

public class BetComp : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI student, subject, coins, mark;
    [SerializeField]
    Image sprite;
    /* 
    Initializes the friend component with the given parameters.
    */
    public void Initialize(BetData betData, Color col)
    {
        student.text = betData.studentname;
        subject.text = betData.subjectname;
        coins.text = betData.coins.ToString();
        mark.text = betData.markExpected.ToString();
        sprite.color = col;
    }
}
