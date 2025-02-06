using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera arCam;
    [SerializeField] private ARRaycastManager _raycastManager;
    [SerializeField] private ARPlaneManager _planeManager;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Button placementButton;

    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    private Pose pose;
    private Touch touch;
    private DataHandler dataHandler;
    private bool isPlacing = false;

    void Start()
    {
        dataHandler = DataHandler.Instance;

        // Usar la instancia centralizada, si existe
        if (dataHandler.currentPlantInstance != null)
        {
            Debug.Log($"🌱 Planta recuperada al iniciar: {dataHandler.currentPlantInstance.name} en posición {dataHandler.GetLastSavedPosition()}");
        }
        // Si no existe y hay datos guardados, instanciarla
        else if (dataHandler.GetLastSavedPosition() != Vector3.zero && dataHandler.plantPrefab != null)
        {
            dataHandler.currentPlantInstance = Instantiate(dataHandler.plantPrefab, dataHandler.GetLastSavedPosition(), dataHandler.GetLastSavedRotation());
            Debug.Log($"🌱 Planta instanciada al iniciar: {dataHandler.currentPlantInstance.name} en posición {dataHandler.GetLastSavedPosition()}");
        }
        else
        {
            Debug.Log("⚠ No hay planta guardada en DataHandler.");
        }

        SetPlacementMode(false);
        _planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDestroy() => _planeManager.planesChanged -= OnPlanesChanged;

    void Update()
    {
        if (!isPlacing) return;
        CrossHairCalculation();
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !IsPointerOverUI(touch))
            {
                PlacePlant();
                SetPlacementMode(false);
            }
        }
    }

    public void TogglePlacementMode()
    {
        if (!isPlacing)
        {
            SetPlacementMode(true);
            placementButton.interactable = false;
            Debug.Log("🟢 Modo colocación activado.");
        }
    }

    private void SetPlacementMode(bool active)
    {
        isPlacing = active;
        crosshair.SetActive(active);
        if (active)
        {
            _planeManager.enabled = true;
            foreach (var plane in _planeManager.trackables)
                plane.gameObject.SetActive(true);
            Debug.Log("🟢 Modo colocación activado: crosshair y planos habilitados.");
        }
        else
        {
            _planeManager.enabled = false;
            foreach (var plane in _planeManager.trackables)
                plane.gameObject.SetActive(false);
            placementButton.interactable = true;
            Debug.Log("🔴 Modo colocación desactivado: crosshair y planos deshabilitados.");
        }
    }

    private bool IsPointerOverUI(Touch touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = touch.position };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    private void CrossHairCalculation()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f);
        if (_raycastManager.Raycast(screenCenter, _hits, TrackableType.Planes))
        {
            pose = _hits[0].pose;
            crosshair.transform.position = pose.position;
            crosshair.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }

    private void PlacePlant()
    {
        // Si ya existe la planta, actualizar su posición y rotación
        if (dataHandler.currentPlantInstance != null)
        {
            dataHandler.currentPlantInstance.transform.position = pose.position;
            dataHandler.currentPlantInstance.transform.rotation = pose.rotation;
            Debug.Log("✅ Planta reposicionada.");
        }
        // Si no existe, instanciar el prefab
        else if (dataHandler.plantPrefab != null)
        {
            dataHandler.currentPlantInstance = Instantiate(dataHandler.plantPrefab, pose.position, pose.rotation);
            Debug.Log("✅ Planta instanciada.");
        }
        else
        {
            Debug.LogError("No se ha asignado un prefab de planta en DataHandler.");
        }
        dataHandler.SavePlantPosition(pose.position, pose.rotation);
        SetPlacementMode(false);
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (var newPlane in args.added)
            Debug.Log($"🟢 Nuevo plano detectado: {newPlane.trackableId}, Tamaño: {newPlane.size}");
        foreach (var updatedPlane in args.updated)
            Debug.Log($"🟡 Plano actualizado: {updatedPlane.trackableId}, Nuevo tamaño: {updatedPlane.size}");
        foreach (var removedPlane in args.removed)
            Debug.Log($"🔴 Plano eliminado: {removedPlane.trackableId}");
    }
}
