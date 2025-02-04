using Mapbox.Unity.Map;
using UnityEngine;

public class MariscalButton : MonoBehaviour
{
    [SerializeField] private MapInteractionController mapController;
    private Vector3 mariscalPosition;

    void Start()
    {
        if (mapController == null)
            mapController = GameObject.Find("Map").GetComponent<MapInteractionController>();

        // Guardar posición inicial del jugador
        
        mariscalPosition = new Vector3(281.9355f, 0f, -141.1019f);
    }

    public void CenterMapMariscal()
    {
        mapController.transform.position = mariscalPosition;
    }
}