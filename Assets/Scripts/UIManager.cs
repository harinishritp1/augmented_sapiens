using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject arPanel;
    public TextMeshProUGUI markerText;
    public GameObject notePanel;
    public TMP_InputField noteDescription;
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
}
