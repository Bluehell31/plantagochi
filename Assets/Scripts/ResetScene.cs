using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{
    public void ResetSceneToStart()
    {
        // Se utiliza el singleton del GestorDeDatos
        if (GestorDeDatos.Instance == null)
        {
            Debug.LogError("❌ Error: No se encontró el GestorDeDatos en la escena.");
            return;
        }

        // Crear nuevos datos por defecto para el jugador
        DatosJugador datosJugador = new DatosJugador()
        {
            nombreJugador = "user001",
            nivel = 1,
            experiencia = 0,
            currentHealth = 10f,
            receivedWater = false,
            receivedNutrients = false,
            receivedSparkles = false,
            lastUpdatedDate = System.DateTime.Now.ToString("yyyy-MM-dd"),
            completedDailyTasks = false,
            monedas = 50,
            levelUpDoneToday = false,
            // Si es necesario, agrega otros campos como cantidadPolen y cantidadFertilizante:
            cantidadPolen = 0,
            cantidadFertilizante = 0
        };

        // Eliminar datos antiguos de PlayerPrefs, en caso de que hayan quedado
        PlayerPrefs.DeleteAll();

        // Actualizar la copia en memoria con los nuevos datos y guardarlos en JSON
        GestorDeDatos.Instance.ActualizarDatos(datosJugador);

        Debug.Log("📁 Datos reiniciados. Recargando escena...");

        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
