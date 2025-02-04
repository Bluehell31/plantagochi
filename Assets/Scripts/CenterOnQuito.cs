using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class CenterOnQuito : MonoBehaviour
{
    [SerializeField] private AbstractMap map;
    [SerializeField] private Vector2d quitoCoordinates = new Vector2d(-0.1807, -78.4678);

      private void Start()
    {
        if (map != null)
        {
            // Convertir las coordenadas geográficas a una posición en el mundo
            Vector3 position = map.GeoToWorldPosition(quitoCoordinates);
            transform.position = position;

            Debug.Log($"Transform para Quito fijado en posición mundial: {position}");
        }
        else
        {
            Debug.LogError("Mapa no asignado en CenterOnQuito");
        }
    }
}