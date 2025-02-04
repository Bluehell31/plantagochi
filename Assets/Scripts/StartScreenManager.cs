using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject welcomePanel;
    public GameObject gameElements;

    void Start()
    {
        bool gameStarted = PlayerPrefs.GetInt("GameStarted", 0) == 1;

        if (gameStarted)
        {
            // Si el juego ya ha comenzado, ir directamente al panel de juego
            startPanel.SetActive(false);
            welcomePanel.SetActive(false);
            gameElements.SetActive(true);
        }
        else
        {
            // Si es la primera vez que se ejecuta el juego, mostrar el panel de inicio
            startPanel.SetActive(true);
            welcomePanel.SetActive(false);
            gameElements.SetActive(false);
        }
    }

    public void OnStartPanelButtonPressed()
    {
        // Cuando se presiona el botón de inicio en el panel de inicio, mostrar el panel de bienvenida
        startPanel.SetActive(false);
        welcomePanel.SetActive(true);
        gameElements.SetActive(false);
    }

    public void OnGameStarted()
    {
        // Esta función se llama cuando se inicia el juego desde el panel de bienvenida
        PlayerPrefs.SetInt("GameStarted", 1);
        PlayerPrefs.Save();

        // Ir directamente al panel de juego
        startPanel.SetActive(false);
        welcomePanel.SetActive(false);
        gameElements.SetActive(true);
    }
}


