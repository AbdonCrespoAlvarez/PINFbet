using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MenuInicio : MonoBehaviour
{

    public void EnviarDatos()
    {

        StartCoroutine(PeticionPOST("http://127.0.0.1:8080/habitacion/alta"));

    }

    private IEnumerator PeticionPOST(string uri)
    {

        Apuesta apuesta = new Apuesta();
        apuesta.PINFcoinsApostadas = 1115;
        apuesta.notaFinal = 7;
        apuesta.creador = "Pepe tijuela manolito lucas";

        string datos = JsonUtility.ToJson(apuesta); // Pasarlo todo a cadena JSON
        UnityWebRequest peticion = UnityWebRequest.Post(uri, datos); // Hacer a la determinada dirección la determinada operación

        // Preparar la petición
        byte[] cargaCodificada = new System.Text.UTF8Encoding().GetBytes(datos); // Pasarlo todo a bytes
        peticion.uploadHandler = new UploadHandlerRaw(cargaCodificada);
        peticion.downloadHandler = new DownloadHandlerBuffer();

        peticion.SetRequestHeader("Content-Type", "application/json"); // Decirle al servidor que lo que se le envía es un JSON
       
        yield return peticion.SendWebRequest(); // Esperar a que el servidor termine de hacer la operación
        
        if (peticion.isNetworkError)
            Debug.Log("Error: " + peticion.error);
        else
            Debug.Log("Recibido: " + peticion.downloadHandler.text);

    }

    public void RecibirDatos()
    {

        StartCoroutine(PeticionGET("http://127.0.0.1:8080/habitacion/listado"));

    }

    private IEnumerator PeticionGET(string uri)
    {

        UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError)
            Debug.Log("Error: " + webRequest.error);
        else
            Debug.Log("Recibido: " + webRequest.downloadHandler.text);

    }

    public void Salir()
    {

        Debug.Log("Salir");

        Application.Quit();

    }

}
