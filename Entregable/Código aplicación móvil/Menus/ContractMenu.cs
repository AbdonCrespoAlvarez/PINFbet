using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI.Extensions;
using UnityEngine;
using TMPro;

/*
Subobjectives: SO_3.
*/

public class ContractMenu : SimpleMenu<ContractMenu>
{
    [SerializeField]
    private TextMeshProUGUI contractText;
    /* 
    When the user accepts the use contract, display the InputSubjectsMenu
    */
    public void OnAccepted()
	{
        InputSubjectsMenu.Open();
	}
}
