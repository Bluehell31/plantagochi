using UnityEngine;
using UnityEngine.UI; // Necesario para trabajar con el componente Text y Button

public class DebugMenu : MonoBehaviour
{
    // Referencias a los elementos de la UI y al script HealthAndExpSprites
    public GameObject DebugMenuPanel;  // Referencia al Canvas del Debug Menu
    public HealthAndExpSprites healthAndExpSprites; // Referencia al script HealthAndExpSprites

    public PlantLevelManager plantLevelManager;
    public ButtonHandler buttonHandler;
    public Text timeText; // Referencia al Text para mostrar el tiempo transcurrido

    // Variables para controlar el tiempo simulado
    private float simulatedTime = 0f;
    private float timeScale = 1f;  // Escala del tiempo (1x normal, 2x acelerado, etc.)
    private float simulatedDayTime = 24f * 3600f; // Tiempo de un día completo en segundos

    void Start()
    {
        // Inicializamos el tiempo en la UI
        UpdateTimeText();

        // Aseguramos que el menú de depuración esté apagado al inicio
        DebugMenuPanel.SetActive(false);  // Desactiva el panel al inicio
    }

    // Función para aumentar la velocidad del tiempo
    public void IncreaseTimeScale()
    {
        timeScale *= 2f;  // Duplicar la velocidad del tiempo
        Time.timeScale = timeScale;  // Aplicar el cambio a Time.timeScale
        Debug.Log("Velocidad del tiempo: " + timeScale);
    }

    // Función para disminuir la velocidad del tiAdd
    public void DecreaseTimeScale()
    {
        timeScale /= 2f;  // Reducir la velocidad del tiempo
        Time.timeScale = timeScale;  // Aplicar el cambio a Time.timeScale
        Debug.Log("Velocidad del tiempo: " + timeScale);
    }

    // Función para resetear el tiempo a su valor normal
    public void ResetTimeScale()
    {
        timeScale = 1f;  // Restablecer a la velocidad normal
        Time.timeScale = timeScale;  // Aplicar el cambio
        Debug.Log("Velocidad del tiempo restablecida");
    }

    // Actualiza el tiempo transcurrido en la UI
    public void GoToNextDay()
    {
        simulatedTime += Time.deltaTime * timeScale;  // Aumenta el tiempo simulado con el tiempo ajustado
        UpdateTimeText();
        
        // Si se simula un día completo, reiniciamos el tiempo y actualizamos las barras
        if (simulatedTime >= simulatedDayTime)
        {
            simulatedTime = 0f; // Reseteamos el tiempo
            Debug.Log("Un día completo ha pasado. Se actualizan las barras y el estado.");
            healthAndExpSprites.HandleEnergyAndHealth();  // Actualiza la salud y la experiencia de la planta
        }
    }

    // Muestra el tiempo transcurrido en formato HH:MM:SS
    void UpdateTimeText()
    {
        // Convertir los segundos a horas, minutos y segundos
        int hours = Mathf.FloorToInt(simulatedTime / 3600);
        int minutes = Mathf.FloorToInt((simulatedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(simulatedTime % 60);

        // Formatear el tiempo en HH:MM:SS
        timeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }

    // Funciones para los botones en el menú de depuración

    // Aumenta la experiencia
    /*public void IncreaseExperience(int amount)
    {
        healthAndExpSprites.AddExperienceAndHealth(amount, 0);  // Solo aumentamos la experiencia
        Debug.Log("Experiencia aumentada: " + amount);
    }*/

    // Aumenta la salud
    public void IncreaseHealth(int amount)
    {
        healthAndExpSprites.AddHealth(amount);  // Solo aumentamos la salud
        Debug.Log("Salud aumentada: " + amount);
    }

    // Cambiar el estado de la planta (Ej: ponerla muerta)
    /*public void ChangePlantStateToDead()
    {
        healthAndExpSprites.PonerVidaEnCero();  // Ponemos la salud a 0
        Debug.Log("La planta está muerta.");
    }*/

    // Resetear el progreso (salud, experiencia, nivel)
    /*public void ResetProgress()
    {
        PlayerPrefs.SetFloat("CurrentHealth", currentHealth);  // Establecemos la salud a su valor máximo
        plantLevelManager.SetPlayerLevel(1);  // Establecemos el nivel a 1
        PlayerPrefs.DeleteAll();  // Elimina los datos guardados de PlayerPrefs
        Debug.Log("Progreso reseteado.");
    }*/

    /*// Cambiar el nivel de la planta
    public void ChangePlayerLevel(int level)
    {
        healthAndExpSprites.SetPlayerLevel(level);  // Cambiamos el nivel del jugador
        Debug.Log("Nivel del jugador cambiado a: " + level);
        healthAndExpSprites.UpdateStats();
    }*/

    

    // Añadir monedas para pruebas (si es necesario)
    public void AddCoinsDebug(int cantidad)
    {
        Debug.Log($"📂 Ruta del archivo JSON: {Application.persistentDataPath}/datos_jugador.json");

        buttonHandler.AddCoins(cantidad);  // Cambiamos las monedas
        Debug.Log("Monedas Añadidas");
    }

    // Mostrar u ocultar el menú de depuración
    public void ToggleDebugMenu()
    {
        DebugMenuPanel.SetActive(!DebugMenuPanel.activeSelf);  // Alterna entre mostrar y ocultar el menú
    }
}
