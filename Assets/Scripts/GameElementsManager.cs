using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameElementsManager : MonoBehaviour
{
    private DatosJugador datosJugador;

    public Text playerNameText;
    public GameObject shopPolinizarPanel;
    public GameObject imagePanel;
    private ImagePanelManager imagePanelManager;
    public GameObject gameElementsPanel;
    public GameObject TiendaPlantas; // Panel de tienda de plantas

    void Start()
    {
        if (GestorDeDatos.Instance == null)
        {
            Debug.LogError("GestorDeDatos no se encontró en la escena.");
            return;
        }

        // Se obtiene la copia en memoria
        datosJugador = GestorDeDatos.Instance.GetDatosJugador();

        if (string.IsNullOrEmpty(datosJugador.nombreJugador))
        {
            datosJugador.nombreJugador = "user001"; // Valor por defecto
        }

        if (playerNameText != null)
            playerNameText.text = datosJugador.nombreJugador;

        GestorDeDatos.Instance.GuardarDatos();

        imagePanelManager = imagePanel.GetComponent<ImagePanelManager>();

        shopPolinizarPanel.SetActive(false);
        imagePanel.SetActive(false);
        TiendaPlantas.SetActive(false);
    }

    public void ToggleShopPolinizarPanel()
    {
        if (gameElementsPanel.activeSelf)
        {
            bool currentState = shopPolinizarPanel.activeSelf;
            shopPolinizarPanel.SetActive(!currentState);
        }
    }

    public void ShowImagePanel()
    {
        if (gameElementsPanel.activeSelf)
        {
            imagePanel.SetActive(true);
            gameElementsPanel.SetActive(false);
            if (imagePanelManager != null)
            {
                imagePanelManager.ResetToFirstImage();
            }
        }
    }

    public void ActivateGameElementsPanel()
    {
        gameElementsPanel.SetActive(true);
    }

    public void ShowTiendaPlantas()
    {
        if (gameElementsPanel.activeSelf)
        {
            TiendaPlantas.SetActive(true);
            gameElementsPanel.SetActive(false);
        }
    }
}
