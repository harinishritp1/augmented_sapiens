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
        string url = "";

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

        // List<IMultipartFormSection> form = new List<IMultipartFormSection>();

        // form.Add(new MultipartFormDataSection("name", fileName));
        // if(imageBytes != null)
        //     form.Add(new MultipartFormFileSection("attachment", imageBytes, fileName, "image"));

        WWWForm form = new WWWForm();
        form.AddBinaryData("attachment", ScreenshotManager.Instance.imageBytes);
        // form.AddField("field", "null");
        // form.AddField("name", foldername);
        // form.AddField("parent_id", folderID);
        
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        // www.SetRequestHeader("x-auth-token", token);

        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
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
