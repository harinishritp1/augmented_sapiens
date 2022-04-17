using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class AdminManager : MonoBehaviour
{
    public static AdminManager Instance;

    public int numOfRequests = 0;
    
    public struct Requests 
    {
        public string color;
        public string description;
        public string image;
        public float latitude;
        public float longitude;
        public int priority;
        public string status;
        public int tickedID;
    };  
    public Requests request;
    public List<Requests> requestList = new List<Requests>();

    private string response = "";
    [SerializeField]
    private TextAsset testJSON;

    private void Awake() 
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start() 
    {
        StartCoroutine(GetActiveRequests());
        // LoadTestFile();
    }

    private void LoadTestFile()
    {
        if (testJSON != null)
            JSONParser(testJSON.ToString());
        else
            Debug.Log("null");
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
                response = www.downloadHandler.text;
                print(response);    
                JSONParser(response);
            }
        }
    }

    private void JSONParser(string response)
    {
        JSONObject j = new JSONObject(JSONObject.Type.STRING);
        j = JSONObject.Create(response);

        JSONObject countObject = j.GetField("count");
        numOfRequests = Int32.Parse(countObject.ToString());
        UIManager.Instance.cardsPanel = new GameObject[numOfRequests];
        UIManager.Instance.ActivateCards();

        JSONObject data = j.GetField("data");

        if (numOfRequests!=0)
        {
            for (int i=0; i<numOfRequests; i++)
            {
                // Debug.Log(data[i].GetField("Color").ToString().Trim('"'));
                request = new Requests();

                request.color = data[i].GetField("Color").ToString().Trim('"');
                request.description = data[i].GetField("Description").ToString().Trim('"');
                request.image = data[i].GetField("Image").ToString().Trim('"');
                request.latitude = float.Parse(data[i].GetField("Latitude").ToString());
                request.longitude = float.Parse(data[i].GetField("Longitude").ToString());
                request.priority = Int32.Parse(data[i].GetField("Priority").ToString());
                request.status = data[i].GetField("Status").ToString();
                request.tickedID = Int32.Parse(data[i].GetField("Ticket Id").ToString());

                UIManager.Instance.PopulateRequests(i);
                requestList.Add(request);
            }
        }
    }
}
