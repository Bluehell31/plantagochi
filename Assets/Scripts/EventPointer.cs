using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//importe de mapbox y utilidades
using Mapbox.Examples;
using Mapbox.Utils;     
public class EventPointer : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f; // Velocidad de rotación
    [SerializeField] private float amplitude = 2.0f;    // Amplitud del movimiento
    [SerializeField] private float frequency = 0f;    // Frecuencia del movimiento

    LocationStatus playerLocation;
    public int eventID;
    MenuUIManager menuUIManager;
    EventManager eventManager;
    public Vector2d eventPos;
    // Start es llamado antes de la primera actualización de fotogramas
    void Start()
    {
        menuUIManager = GameObject.Find("Canvas").GetComponent<MenuUIManager>();
        eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();
        // Inicialización si es necesaria
    }

    // Update es llamado una vez por frame
    void Update()
    {
        FloatAndRotatePointer(); // Llama al método para rotar y mover
    }

    // Método que hace que el objeto rote y flote
    void FloatAndRotatePointer()
    {
        // Rotación
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Movimiento en el eje Y
        transform.position = new Vector3(transform.position.x, (Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude)+ 5, transform.position.z);
    }
    private void OnMouseDown() {
     
        playerLocation = GameObject.Find("Canvas").GetComponent<LocationStatus>();
        var currentPlayerLocation= new GeoCoordinatePortable.GeoCoordinate(playerLocation.GetLocationLat(), playerLocation.GetLocationLon());
        var eventLocation = new GeoCoordinatePortable.GeoCoordinate(eventPos[0], eventPos[1]);
        var distance = currentPlayerLocation.GetDistanceTo(eventLocation);
        Debug.Log("la distancia es: "+distance);
        if(distance<=eventManager.maxDistance){
        menuUIManager.DisplayUserInRangePanel();
        }else
        {
        menuUIManager.DisplayUserOutOfRangePanel();   
        }
        
    }
}
