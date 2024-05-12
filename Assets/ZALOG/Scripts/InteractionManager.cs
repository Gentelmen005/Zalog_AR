using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARTrackedImageManager))]
public class InteractionManager : MonoBehaviour
{

    [SerializeField] private GameObject startWindow;
    [SerializeField] private GameObject descriptionWindow; 
    [SerializeField] private GameObject mainWindow;

    private Camera _arCamera;
    public Camera ARCamera {
        get { 
            return _arCamera; 
        }
    }

    private ARRaycastManager _aRRaycastManager;
    private ARAnchorManager _aRAnchorManager;
    private ARTrackedImageManager _arImageManager;
    private List<ARRaycastHit> _raycastHits;
    private ARCameraManager _arCameraManager;

    #region Singleton
    /// <summary>
    /// Instance of our Singleton
    /// </summary>
    public static InteractionManager Instance {
        get {
            return _instance;
        }
    }
    private static InteractionManager _instance;

    public void InitializeSingleton()
    {
        // Destroy any duplicate instances that may have been created
        if (_instance != null && _instance != this)
        {
            Debug.Log("destroying singleton");
            Destroy(this);
            return;
        }
        _instance = this;
    }
    #endregion

    private void Awake()
    {
        InitializeSingleton();

        // setup variables
        _aRRaycastManager = GetComponent<ARRaycastManager>();
        _raycastHits = new List<ARRaycastHit>();
        _aRAnchorManager = GetComponent<ARAnchorManager>();
        _arImageManager = GetComponent<ARTrackedImageManager>();
        _arCameraManager = GetComponentInChildren<ARCameraManager>();
        if (!_arCameraManager)
            throw new MissingComponentException("ARCameraManager component not found!");
    }


    private void OnEnable()
    {
        _aRAnchorManager.anchorsChanged += OnAnchorsChanged;
    }

    private void OnDisable()
    {
        _aRAnchorManager.anchorsChanged -= OnAnchorsChanged;
    }


    private void OnAnchorsChanged(ARAnchorsChangedEventArgs args)
    {
        if (args.added.Count > 0)
        {
            foreach (ARAnchor anchor in args.added)
            {
                Debug.Log("[INTERACTION_MANAGER]: added anchor " + anchor.name);
            }
        }

        if (args.updated.Count > 0)
        {
            foreach (ARAnchor anchor in args.updated)
            {
                Debug.Log("[INTERACTION_MANAGER]: updated anchor " + anchor.name);
            }
        }

        if (args.removed.Count > 0)
        {
            foreach (ARAnchor anchor in args.removed)
            {
                Debug.Log("[INTERACTION_MANAGER]: removed anchor " + anchor.name);
            }
        }
    }

    private void Start()
    {
        // get camera in children
        _arCamera = GetComponentInChildren<Camera>();
        if (!_arCamera)
            throw new MissingComponentException("[INTERACTION_MANAGER] Camera not found in children of Interaction manager!");
    }


    // Метод для открытия окна описания и закрытия начального окна
    public void OpenDescriptionWindow()
    {
        startWindow.SetActive(false);
        descriptionWindow.SetActive(true);
    }

    // Метод для открытия основного окна и закрытия окна описания
    public void OpenMainWindow()
    {
        descriptionWindow.SetActive(false);
        mainWindow.SetActive(true);
    }

    public List<ARRaycastHit> GetARRaycastHits(Vector2 touchPosition, TrackableType trackable = TrackableType.Planes)
    {
        _aRRaycastManager.Raycast(
            screenPoint: touchPosition,
            hitResults: _raycastHits,
            trackableTypes: trackable
        );
        return _raycastHits;
    }

}
