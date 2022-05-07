using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARRayCasting : MonoBehaviour
{

    public GameObject initialGameObject ;

    private GameObject spawnedObject ;
    private ARRaycastManager rayCastManager;
    
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();


	private void Awake()
	{
        rayCastManager = GetComponent<ARRaycastManager>();
	}

    bool TryGetTouchInput(out Vector2 clickPosition) {
        
        if (Input.touchCount > 0) 
        {
            clickPosition = Input.GetTouch(0).position;
            return true; 
        }
        clickPosition = default;
        return false; 
    }

    public void shootArrow(Vector2 clickPosition) {


        if (rayCastManager.Raycast(clickPosition, hits, TrackableType.PlaneWithinPolygon)) {

            var hitPose = hits[0].pose;
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(initialGameObject, hitPose.position, hitPose.rotation);
            }
            else {
                spawnedObject.transform.position = hitPose.position; 
            }
        }
    }
/*
	private void Update()
	{
		if (!TryGetTouchInput(out Vector2 clickPosition))
		{
            return;
		}


        if (rayCastManager.Raycast(clickPosition, hits, TrackableType.PlaneWithinPolygon))
        {
    
            var hitPose = hits[0].pose;
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(initialGameObject, hitPose.position, hitPose.rotation);
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
            }
        }
    }
**/
}
