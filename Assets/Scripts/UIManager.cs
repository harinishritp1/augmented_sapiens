using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using DentedPixel.LeanTween;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject notePanel;

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
        
    }

    public void CaptureButton()
    {
        TakeScreenshot.Instance.StartCaptureProcess();
    }

    public void HideUI()
    {
        
    }

    public void BringUpNotePanel()
    {
        LeanTween.moveY(notePanel.GetComponent<RectTransform>(), 0f, 1f).setEase( LeanTweenType.easeOutQuad );
    }
}
