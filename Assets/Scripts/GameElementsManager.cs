using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameElementsManager : MonoBehaviour
{
    private GestorDeDatos gestorDeDatos;
    private DatosJugador datosJugador;


    public Text playerNameText;
    public GameObject shopPolinizarPanel;
    public GameObject imagePanel;
    private ImagePanelManager imagePanelManager;


    public GameObject gameElementsPanel;

    public GameObject TiendaPlantas; // Asigna el panel de tienda de plantas en el Inspector
    void Start()
    {
        gestorDeDatos = GestorDeDatos.Instance;
        datosJugador = gestorDeDatos.CargarDatos();

        if (string.IsNullOrEmpty(datosJugador.nombreJugador))
        {
            datosJugador.nombreJugador = "user001"; // Valor por defecto
            
        }

        playerNameText.text = datosJugador.nombreJugador;
        gestorDeDatos.GuardarDatos(datosJugador);

        imagePanelManager = imagePanel.GetComponent<ImagePanelManager>();

        shopPolinizarPanel.SetActive(false);
        imagePanel.SetActive(false);
        TiendaPlantas.SetActive(false);
        //DebugMenu.SetActive(false);
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
            imagePanelManager.ResetToFirstImage();
        }
    }

    public void ActivateGameElementsPanel()
    {
        gameElementsPanel.SetActive(true);
    }

    //Para la tienda de plantas
    public void ShowTiendaPlantas()
    {
        if (gameElementsPanel.activeSelf)
        {
            TiendaPlantas.SetActive(true);
            gameElementsPanel.SetActive(false);
            
        }
    }






}

