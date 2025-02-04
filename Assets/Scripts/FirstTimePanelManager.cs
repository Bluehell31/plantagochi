using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstTimePanelManager : MonoBehaviour
{
    public GameObject coinsPanel;
    public Button closeButton;
    public GameObject gameElementsPanel;

    private const string FirstTimeKey = "FirstTime";

    void Start()
    {
        // Desactivar el panel de elementos del juego al inicio
        gameElementsPanel.SetActive(false);

        // Verificar si es la primera vez que se ejecuta el juego
        if (PlayerPrefs.GetInt(FirstTimeKey, 1) == 1)
        {
            // Mostrar el panel de monedas
            coinsPanel.SetActive(true);
            // Configurar el botón de cerrar
            closeButton.onClick.AddListener(ClosePanel);

            // Marcar que el panel ya se mostró una vez
            PlayerPrefs.SetInt(FirstTimeKey, 0);
            PlayerPrefs.Save();
        }
        else
        {
            // Si no es la primera vez, asegurar que el panel de monedas esté desactivado
            coinsPanel.SetActive(false);
            // Activar el panel de elementos del juego
            gameElementsPanel.SetActive(true);
        }
    }

    void ClosePanel()
    {
        // Cerrar el panel de monedas
        coinsPanel.SetActive(false);
        // Activar el panel de elementos del juego
        gameElementsPanel.SetActive(true);
    }
}
