using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    public GameObject canvasUIManager;
    public GameObject canvasQR;

    public void ActivarCanvasQR()
    {
        canvasUIManager.SetActive(false);
        canvasQR.SetActive(true);
    }

    public void RegresarAUIManager()
    {
        canvasQR.SetActive(false);
        canvasUIManager.SetActive(true);
    }
}
