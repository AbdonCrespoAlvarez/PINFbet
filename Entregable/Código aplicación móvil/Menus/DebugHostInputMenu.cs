using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
Used to facilitate academic evaluation via Windows and Linux versions of PINFbet's
mobile application.
Module implements a config buttom in the sign up and sign in menus that allow the user
to specify the IP and port of the PINFbet server to connect to.
*/

public class DebugHostInputMenu : MonoBehaviour
{
	[SerializeField]
	TMP_InputField hostInput, portInput;
    public void OnAddressOk()
	{
		string host = hostInput.text;
		if (host == "") host = "localhost";
		string port = portInput.text;
		if (port == "") port = "23415";
		RequestHandler.APIAddress = "http://" + host + ':' + port + '/';
	}
}
