using UnityEngine;
using Mapbox.Unity.Map;     // Asegúrate de tener este using si vas a usar AbstractMap
using Mapbox.Utils;         // Para Vector2d
using Mapbox.Examples; 
using Mapbox.Unity.Utilities;

public class MapInteractionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform mapTransform;
    [SerializeField] private Camera mapCamera;
    [SerializeField] private AbstractMap _map;

    [Header("Pan Settings")]
    [SerializeField] private float panSpeed = 15f;
    [SerializeField] private float mobilePanSpeed = 15f;
    [SerializeField] private float maxPanDelta = 50f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 0.5f;           // Desktop
    [SerializeField] private float pinchZoomSpeed = 0.2f;      // Móvil
    [SerializeField] private Vector2 zoomLimits = new Vector2(15f, 50f);

    [Header("Centro y escala")]
    [SerializeField] private Vector2d mapOriginLatLon; // Lat/lon que corresponde al (0,0) de tu plano
    [SerializeField] private float worldScale = 1f;    // factor de escalado metros <-> Unity

    private Vector3 targetPosition;

    private enum TwoFingerGesture { None, Zoom }
    private TwoFingerGesture currentGesture = TwoFingerGesture.None;

    private Vector2 lastTouchPosition;
    private float lastTouchDistance;
    private bool isDragging = false;

    private Vector3 initialMapPosition;
    private Vector2 originalBoundsMin;
    private Vector2 originalBoundsMax;
    public Vector2 mapBoundsMin;
    public Vector2 mapBoundsMax;

    private void Start()
    {
        if (mapTransform == null)
            mapTransform = transform;

        if (mapCamera == null)
            mapCamera = Camera.main;

        initialMapPosition = transform.position;
        originalBoundsMin = mapBoundsMin;
        originalBoundsMax = mapBoundsMax;
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            HandleMobileInput();
        }
        else
        {
            HandleDesktopInput();
        }
    }

    private void HandleDesktopInput()
    {
        // --- Pan con clic izquierdo ---
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastTouchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastTouchPosition;
            delta = Vector2.ClampMagnitude(delta, maxPanDelta);

            Vector3 translation = new Vector3(
                delta.x * panSpeed * Time.deltaTime,
                0f,
                delta.y * panSpeed * Time.deltaTime
            );

            mapTransform.Translate(translation, Space.World);
            lastTouchPosition = Input.mousePosition;
        }

        // --- Zoom con rueda del mouse ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float newFOV = mapCamera.fieldOfView - scroll * zoomSpeed;
            mapCamera.fieldOfView = Mathf.Clamp(newFOV, zoomLimits.x, zoomLimits.y);
        }
    }

    private void HandleMobileInput()
    {
        if (Input.touchCount == 0)
        {
            isDragging = false;
            currentGesture = TwoFingerGesture.None;
            return;
        }

        if (Input.touchCount == 1)
        {
            currentGesture = TwoFingerGesture.None;

            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isDragging = true;
                    lastTouchPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        Vector2 delta = touch.position - lastTouchPosition;
                        delta = Vector2.ClampMagnitude(delta, maxPanDelta);

                        Vector3 translation = new Vector3(
                            delta.x * mobilePanSpeed * Time.deltaTime,
                            0f,
                            delta.y * mobilePanSpeed * Time.deltaTime
                        );

                        mapTransform.Translate(translation, Space.World);
                        lastTouchPosition = touch.position;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            float currentTouchDistance = Vector2.Distance(touch0.position, touch1.position);
            float distDelta = currentTouchDistance - lastTouchDistance;

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                lastTouchDistance = currentTouchDistance;
                currentGesture = TwoFingerGesture.None;
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                if (currentGesture == TwoFingerGesture.None && Mathf.Abs(distDelta) > 0.1f)
                {
                    currentGesture = TwoFingerGesture.Zoom;
                }

                if (currentGesture == TwoFingerGesture.Zoom)
                {
                    float newFOV = mapCamera.fieldOfView - distDelta * pinchZoomSpeed;
                    mapCamera.fieldOfView = Mathf.Clamp(newFOV, zoomLimits.x, zoomLimits.y);
                }
            }

            lastTouchDistance = currentTouchDistance;
        }
    }

    public void FocusMapOnPlayer(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            Debug.LogError("Falta la referencia al transform del jugador.");
            return;
        }

        Vector3 camXZ = new Vector3(
            mapCamera.transform.position.x,
            0f,
            mapCamera.transform.position.z+ 70f
        );

        Vector3 playerXZ = new Vector3(
            playerTransform.position.x,
            0f,
            playerTransform.position.z
        );

        Vector3 offset = camXZ - playerXZ;
        mapTransform.position += offset;
    }

    public void ResetMapPosition()
    {
        mapTransform.position = Vector3.zero;
    }

    public void ResetZoom()
    {
        mapCamera.fieldOfView = (zoomLimits.x + zoomLimits.y) / 2f;
    }

    public void ResetToInitialState()
    {
        transform.position = initialMapPosition;
        mapBoundsMin = originalBoundsMin;
        mapBoundsMax = originalBoundsMax;
    }
}
