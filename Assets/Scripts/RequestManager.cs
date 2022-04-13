using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class RequestManager : MonoBehaviour
{
    public static RequestManager Instance;

    public TextMeshProUGUI gpsText;

    private float latitude;
    private float longitude;
    private string description = "";
    private string color = "";

    private void Awake() 
    {
        if (Instance == null)
            Instance = this;

        GetSavedData();
        UpdateGPStext();
    }

    private void GetSavedData()
    {
        latitude = PlayerPrefs.GetFloat("Lat");
        longitude = PlayerPrefs.GetFloat("Long");
        description = PlayerPrefs.GetString("description");
        color = PlayerPrefs.GetString("color");
    }

    private void UpdateGPStext()
    {
        gpsText.text = "Detected Coordinates: " + latitude.ToString() + ", " + longitude.ToString();
    }

    private void Start() 
    {
        // RaiseServiceRequest(); // For Debugging
    }

    public void RaiseServiceRequest()
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
        // form.AddField("image", "rgb");   // for Debugging
        form.AddField("latitude", latitude.ToString());
        form.AddField("longitude",longitude.ToString());
        form.AddField("color", color);
        form.AddField("description", description);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                string response = www.downloadHandler.text;
                print(response);    
            }
        }
    }
}
