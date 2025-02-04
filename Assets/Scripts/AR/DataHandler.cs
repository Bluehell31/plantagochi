using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    public GameObject plantModel;
    private GestorDeDatos gestorDeDatos;

    private static DataHandler instance;
    
    public static DataHandler Instance  
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataHandler>();
            }
            return instance;
        }
    }

    void Start()
    {
        gestorDeDatos = FindObjectOfType<GestorDeDatos>(); // Obtener referencia
        LoadPlantPosition();
    }

    public void SavePlantPosition(Vector3 position, Quaternion rotation)
    {
        DatosJugador datosJugador = gestorDeDatos.CargarDatos();
        datosJugador.plantPosition = position;
        datosJugador.plantRotation = rotation;
        gestorDeDatos.GuardarDatos(datosJugador);
        Debug.Log("🌱 Posición de la planta guardada en JSON.");
    }



    public void LoadPlantPosition()
    {
        DatosJugador datosJugador = gestorDeDatos.CargarDatos();
        lastSavedPosition = datosJugador.plantPosition;
        lastSavedRotation = datosJugador.plantRotation;

        Debug.Log("🌱 Posición de la planta cargada desde JSON.");
    }


    // Variables privadas para almacenar la última posición guardada
    private Vector3 lastSavedPosition;
    private Quaternion lastSavedRotation;

    // Método para obtener la última posición guardada desde InputManager
    public Vector3 GetLastSavedPosition()
    {
        return lastSavedPosition;
    }

    public Quaternion GetLastSavedRotation()
    {
        return lastSavedRotation;
    }

}
