using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageDetectionMode : MonoBehaviour
{
    [SerializeField] private List<GameObject> _objectsToPlace;
    [SerializeField] private ARTrackedImageManager _aRTrackedImageManager;

    //public ARVideoPlayerObject CurrentVideoPlayerObject = null;

    private int _refImageCount;
    private Dictionary<string, GameObject> _allObjects;
    private IReferenceImageLibrary _refLibrary;
    private GameObject currentObject = null;
    private void RotateSelectedObject(UnityEngine.Touch touch1)
    {

        if (touch1.phase != TouchPhase.Moved)
            return;

        currentObject.transform.position = InteractionManager.Instance.GetARRaycastHits(touch1.position)[0].pose.position;
    }

    private void OnEnable()
    {
        _aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        _aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    private void Start()
    {
        _refLibrary = _aRTrackedImageManager.referenceLibrary;
        _refImageCount = _refLibrary.count;
        LoadObjectDictionary();
    }

    private void LoadObjectDictionary()
    {
        _allObjects = new Dictionary<string, GameObject>();
        for (int i = 0; i < _refImageCount; i++)
        {
            GameObject newOverlay = new GameObject();
            newOverlay = _objectsToPlace[i];
            if (_objectsToPlace[i].gameObject.scene.rootCount == 0)
                newOverlay = Instantiate(_objectsToPlace[i], transform.localPosition, Quaternion.identity);

            _allObjects.Add(_refLibrary[i].name, newOverlay);
            newOverlay.SetActive(false);
        }
    }

    private void ActivateTrackedObject(string imageName)
    {
        Debug.Log("[IMAGE_DETECTION]: Tracked the target: " + imageName);
        _allObjects[imageName].SetActive(true);
        _allObjects[imageName].transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
    }

    //private IEnumerator ScaleObject(GameObject obj, float targetScale, float duration)
    //{
    //    float elapsedTime = 0;
    //    Vector3 startingScale = obj.transform.localScale;
    //    Vector3 finalScale = new Vector3(targetScale, targetScale, targetScale);

    //    while (elapsedTime < duration)
    //    {
    //        obj.transform.localScale = Vector3.Lerp(startingScale, finalScale, (elapsedTime / duration));
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    obj.transform.localScale = finalScale;
    //}

    private void UpdateTrackedObject(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            currentObject = _allObjects[trackedImage.referenceImage.name];
            currentObject.SetActive(true);
            currentObject.transform.position = trackedImage.transform.position;
            currentObject.transform.rotation = trackedImage.transform.rotation;
        }
        else
        {
            _allObjects[trackedImage.referenceImage.name].SetActive(false);
            if (currentObject == _allObjects[trackedImage.referenceImage.name])
            {
                currentObject = null;
            }
        }
    }



    private void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var addedImage in args.added)
        {
            ActivateTrackedObject(addedImage.referenceImage.name);
        }

        foreach (var updated in args.updated)
        {
            UpdateTrackedObject(updated);
        }

        foreach (var trackedImage in args.removed)
        {
            Destroy(trackedImage.gameObject);
        }
    }

    public void Activate()
    {
        //CurrentVideoPlayerObject = null;
    }

    public void Deactivate()
    {

        //CurrentVideoPlayerObject = null;
        foreach (var key in _allObjects)
            key.Value.SetActive(false);
    }

    public void TouchInteraction(UnityEngine.Touch[] touches)
    {
        UnityEngine.Touch touch = touches[0];
        RotateSelectedObject(touch);
    }


}
