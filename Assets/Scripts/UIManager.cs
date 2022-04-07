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

    private bool isNotePanelUp = false;

    private void Awake() 
    {
        if (Instance == null)
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

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
        LeanTween.moveY(notePanel.GetComponent<RectTransform>(), 0f, 1f).setEase( LeanTweenType.easeOutQuad );
        isNotePanelUp = true;
    }

    public void CloseNotePanel()
    {
        Destroy(GameManager.Instance.markers[GameManager.Instance.markers.Count-1]);
        LeanTween.moveY(notePanel.GetComponent<RectTransform>(), -1526f, 1f).setEase( LeanTweenType.easeOutQuad );
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
        switch(status)
        {
            case 1: 

                break;

            default:
                break;
        }
    }

    private void ChangeStatus(int status)
    {
        // foreach(GameObject button in )
        
    }
}
