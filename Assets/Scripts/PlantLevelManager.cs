using System;
using UnityEngine;
using TMPro;  // Recuerda añadir "using TMPro;" si usas TextMeshProUGUI

public class PlantLevelManager : MonoBehaviour
{
    public static PlantLevelManager Instance { get; private set; }

    [Header("Panel y texto de subida de nivel (opcional)")]
    [SerializeField] private GameObject LevelUpPanel;      // Panel que se muestra al subir nivel
    [SerializeField] private TextMeshProUGUI levelUpText;  // Texto que muestra el nuevo nivel

    // Referencias a la lógica de guardado
    private int jugadorNivel;
    private GestorDeDatos gestorDeDatos;  
    private DatosJugador datosJugador;     

    // Configuraciones de nivel
    private const int maxLevel = 10;
    private const int levelUpThreshold = 3; // La planta puede cambiar de apariencia cada 3 niveles

    void Awake()
    {
        // Singleton básico
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Buscar el GestorDeDatos (o asignarlo por inspector, como prefieras)
        gestorDeDatos = FindObjectOfType<GestorDeDatos>();
        if (gestorDeDatos == null)
        {
            Debug.LogError("❌ No se encontró GestorDeDatos en la escena.");
            return;
        }

        // Cargar datos desde JSON
        datosJugador = gestorDeDatos.CargarDatos();
        jugadorNivel= datosJugador.nivel;
        // Aseguramos que el nivel sea al menos 1 (o lo que consideres apropiado si no había datos)
        if (jugadorNivel < 1)
        {
            jugadorNivel = 1;
            datosJugador.nivel = jugadorNivel;
            gestorDeDatos.GuardarDatos(datosJugador);
        }

        Debug.Log($"🌱 Nivel inicial de la planta: {datosJugador.nivel}");
    }

    /// <summary>
    /// Obtener el nivel actual de la planta.
    /// </summary>
    public int GetPlantLevel()
    {
        // Si no se han cargado datos por alguna razón, regresamos un valor por defecto
        return (datosJugador != null) ? datosJugador.nivel : 1;
    }

    /// <summary>
    /// Sube el nivel de la planta en 1, siempre y cuando no supere el máximo.
    /// </summary>
    public void IncreaseLevel()
    {
        if (datosJugador == null) return;
        
        jugadorNivel = datosJugador.nivel;
        // Verificar si no se alcanzó el nivel máximo
        if (jugadorNivel < maxLevel)
        {
            jugadorNivel++;
            datosJugador.nivel = jugadorNivel;
            gestorDeDatos.GuardarDatos(datosJugador);

            Debug.Log($"🌱 La planta ha subido de nivel. Nuevo nivel: {datosJugador.nivel}");

            // Método para manejar acciones tras subir nivel (UI, cambios de apariencia, etc.)
            OnLevelUpdated();
        }
        else
        {
            Debug.Log("🌱 La planta ya está en el nivel máximo.");
        }
    }

    /// <summary>
    /// Ajusta directamente el nivel de la planta (por ejemplo, para debug o restaurar backup).
    /// </summary>
    public void SetPlayerLevel(int newLevel)
    {
        if (datosJugador == null) return;

        // Limitar el nivel entre 1 y maxLevel
        jugadorNivel = datosJugador.nivel;
        jugadorNivel = Mathf.Clamp(newLevel, 1, maxLevel);
        datosJugador.nivel = jugadorNivel;
        gestorDeDatos.GuardarDatos(datosJugador);

        Debug.Log($"🌱 Se ha establecido el nivel de la planta a {datosJugador.nivel}");
    }

    /// <summary>
    /// Se llama cada vez que el nivel cambia. Aquí puedes actualizar modelos, UI, etc.
    /// </summary>
    private void OnLevelUpdated()
    {
        // Mostrar el panel de Level Up (si está disponible)
        ShowLevelUpPanel();

        // Ejemplo: si quieres cambiar el modelo de la planta cada X niveles,
        // podrías llamarlo aquí. Por ejemplo:
        // if (datosJugador.nivel % levelUpThreshold == 0)
        // {
        //     // Cambiar apariencia de la planta
        //     CenterObjectInCanvas.Instance.AssignPlantModel(datosJugador.nivel);
        // }
    }

    /// <summary>
    /// Muestra un panel de "Subida de nivel" con el texto del nivel actual.
    /// </summary>
    private void ShowLevelUpPanel()
    {
        if (LevelUpPanel != null && levelUpText != null)
        {
            LevelUpPanel.SetActive(true);
            levelUpText.text = $"Felicidades! Has subido al nivel {datosJugador.nivel}";
        }
        else
        {
            Debug.LogError("❌ LevelUpPanel o levelUpText no están asignados en el inspector.");
        }
    }

    /// <summary>
    /// Cierra el panel de "Subida de nivel".
    /// </summary>
    public void CloseLevelUpPanel()
    {
        if (LevelUpPanel != null)
        {
            LevelUpPanel.SetActive(false);
        }
    }
}