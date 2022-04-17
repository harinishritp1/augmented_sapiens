using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public AdminManager adminManager;

    public GameObject arPanel;
    public TextMeshProUGUI markerText;
    public GameObject notePanel;
    public GameObject adminPanel;
    public GameObject passwordPanel;
    public GameObject cardPanel;
    public GameObject[] cardsPanel;
    public GameObject handleRequestPanel;
    public TMP_InputField noteDescription;
    public TMP_InputField password;
    public GameObject[] statusButtons = new GameObject[4];
    public Sprite[] selectedStatusImages = new Sprite[4];
    public Sprite[] deselectedStatusImages = new Sprite[4];
    public bool isNotePanelUp = false;
    public string anchorColor = "";

    private int status = 0;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // BringUpNotePanel(); // For Debugging
    }

    // Update is called once per frame
    void Update()
    {
        FillMarkerNote();
    }

    public void CaptureButton()
    {
        TakeScreenshot.Instance.StartCaptureProcess();
    }

    public void HideUI()
    {
        arPanel.SetActive(false);
    }

    public void BringUpNotePanel()
    {
        StatusButtons(1);
        noteDescription.text = "";
        LeanTween.moveY(notePanel.GetComponent<RectTransform>(), 0f, 0.5f).setEase( LeanTweenType.easeOutQuad );
        isNotePanelUp = true;
    }

    public void CloseNotePanel()
    {
        LeanTween.moveY(notePanel.GetComponent<RectTransform>(), -1526f, 0.5f).setEase( LeanTweenType.easeOutQuad );
        StartCoroutine(SetBoolFalse());
    }

    private IEnumerator SetBoolFalse()
    {
        yield return new WaitForSeconds(0.1f);
        isNotePanelUp = false;
    }

    public void DeleteAnchor()
    {
        Destroy(GameManager.Instance.markers[GameManager.Instance.markers.Count-1]);
        CloseNotePanel();
    }

    public void SaveAnchor()
    {
        CloseNotePanel();
    }

    private void FillMarkerNote()
    {
        if(isNotePanelUp && markerText != null)
        {
            markerText.text = noteDescription.text;
        }
    }

    public void StatusButtons(int status)
    {
        RevertSelectedStatus();

        switch(status)
        {
            case 1: 
                statusButtons[0].GetComponent<Image>().sprite = selectedStatusImages[0];
                ARManager.Instance.spawnedObject.GetComponent<SpriteRenderer>().sprite = deselectedStatusImages[0];
                status = 1; anchorColor = "Red";
                break;

            case 2:
                statusButtons[1].GetComponent<Image>().sprite = selectedStatusImages[1];
                ARManager.Instance.spawnedObject.GetComponent<SpriteRenderer>().sprite = deselectedStatusImages[1];
                status = 2; anchorColor = "Yellow";
                break;

            case 3:
                statusButtons[2].GetComponent<Image>().sprite = selectedStatusImages[2];
                ARManager.Instance.spawnedObject.GetComponent<SpriteRenderer>().sprite = deselectedStatusImages[2];
                status = 3; anchorColor = "Blue";
                break;
            
            case 4: 
                statusButtons[3].GetComponent<Image>().sprite = selectedStatusImages[3];
                ARManager.Instance.spawnedObject.GetComponent<SpriteRenderer>().sprite = deselectedStatusImages[3];
                status = 4; anchorColor = "Green";
                break;

            default:
                break;
        }
    }

    private void RevertSelectedStatus()
    {
        for(int i = 0; i < 4; i++)
        {
            statusButtons[i].GetComponent<Image>().sprite = deselectedStatusImages[i];
        }        
    }

    public void AdminButton()
    {
        LeanTween.moveY( adminPanel.GetComponent<RectTransform>(), 0f, 0.5f).setEase( LeanTweenType.easeOutQuad );
    }

    public void SignInButton()
    {
        if (password.text == "123")
        {
            Debug.Log("Login successful");
            LeanTween.moveY( passwordPanel.GetComponent<RectTransform>(), 1474f, 0.5f).setEase( LeanTweenType.easeOutQuad );
            cardPanel.SetActive(true);
        }
    }

    public void ActivateCards()
    {
        foreach(Transform Child in cardPanel.transform)
        {
            Child.gameObject.SetActive(false);
        }

        for(int i=0; i<cardsPanel.Length; i++)
        {
            cardsPanel[i] = cardPanel.transform.GetChild(i).gameObject;
            cardsPanel[i].SetActive(true);
        }
    }

    public void PopulateRequests(int cardNumber)
    {
        cardsPanel[cardNumber].GetComponent<Image>().sprite = Sprite.Create(CreateImage(adminManager.request.image), new Rect(0, 0, Util.ScreenWidth, Util.ScreenHeight), new Vector2(.5f, .5f), 100, 0, SpriteMeshType.FullRect);        
    }

    public Texture2D CreateImage(string base64Tex)
    {
        byte[] imageBytes = System.Convert.FromBase64String (base64Tex);
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.anisoLevel = 10;
        tex.filterMode = FilterMode.Trilinear;
        tex.LoadImage(imageBytes);
        return tex;
    }

    public void RequestButton(int cardNumber)
    {
        handleRequestPanel.SetActive(true);
        handleRequestPanel.GetComponent<Image>().sprite = Sprite.Create(CreateImage(adminManager.requestList[cardNumber].image), new Rect(0, 0, Util.ScreenWidth, Util.ScreenHeight), new Vector2(.5f, .5f), 100, 0, SpriteMeshType.FullRect); 
    }

    public void RequestBackButton()
    {
        handleRequestPanel.SetActive(false);
    }
}
