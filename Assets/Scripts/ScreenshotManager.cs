using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;



public class ScreenshotManager : MonoBehaviour
{
    public static ScreenshotManager Instance;
    [HideInInspector]public byte[] imageBytes;

    public Image ScreenshotImage;

    private Texture2D tex;
    private string api = "";

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        string base64Tex = PlayerPrefs.GetString ("Screenshot");

        if (!string.IsNullOrEmpty (base64Tex)) 
        {
            // convert it to byte array
            imageBytes = System.Convert.FromBase64String (base64Tex);
            int width = Screen.width;
            int height = Screen.height;
            tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.anisoLevel = 10;
            tex.filterMode = FilterMode.Trilinear;
            tex.LoadImage(imageBytes);
            ScreenshotImage.sprite = Sprite.Create(tex, new Rect(0, 0, Util.ScreenWidth, Util.ScreenHeight), new Vector2(.5f, .5f), 100, 0, SpriteMeshType.FullRect);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
        form.AddBinaryData("attachment", imageBytes);
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
