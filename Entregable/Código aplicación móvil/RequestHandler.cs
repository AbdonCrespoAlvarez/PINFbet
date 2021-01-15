using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System.IO;

/*
Subobjectives: SO_9.
*/

public class RequestHandler : MonoBehaviour
{
    public static string sessionUsername = "";
    public static string sessionCryptedPass = "";
    public static string salt = "";
    public static string sessionToken = "";

    private static string APIAddress = "http://localhost:23415/";
    private static RequestHandler instance;

    /*
    Initialize an instance of the class to make the HTTP requests
    */
    void Start()
    {
        instance = this;
        string path = Path.Combine(Application.dataPath, "config.json");
        if (File.Exists(path))
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string jsonString = reader.ReadToEnd();
                    Config conf = JsonUtility.FromJson<Config>(jsonString);
                    APIAddress = "http://" + conf.host + ':' + conf.port + '/';
                }
            }
        }
	}

    /*
    Static method to provide accesibility for the instance's coroutine execution of the GET HTTP Requests.
    It also creates the uri based on the Host and endPoint desired.
    */
    public static void Get(string endPoint, System.Action<UnityWebRequest> callback)
    {
        instance.StartCoroutine(GetRequest(APIAddress + endPoint, (response) => {
            callback(response);
        }));
    }
    /*
    Static method to provide accesibility for the instance's coroutine execution of the POSTS HTTP Requests
    the data sent in the request body is going to be a RestData object converted to JSON. It also creates the uri
    based on the Host and endPoint desired.
    */
    public static void Post(string endPoint, RestData restRequest, System.Action<UnityWebRequest> callback)
    {
        string jsonRequest = JsonUtility.ToJson(restRequest);
        instance.StartCoroutine(PostRequest(APIAddress + endPoint, jsonRequest, (response)=> {
            callback(response);
        }));
    }
    /*
    Static method to provide accesibility for the instance's coroutine execution of the POSTS HTTP Requests
    the data sent in the request is the json string given as a parameter.It also creates the uri
    based on the Host and endPoint desired.
   */
    public static void Post(string endPoint, string jsonRequest, System.Action<UnityWebRequest> callback)
    {
        instance.StartCoroutine(PostRequest(APIAddress + endPoint, jsonRequest, (response) => {
            callback(response);
        }));
    }
    /*
    This method provides the authorization token which also authenticates the user on the REST API and sends
    a GET HTTP Request which the given uri.
   */
    private static IEnumerator GetRequest(string uri, System.Action<UnityWebRequest> callback)
    {
        // Setup the request and it's authorization token
        UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        webRequest.SetRequestHeader("authorization", sessionToken);
        // Send the GET request
        yield return webRequest.SendWebRequest();
        //Call the function given as a parameter once the request has been answered
        callback(webRequest);
    }
    /*
    This method provides the authorization token which also authenticates the user on the REST API and sends
    a POST HTTP Request which the given uri and the JSON string to sent as the request's body.
   */
    private static IEnumerator PostRequest(string uri, string jsonData, System.Action<UnityWebRequest> callback)
    {
        // Setup the request and it's authorization token
        UnityWebRequest webRequest = UnityWebRequest.Post(uri, jsonData);
        webRequest.SetRequestHeader("authorization", sessionToken);

        // Encode the body of the request properly first
        byte[] cargaCodificada = new System.Text.UTF8Encoding().GetBytes(jsonData);
        webRequest.uploadHandler = new UploadHandlerRaw(cargaCodificada);
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        // Set the request as REST with the appropiated content-type and then send it
        webRequest.SetRequestHeader("Content-Type", "application/json");
        yield return webRequest.SendWebRequest();
        //Call the function given as a parameter once the request has been answered
        callback(webRequest);
    }
}
