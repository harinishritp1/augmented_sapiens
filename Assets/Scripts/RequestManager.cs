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
    public GameObject notificationPanel;
    public GameObject loadingPanel;
    public TextMeshProUGUI notificationhHeading;

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
        gpsText.text = "Location: " + latitude.ToString() + ", " + longitude.ToString();
    }

    private void Start() 
    {
        // RaiseServiceRequest(); // For Debugging
    }

    public void RaiseServiceRequest()
    {
        StartCoroutine(UploadToCloud());
        loadingPanel.SetActive(true);
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

        bool connection = false;
        yield return StartCoroutine(CheckInternetConnection((isConnected) => {
            // handle connection status here
            connection = isConnected;
        }));

        if (!connection)
        {
            Debug.Log("No internet Connection");
            ActivateNotification("No internet Connection");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("image", ScreenshotManager.Instance.base64Tex);
        form.AddField("latitude", latitude.ToString());
        form.AddField("longitude",longitude.ToString());
        form.AddField("color", color);
        form.AddField("description", description);

        /************ For Debugging *************/
        // form.AddField("image", "rgb");   
        // form.AddField("latitude", "123");
        // form.AddField("longitude", "123");
        // form.AddField("color", "blue");
        // form.AddField("description", "description");

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            loadingPanel.SetActive(false);

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                // ActivateNotification("Error submitting request. Please try again after some time.");
                ActivateNotification(www.error);
            }
            else
            {
                string response = www.downloadHandler.text;
                print(response);
                if(response != null)
                {
                    ActivateNotification("Ticket created successfully!");
                }
            }
        }
    }

    private void ActivateNotification(string notification)
    {
        notificationPanel.SetActive(true);
        notificationhHeading.text = notification;
    }

    public void OkayButton()
    {
        // LeanTween.scale(notificationPanel.transform.GetChild(0).gameObject.GetComponent<RectTransform>(), notificationPanel.transform.GetChild(0).gameObject.GetComponent<RectTransform>().localScale/10f, 0.5f).setEase( LeanTweenType.easeOutQuad );
        // LeanTween.alpha(notificationPanel.GetComponent<RectTransform>(), 0.0f, 0.3f).setEase( LeanTweenType.easeOutQuad );
        // LeanTween.alpha(notificationPanel.transform.GetChild(0).gameObject.GetComponent<RectTransform>(), 0.0f, 0.3f).setEase( LeanTweenType.easeOutQuad );
        StartCoroutine(DeactivateNotification());
    }

    private IEnumerator DeactivateNotification()
    {
        yield return new WaitForSeconds(0.0f);
        notificationPanel.SetActive(false);
    }
}
