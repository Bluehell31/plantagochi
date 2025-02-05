using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GestorDeDatos : MonoBehaviour
{
  
    public static GestorDeDatos Instance { get; private set; }
    private string rutaArchivo;

    private void Awake()
    {
          if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ✅ Se mantiene entre escenas
            rutaArchivo = Application.persistentDataPath + "/datos_jugador.json";
        }
        else
        {
            Debug.LogWarning("⚠️ Se detectó una instancia duplicada de GestorDeDatos. Eliminando...");
            Destroy(gameObject); // ✅ Se destruye la nueva instancia para evitar duplicados
            return;
        }
        rutaArchivo = Application.persistentDataPath + "/datos_jugador.json";
    }

    // Guardar datos en JSON
    public void GuardarDatos(DatosJugador datos)
    {
        try
        {
            string json = JsonUtility.ToJson(datos, true); // Convierte a JSON
            Debug.Log(json);
            File.WriteAllText(rutaArchivo, json);         // Escribe el archivo
            Debug.Log("📁 Datos guardados correctamente en: " + rutaArchivo);
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Error al guardar los datos: " + e.Message);
        }
    }


    // Cargar datos desde JSON
    public DatosJugador CargarDatos()
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
                lastUpdatedDate = System.DateTime.Now.ToString(),
                monedas = 10
            };
        }
    }
}

