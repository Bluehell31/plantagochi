using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsPanelManager : MonoBehaviour
{
    public GameObject gameElements; // Referencia al objeto que contiene los elementos de gameElements
    public Button goToGameElementsButton; // Referencia al bot�n que lleva a gameElements

    void Start()
    {
        // Desactivamos gameElements al iniciar el panel de coins
        gameElements.SetActive(false);
        // Asignamos la funci�n GoToGameElements al evento onClick del bot�n
        goToGameElementsButton.onClick.AddListener(GoToGameElements);
    }

    // M�todo para ir a gameElements desde el panel de coins
    public void GoToGameElements()
    {
        // Desactivamos el panel de coins
        gameObject.SetActive(false);
        // Activamos gameElements
        gameElements.SetActive(true);
    }
    
}

