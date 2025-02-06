using UnityEngine;
using TMPro;

public class PlantLevelManager : MonoBehaviour
{
    public static PlantLevelManager Instance { get; private set; }

    [Header("Panel y texto de subida de nivel (opcional)")]
    [SerializeField] private GameObject LevelUpPanel;
    [SerializeField] private TextMeshProUGUI levelUpText;

    private DatosJugador datosJugador;

    private const int maxLevel = 10;
    private const int levelUpThreshold = 3;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (GestorDeDatos.Instance == null)
        {
            Debug.LogError("No se encontró GestorDeDatos en la escena.");
            return;
        }
        datosJugador = GestorDeDatos.Instance.GetDatosJugador();
        if (datosJugador.nivel < 1)
        {
            datosJugador.nivel = 1;
            GestorDeDatos.Instance.GuardarDatos();
        }
        Debug.Log($"Nivel inicial de la planta: {datosJugador.nivel}");
    }

    public int GetPlantLevel()
    {
        return GestorDeDatos.Instance.GetDatosJugador().nivel;
    }

    public void IncreaseLevel()
    {
        datosJugador = GestorDeDatos.Instance.GetDatosJugador();
        if (datosJugador.nivel < maxLevel)
        {
            datosJugador.nivel++;
            GestorDeDatos.Instance.GuardarDatos();
            Debug.Log($"La planta ha subido de nivel. Nuevo nivel: {datosJugador.nivel}");
            OnLevelUpdated();
        }
        else
        {
            Debug.Log("La planta ya está en el nivel máximo.");
        }
    }

    public void SetPlayerLevel(int newLevel)
    {
        datosJugador = GestorDeDatos.Instance.GetDatosJugador();
        datosJugador.nivel = Mathf.Clamp(newLevel, 1, maxLevel);
        GestorDeDatos.Instance.GuardarDatos();
        Debug.Log($"Se ha establecido el nivel de la planta a {datosJugador.nivel}");
    }

    private void OnLevelUpdated()
    {
        ShowLevelUpPanel();
    }

    private void ShowLevelUpPanel()
    {
        if (LevelUpPanel != null && levelUpText != null)
        {
            LevelUpPanel.SetActive(true);
            levelUpText.text = $"Felicidades! Has subido al nivel {datosJugador.nivel}";
        }
        else
        {
            Debug.LogError("LevelUpPanel o levelUpText no están asignados.");
        }
    }

    public void CloseLevelUpPanel()
    {
        if (LevelUpPanel != null)
        {
            LevelUpPanel.SetActive(false);
        }
    }
}
