using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class ResetScene : MonoBehaviour
{
    private GestorDeDatos gestorDeDatos;

    public void ResetSceneToStart()
    {
        gestorDeDatos = FindObjectOfType<GestorDeDatos>();

        if (gestorDeDatos == null)
        {
            Debug.LogError("❌ Error: No se encontró el GestorDeDatos en la escena.");
            return; // Evita que el código continúe si `gestorDeDatos` es null
        }

        // Restablecer los datos del jugador
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
            levelUpDoneToday = false
        };

        // Eliminar PlayerPrefs (aunque ya no se usa, por si quedaron datos antiguos)
        PlayerPrefs.DeleteAll();

        // Guardar los nuevos datos en JSON
        gestorDeDatos.GuardarDatos(datosJugador);

        Debug.Log("📁 Datos reiniciados. Recargando escena...");

        // Recargar la escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
