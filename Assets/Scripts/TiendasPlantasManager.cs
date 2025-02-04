using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TiendasPlantasManager : MonoBehaviour
{
    public GameObject gameElements;
    private const string COINS_KEY = "Coins";
    private int monedas;
    // Start is called before the first frame update
    void Start()
    {
        monedas = PlayerPrefs.GetInt(COINS_KEY, 1000);

    }

    
    public void OnFinishButtonClicked()
    {
        gameElements.SetActive(true);
        gameObject.SetActive(false); // Desactivar el panel 
    }
    
}
