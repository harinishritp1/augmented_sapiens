using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using LeTai.Asset.TranslucentImage;
using TMPro;

[RequireComponent(typeof(ARRaycastManager))]
public class ARManager : MonoBehaviour
{
    public static ARManager Instance;
    public GameObject spawnedObject;

    [SerializeField]
    private GameObject markerPrefab;
    private ARRaycastManager arRaycastManager;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake() 
    {
        if (Instance == null)
            Instance = this;
            
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        PlaceObjectOnTap();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                touchPosition = Input.GetTouch(0).position;
                return true;    
            }
        }

        touchPosition = default;
        return false;
    }

    private void PlaceObjectOnTap()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;
        
        if (!UIManager.Instance.isNotePanelUp && spawnedObject==null)
        {
            Debug.Log("Instantiating marker");
            spawnedObject = Instantiate(markerPrefab, Camera.main.transform.position + new Vector3(0.0f, 0.0f, 0.2f), Quaternion.identity);
            GameManager.Instance.markers.Add(spawnedObject);
            spawnedObject.transform.GetChild(0).GetChild(0).GetComponent<TranslucentImage>().source = Camera.main.GetComponent<TranslucentImageSource>();
            
            UIManager.Instance.markerText = spawnedObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            UIManager.Instance.BringUpNotePanel();
        }
        

        // if (spawnedObject.transform.GetChild(0).GetChild(0).GetComponent<TranslucentImage>().source == null)
        // {
        //     spawnedObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Null";
        // }
    }
}