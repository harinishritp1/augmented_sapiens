using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GPS : MonoBehaviour
{
    public static GPS Instance;

    public float latitude, longitude;  

    void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    private void Start() 
    {
        StartCoroutine(GetLocation());
    }

    IEnumerator GetLocation()
    {        
        // Check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location Services not enabled");
            yield break;
        } 

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;

            // PlayerPrefs.SetFloat("Lat", latitude);
            // PlayerPrefs.SetFloat("Long", longitude);
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }
}
