using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotonRegresarTienda : MonoBehaviour
{
    public GameObject tiendaPlantas;
    public GameObject gameElementsPanel;

    public void RegresarAGameElements()
    {
        tiendaPlantas.SetActive(false);
        gameElementsPanel.SetActive(true);
    }
}

