using UnityEngine;
using UnityEngine.UI;

public class WelcomePanelManager : MonoBehaviour
{
    private GestorDeDatos gestorDeDatos;
    private DatosJugador datosJugador;

    public GameObject welcomePanel;
    public InputField playerNameInput;
    public GameObject gameElements;
    public Button welcomePanelButton;
    public StartScreenManager startScreenManager;
    public CenterObjectInCanvas centerObjectScript; // Referencia al script CenterObjectInCanvas

    void Start()
    {
        gestorDeDatos = FindObjectOfType<GestorDeDatos>();

        if (gestorDeDatos == null)
        {
            Debug.LogError("❌ Error: No se encontró `GestorDeDatos` en la escena.");
            return;
        }

        datosJugador = gestorDeDatos.CargarDatos();

        if (datosJugador == null)
        {
            Debug.LogError("❌ Error: `datosJugador` es NULL. Intentando crear nuevos datos...");
            datosJugador = new DatosJugador() { nombreJugador = "user001" };
            gestorDeDatos.GuardarDatos(datosJugador);
        }

        welcomePanelButton.interactable = false;
    }

    public void OnPlayerNameChanged()
    {
        welcomePanelButton.interactable = !string.IsNullOrEmpty(playerNameInput.text);
    }

    public void OnWelcomePanelButtonPressed()
    {
        if (gestorDeDatos == null)
        {
            Debug.LogError("❌ Error: `gestorDeDatos` es NULL.");
            return;
        }

        if (datosJugador == null)
        {
            Debug.LogError("❌ Error: `datosJugador` es NULL.");
            return;
        }

        if (playerNameInput == null)
        {
            Debug.LogError("❌ Error: `playerNameInput` no está asignado en el Inspector.");
            return;
        }

        string playerName = playerNameInput.text;
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "user001";
        }

        datosJugador.nombreJugador = playerName;
        gestorDeDatos.GuardarDatos(datosJugador);

        if (welcomePanel != null)
            welcomePanel.SetActive(false);
        else
            Debug.LogError("❌ Error: `welcomePanel` no está asignado en el Inspector.");

        if (gameElements != null)
            gameElements.SetActive(true);
        else
            Debug.LogError("❌ Error: `gameElements` no está asignado en el Inspector.");

        if (startScreenManager != null)
        {
            startScreenManager.OnGameStarted();
        }
        else
        {
            Debug.LogError("❌ Error: `startScreenManager` no está asignado en el Inspector.");
        }
    }
}
