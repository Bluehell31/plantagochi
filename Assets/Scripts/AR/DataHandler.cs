using UnityEngine;

public class DataHandler : MonoBehaviour
{
    // Asigna en el inspector el prefab de la planta
    public GameObject plantPrefab;
    // Aquí se almacenará la instancia creada en la escena
    public GameObject currentPlantInstance;

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

    // Variables privadas para la posición y rotación guardadas
    private Vector3 lastSavedPosition;
    private Quaternion lastSavedRotation;

    void Start()
    {
        if (GestorDeDatos.Instance == null)
        {
            Debug.LogError("GestorDeDatos no se encontró en la escena.");
            return;
        }
        gestorDeDatos = GestorDeDatos.Instance;
        LoadPlantPosition();
    }

    public void SavePlantPosition(Vector3 position, Quaternion rotation)
    {
        DatosJugador datos = gestorDeDatos.GetDatosJugador();
        datos.plantPosition = position;
        datos.plantRotation = rotation;
        gestorDeDatos.GuardarDatos();
        Debug.Log("🌱 Posición de la planta guardada en JSON.");
    }

    public void LoadPlantPosition()
    {
        DatosJugador datos = gestorDeDatos.GetDatosJugador();
        lastSavedPosition = datos.plantPosition;
        lastSavedRotation = datos.plantRotation;
        Debug.Log("🌱 Posición de la planta cargada desde JSON.");
    }

    public Vector3 GetLastSavedPosition() => lastSavedPosition;
    public Quaternion GetLastSavedRotation() => lastSavedRotation;
}
