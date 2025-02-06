using System;
using System.IO;
using UnityEngine;

public class GestorDeDatos : MonoBehaviour
{
    public static GestorDeDatos Instance { get; private set; }
    private string rutaArchivo;
    private DatosJugador datosJugador; // Objeto único en memoria

    // Evento para notificar cuando los datos se actualizan
    public event Action OnDataChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            rutaArchivo = Application.persistentDataPath + "/datos_jugador.json";
            datosJugador = CargarDatosDesdeArchivo();
        }
        else
        {
            Debug.LogWarning("⚠️ Se detectó una instancia duplicada de GestorDeDatos. Eliminando...");
            Destroy(gameObject);
        }
    }

    private DatosJugador CargarDatosDesdeArchivo()
    {
        if (File.Exists(rutaArchivo))
        {
            string json = File.ReadAllText(rutaArchivo);
            return JsonUtility.FromJson<DatosJugador>(json);
        }
        else
        {
            Debug.LogWarning("No se encontró archivo de datos. Creando nuevos datos.");
            return new DatosJugador()
            {
                nivel = 1,
                experiencia = 0,
                currentHealth = 10,
                lastUpdatedDate = DateTime.Now.ToString("yyyy-MM-dd"),
                monedas = 50,
                cantidadPolen = 0,
                cantidadFertilizante = 0
            };
        }
    }

    // Devuelve la única instancia de DatosJugador
    public DatosJugador GetDatosJugador() => datosJugador;

    // Guarda la única copia en memoria y notifica a los suscriptores
    public void GuardarDatos()
    {
        try
        {
            string json = JsonUtility.ToJson(datosJugador, true);
            File.WriteAllText(rutaArchivo, json);
            Debug.Log("📁 Datos guardados correctamente en: " + rutaArchivo);
            OnDataChanged?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Error al guardar los datos: " + e.Message);
        }
    }

    // Ejemplo de método centralizado para actualizar la salud
    public void ActualizarSalud(float nuevaSalud)
    {
        datosJugador.currentHealth = nuevaSalud;
        GuardarDatos();
    }

    // Método para actualizar otros datos (monedas, nivel, etc.) se implementaría de manera similar
    public void ActualizarDatos(DatosJugador nuevosDatos)
    {
        datosJugador = nuevosDatos;
        GuardarDatos();
    }
}
