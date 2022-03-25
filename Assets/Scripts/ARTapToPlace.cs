using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlace : MonoBehaviour
{
    [SerializeField]
    private GameObject markerPrefab;
    private GameObject spawnedObject;
    private ARRaycastManager arRaycastManager;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake() 
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        PlaceObjectOnTap();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    private void PlaceObjectOnTap()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;
        
        if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.All))
        {
            var hitPose = hits[0].pose;

            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(markerPrefab, hitPose.position, hitPose.rotation);
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
            }
        }
        
    }
}
