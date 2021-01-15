using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;
using System.Text;

/*
Subobjectives: SO_4, SO_9.
*/

public class RegisterMenu : SimpleMenu<RegisterMenu>
{
    [SerializeField]
    private TMP_InputField username, fullname, passInput, passInputRepat;
    [SerializeField]
    private TMP_Text errorDisplay;

    /* 
    Confirms that the user data is correct at a local level and informs of posible errors,
    then sends a createAccount request for the Server to execute.
    */
    public void OnRegisterConfirmed()
    {
        if (username.text == "")
        {
            errorDisplay.text = "Por favor, introduzca un nombre de usuario";
        }
        else if (passInput.text.Length < 8)
        {
            errorDisplay.text = "Por favor, introduzca una contraseña de más de 8 caracteres";
        }
        else if (passInput.text != passInputRepat.text)
        {
            errorDisplay.text = "Las contraseñas no coinciden, asegurese de que la ha escrito correctamente";
        }
        TryRegister(username.text, passInput.text);
    }

    /* 
    If the user clicks the login button, open the Login Menu.
    */
    public void OnLoginPressed()
    {
        LoginMenu.Open();
    }

    /* 
    If the user clicks the exit button, close the application.
    */
    public void OnExitPressed()
    {
        Application.Quit();
    }

    /* 
    Given a name and password, encrypts the password with a randomized salt to create the User object,
    then sends a create account request and handles errors which may have ocurred. It also assigns the authorization token
    received from the server and displays the profile or subjectInput Menu based on the servers response.
    */
    private void TryRegister(string name, string pass)
    {
        string salt = Random.Range(0, 9999).ToString();
        string cryptPass = Encrypter.generateHash(pass, salt);
        UserData userToSend = new UserData(name, fullname.text, cryptPass, salt);
        RequestHandler.Post("createAccount", userToSend, (result) => {
            if (result.isNetworkError)
            {
                Debug.Log("FailNet");
                errorDisplay.text = "Error de conexion";
            }
            else if (result.isHttpError)
            {
                Debug.Log("FailHttp: " + result.error);
                errorDisplay.text = "No se ha podido validar la cuenta, revise si ha introducido sus datos correctamente";
            }
            else
            {
                ContractMenu.Open();
                RequestHandler.sessionToken = result.downloadHandler.text;
                RequestHandler.sessionUsername = name;
            }
        });
    }
}