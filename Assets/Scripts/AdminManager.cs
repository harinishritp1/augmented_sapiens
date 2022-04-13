using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class AdminManager : MonoBehaviour
{
    private void Start() 
    {
        StartCoroutine(GetActiveRequests());
    }

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

    private IEnumerator GetActiveRequests()
    {
        string url = "https://augmentedsapiens-staging.herokuapp.com/getactiveticket";

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

        using (UnityWebRequest www = UnityWebRequest.Get(url))
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
