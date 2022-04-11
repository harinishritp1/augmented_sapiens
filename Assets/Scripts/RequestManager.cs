using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class RequestManager : MonoBehaviour
{
    public static RequestManager Instance;

    private float latitude;
    private float longitude;
    private string description = "";

    private void Awake() 
    {
        if (Instance == null)
            Instance = this;


        GetGPSCoords();
        GetIssueDescription();
        GetAnchorColor();
    }

    private void GetGPSCoords()
    {
        latitude = PlayerPrefs.GetFloat("Lat");
        longitude = PlayerPrefs.GetFloat("Long");
    }

    private void GetIssueDescription()
    {
        description = PlayerPrefs.GetString("description");
    }

    private void GetAnchorColor()
    {

    }

    public void RaiseRequest()
    {
        StartCoroutine(UploadToCloud());
    }



    /******************* Raise a service request *******************/

    public IEnumerator CheckInternetConnection(Action<bool> action)
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Error. Check internet connection!");
            action(false);
        }
        else
        {
            Debug.Log("Internet works");
            action(true);
        }
        yield return 0;
    }

    private IEnumerator UploadToCloud()
    {
        string url = "https://augmentedsapiens-staging.herokuapp.com/createticket";
        // string url = "http://172.20.10.2:5000";

        bool connection = false;
        yield return StartCoroutine(CheckInternetConnection((isConnected) => {
            // handle connection status here
            connection = isConnected;
        }));

        if (!connection)
        {
            Debug.Log("No internet Connection");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("image", ScreenshotManager.Instance.base64Tex);
        // form.AddField("image", "rgb");
        form.AddField("latitude", "123");
        form.AddField("longitude", "123");
        form.AddField("color", "Code Red");
        form.AddField("description", "Hey y'all!");

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                string response = www.downloadHandler.text;
                print(response);    
            }
        }
    }
}
