using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuntosInicialPanelManager : MonoBehaviour
{
    public GameObject puntosInicialPanel;
    public GameObject gameElementsPanel;
    public Button puntosInicialButton;
    public StartScreenManager startScreenManager; // Referencia al StartScreenManager

    private void Start()
    {
        puntosInicialButton.onClick.AddListener(OnPuntosInicialButtonClicked);
    }

    public void OnPuntosInicialButtonClicked()
    {
        puntosInicialPanel.SetActive(false);
        gameElementsPanel.SetActive(true);

        // Actualizar el indicador de que el juego ha sido iniciado
        startScreenManager.OnGameStarted();
    }
}
