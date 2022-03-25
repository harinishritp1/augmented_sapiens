using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TakeScreenshot : MonoBehaviour
{
    public static TakeScreenshot Instance;

    private bool imageSaved = false;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartCaptureProcess()
    {
        UIManager.Instance.HideUI();
        StartCoroutine(Capture());
        StartCoroutine(ChangeScene());
    }

    private IEnumerator Capture()
    {        
        yield return new WaitForEndOfFrame();

        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Trilinear;
        tex.anisoLevel = 10;

        // Read screen contents into the texture        
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] imageBytes = tex.EncodeToPNG();
        Object.Destroy(tex);
        string base64Tex = System.Convert.ToBase64String (imageBytes);
        Debug.Log(base64Tex);

        // write string to playerpref
        PlayerPrefs.SetString ("Screenshot", base64Tex);
        PlayerPrefs.Save();
        imageSaved = true;
    }

    IEnumerator ChangeScene()
    {
        // yield return new WaitUntil(isNoteDataReceieved);
        yield return new WaitUntil(isImageSaved);
        Debug.Log("Changing Scene");
        SceneManager.LoadScene(1);
    }

    bool isImageSaved()
    {
        if(imageSaved)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
