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
    [HideInInspector]public string base64Tex;

    public Image ScreenshotImage;
    private Texture2D tex;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        base64Tex = PlayerPrefs.GetString ("Screenshot");

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
}
