using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePanelManager : MonoBehaviour
{
    public GameObject image1;
    public GameObject image2;
    public GameObject image3;
    public GameObject gameElements;

    void Start()
    {
        ShowImage(1); // Mostrar la primera imagen al inicio
    }

    public void ShowImage(int imageNumber)
    {
        image1.SetActive(imageNumber == 1);
        image2.SetActive(imageNumber == 2);
        image3.SetActive(imageNumber == 3);
    }

    public void OnNextButton1Clicked()
    {
        ShowImage(2);
    }

    public void OnNextButton2Clicked()
    {
        ShowImage(3);
    }

    public void OnFinishButtonClicked()
    {
        gameElements.SetActive(true);
        gameObject.SetActive(false); // Desactivar el panel de imágenes
    }

    public void ResetToFirstImage()
    {
        ShowImage(1);
    }
}

