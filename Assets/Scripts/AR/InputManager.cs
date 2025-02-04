using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera arCam;
    [SerializeField] private ARRaycastManager _raycastManager;
    [SerializeField] private ARPlaneManager _planeManager;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Button placementButton; // Cambiar a Button


    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    private Pose pose;
    private Touch touch;
    private GameObject placedPlant;
    private DataHandler dataHandler;
    private bool isPlacing = false;

    void Start()
    {
        dataHandler = DataHandler.Instance;

        if (dataHandler.GetLastSavedPosition() != Vector3.zero && dataHandler.plantModel != null)
        {
            placedPlant = Instantiate(dataHandler.plantModel, dataHandler.GetLastSavedPosition(), dataHandler.GetLastSavedRotation());
            Debug.Log($"🌱 Planta recuperada al iniciar: {placedPlant.name} en posición {dataHandler.GetLastSavedPosition()}");
        }
        else
        {
            Debug.Log("⚠ No hay planta guardada en DataHandler.");
        }

        SetPlacementMode(false);

        // 🔹 Suscribirse al evento de detección de planos
        _planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDestroy()
    {
        // 🔹 Asegurar que el evento se desuscriba al destruir el objeto
        _planeManager.planesChanged -= OnPlanesChanged;
    }

    void Update()
    {
        if (!isPlacing) return; // Solo ejecuta si el modo colocación está activo

        CrossHairCalculation();

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && !IsPointerOverUI(touch))
            {
                PlacePlant();
                SetPlacementMode(false); // Desactivar modo colocación después de ubicar la planta
            }
        }
    }

    public void TogglePlacementMode()
    {
        if (!isPlacing)
        {
            SetPlacementMode(true); // Activar el modo colocación
            placementButton.interactable = false; // Desactivar el botón mientras se coloca
            Debug.Log("🟢 Botón presionado: modo colocación activado.");
        }
    }


    private void SetPlacementMode(bool active)
    {
        isPlacing = active;
        crosshair.SetActive(active);

        if (active)
        {
            // Activar detección de planos
            _planeManager.enabled = true;

            foreach (var plane in _planeManager.trackables)
            {
                plane.gameObject.SetActive(true);
            }

            Debug.Log("🟢 Modo colocación activado: crosshair y detección de planos habilitados.");
        }
        else
        {
            // Desactivar detección de planos
            _planeManager.enabled = false;

            foreach (var plane in _planeManager.trackables)
            {
                Debug.Log($"❌ Desactivando plano: {plane.trackableId}");
                plane.gameObject.SetActive(false);
            }

            // Reactivar el botón al desactivar el modo colocación
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

        if (_raycastManager.Raycast(screenCenter, _hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
        {
            pose = _hits[0].pose;
            crosshair.transform.position = pose.position;
            crosshair.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }

    private void PlacePlant()
    {
        if (placedPlant != null)
        {
            Destroy(placedPlant); // Eliminar la planta existente
        }

        // Crear una nueva planta en la posición del crosshair
        placedPlant = Instantiate(dataHandler.plantModel, pose.position, pose.rotation);
        dataHandler.SavePlantPosition(pose.position, pose.rotation);

        // 🔹 Desactivar el modo de colocación inmediatamente después de colocar la planta
        SetPlacementMode(false);

        Debug.Log("✅ Planta colocada. Modo de colocación desactivado automáticamente.");
    }




    // 🔹 Función que registra cuando se crean, actualizan o eliminan planos
    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (var newPlane in args.added)
        {
            Debug.Log($"🟢 Nuevo plano detectado: {newPlane.trackableId}, Tamaño: {newPlane.size}");
        }

        foreach (var updatedPlane in args.updated)
        {
            Debug.Log($"🟡 Plano actualizado: {updatedPlane.trackableId}, Nuevo tamaño: {updatedPlane.size}");
        }

        foreach (var removedPlane in args.removed)
        {
            Debug.Log($"🔴 Plano eliminado: {removedPlane.trackableId}");
        }
    }
}
