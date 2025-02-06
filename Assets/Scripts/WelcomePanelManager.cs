using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        if (GestorDeDatos.Instance == null)
        {
            Debug.LogError("GestorDeDatos no se encontró en la escena.");
            return;
        }
        gestorDeDatos = GestorDeDatos.Instance;
        datosJugador = gestorDeDatos.GetDatosJugador();

        if (datosJugador == null)
        {
            Debug.LogError("datosJugador es NULL. Se crearán nuevos datos...");
            datosJugador = new DatosJugador() { nombreJugador = "user001" };
            gestorDeDatos.GuardarDatos();
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
            Debug.LogError("GestorDeDatos es NULL.");
            return;
        }

        if (datosJugador == null)
        {
            Debug.LogError("datosJugador es NULL.");
            return;
        }

        if (playerNameInput == null)
        {
            Debug.LogError("playerNameInput no está asignado en el Inspector.");
            return;
        }

        string playerName = playerNameInput.text;
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "user001";
        }

        datosJugador.nombreJugador = playerName;
        gestorDeDatos.GuardarDatos();

        if (welcomePanel != null)
            welcomePanel.SetActive(false);
        else
            Debug.LogError("welcomePanel no está asignado en el Inspector.");

        if (gameElements != null)
            gameElements.SetActive(true);
        else
            Debug.LogError("gameElements no está asignado en el Inspector.");

        if (startScreenManager != null)
        {
            startScreenManager.OnGameStarted();
        }
        else
        {
            Debug.LogError("startScreenManager no está asignado en el Inspector.");
        }
    }
}
