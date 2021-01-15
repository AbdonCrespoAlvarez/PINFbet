using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;
using System.Text;

/*
Subobjectives: SO_4, SO_9.
*/

public class LoginMenu : SimpleMenu<LoginMenu>
{
    [SerializeField]
	private TMP_InputField username, passwordInput;
    [SerializeField]
    private TMP_Text errorDisplay;
    /* 
    Confirms that the user data is correct at a local level and informs of posible errors,
    then sends a login request for the Server to execute.
    */
    public void OnLoginConfirmed()
	{
        if(username.text == "")
		{
            errorDisplay.text = "Por favor, introduzca un nombre de usuario";
		}else if (passwordInput.text.Length < 8)
		{
            errorDisplay.text = "Por favor, introduzca una contraseña de más de 8 caracteres";
        }
        TryLogin(username.text, passwordInput.text);
	}
    /* 
    Displays the Register Menu on screen.
    */
    public void OnRegisterPressed()
	{
        RegisterMenu.Open();
	}
    /* 
    Closes the application completely.
    */
    public void OnExitPressed()
	{
		Application.Quit();
	}
    /* 
    Given a name and password, encrypts the password with a randomized salt to create the User object,
    then sends a request to login and handles errors which may have ocurred. It also assigns the authorization token
    received from the server and displays the profile or subjectInput Menu based on the servers response.
    */
    private void TryLogin(string name, string inputPass)
    {
        Debug.Log("Login with: " + name + ":" + inputPass);
        RequestHandler.Get("checkUser/" + name, (checkResult) => {
            if (checkResult.isNetworkError)
            {
                Debug.Log("FailNet");
                errorDisplay.text = "Error de conexion";
            }
            else if (checkResult.isHttpError)
			{
                Debug.Log("FailHttp");
                errorDisplay.text = "No se ha podido validar la cuenta, revise si ha introducido sus datos correctamente";
            }
            else
            {
                RequestHandler.sessionToken = Encrypter.generateHash(inputPass, checkResult.downloadHandler.text);
                Debug.Log("loginAccount / " + name);
                RequestHandler.Get("loginAccount/" + name, (loginResult) =>
                {
                    Debug.Log("LOGGED");
                    if (loginResult.isHttpError)
                    {
                        Debug.Log(loginResult.error);
                        errorDisplay.text = "No se ha podido validar la cuenta, revise si ha introducido sus datos correctamente";
                    }
                    else if (loginResult.downloadHandler.text == "")
					{
                        errorDisplay.text = "No se ha podido validar la cuenta, revise si ha introducido sus datos correctamente";

                    }
                    else
                    {
                        RequestHandler.sessionToken = loginResult.downloadHandler.text;
                        RequestHandler.sessionUsername = name;
                        RequestHandler.Get("isUpdated", (result) =>{
                            if(int.Parse(result.downloadHandler.text) == 0) {
                                InputSubjectsMenu.Open();
                            }
							else {
                                ProfileMenu.Open();
                            }
                        });
                    }
                });
            }
        });
    }
}