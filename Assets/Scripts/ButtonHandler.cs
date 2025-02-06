using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ButtonHandler : MonoBehaviour
{
    [Header("Referencias a scripts")]
    public HealthAndExpSprites healthAndExpSprites;
    public CoinsHandler coinsTiendaUI; 

    [Header("Botones principales")]
    public Button waterButton;
    public Button polinizeButton;
    public Button fertilizeButton;

    [Header("Botones de compra")]
    public Button buyPolinizeButton;
    public Button buyFertilizeButton;

    [Header("Iconos / Efectos visuales")]
    public Image water2d;
    public Image polinizacion2d;
    public Image fertilizacion2d;

    [Header("Textos de UI")]
    public Text CantidadPolen;
    public Text CantidadFertilizante;
    public TextMeshProUGUI MonedasTxt;
    public InputField nombreInput;

    [Header("Paneles de feedback")]
    [SerializeField] private GameObject CompletedTaskPanel;
    [SerializeField] private GameObject LevelUpPanel;

    private void Start()
    {
        if (GestorDeDatos.Instance == null)
        {
            Debug.LogError("❌ No se encontró GestorDeDatos en la escena.");
            return;
        }

        var datos = GestorDeDatos.Instance.GetDatosJugador();
        nombreInput.text = datos.nombreJugador;

        if (healthAndExpSprites == null)
        {
            healthAndExpSprites = FindObjectOfType<HealthAndExpSprites>();
        }

        UpdateUI();

        // (Opcional) Suscribirse a cambios de datos para actualizar la UI
        GestorDeDatos.Instance.OnDataChanged += UpdateUI;
    }

    public void UpdateUI()
    {
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        CantidadPolen.text = datos.cantidadPolen.ToString();
        CantidadFertilizante.text = datos.cantidadFertilizante.ToString();
        MonedasTxt.text = datos.monedas.ToString();

        bool waterUsed = healthAndExpSprites.receivedWater;
        bool nutrientsUsed = healthAndExpSprites.receivedNutrients;
        bool sparklesUsed = healthAndExpSprites.receivedSparkles;

        waterButton.interactable = !waterUsed;
        polinizeButton.interactable = !sparklesUsed && (datos.cantidadPolen > 0);
        fertilizeButton.interactable = !nutrientsUsed && (datos.cantidadFertilizante > 0);
    }

    public void OnWaterButtonClicked()
    {
        if (!healthAndExpSprites.receivedWater)
        {
            healthAndExpSprites.ReceiveWater();
            StartCoroutine(ActivateButtonForDuration(waterButton, water2d, 3f));
            StartCoroutine(DeactivateOtherButtonsForDuration(waterButton, 3f));
            UpdateUI();
            CheckAndActivateCompletedTasksPanel();
        }
        else
        {
            Debug.Log("Ya se ha recibido agua hoy. Inténtalo de nuevo mañana.");
        }
    }

    public void OnPolinizeButtonClicked()
    {
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        if (!healthAndExpSprites.receivedSparkles && datos.cantidadPolen > 0)
        {
            healthAndExpSprites.ReceiveSparkles();
            StartCoroutine(ActivateButtonForDuration(polinizeButton, polinizacion2d, 3f));
            StartCoroutine(DeactivateOtherButtonsForDuration(polinizeButton, 3f));
            UpdateUI();
            CheckAndActivateCompletedTasksPanel();
        }
        else
        {
            Debug.Log("Ya se ha polinizado hoy o no tienes polen.");
        }
    }

    public void OnFertilizeButtonClicked()
    {
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        if (!healthAndExpSprites.receivedNutrients && datos.cantidadFertilizante > 0)
        {
            healthAndExpSprites.ReceiveNutrients();
            StartCoroutine(ActivateButtonForDuration(fertilizeButton, fertilizacion2d, 3f));
            StartCoroutine(DeactivateOtherButtonsForDuration(fertilizeButton, 3f));
            UpdateUI();
            CheckAndActivateCompletedTasksPanel();
        }
        else
        {
            Debug.Log("Ya se ha recibido fertilizante hoy o no tienes fertilizante.");
        }
    }

    void CheckAndActivateCompletedTasksPanel()
    {
        if (healthAndExpSprites.receivedWater &&
            healthAndExpSprites.receivedNutrients &&
            healthAndExpSprites.receivedSparkles)
        {
            Debug.Log("✔ ¡Tareas diarias completadas!");
            CompletedTaskPanel.SetActive(true);
            UpdateUI();
        }
    }

    public void CloseCompletedTaskButton()
    {
        CompletedTaskPanel.SetActive(false);
    }

    public void OnBuyPolinizeButtonClicked()
    {
        if (SpendCoins(25))
        {
            var datos = GestorDeDatos.Instance.GetDatosJugador();
            datos.cantidadPolen += 3;
            GestorDeDatos.Instance.GuardarDatos();
            UpdateUI();
        }
    }

    public void OnBuyFertilizeButtonClicked()
    {
        if (SpendCoins(25))
        {
            var datos = GestorDeDatos.Instance.GetDatosJugador();
            datos.cantidadFertilizante += 3;
            GestorDeDatos.Instance.GuardarDatos();
            UpdateUI();
        }
    }

    public void AddCoins(int amount)
    {
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        datos.monedas += amount;
        GestorDeDatos.Instance.GuardarDatos();
        coinsTiendaUI.RefreshCoinsFromData();
        UpdateUI();
    }

    public bool SpendCoins(int amount)
    {
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        if (datos.monedas >= amount)
        {
            datos.monedas -= amount;
            GestorDeDatos.Instance.GuardarDatos();
            coinsTiendaUI.RefreshCoinsFromData();
            UpdateUI();
            return true;
        }
        return false;
    }

    public void GuardarNombre()
    {
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        datos.nombreJugador = nombreInput.text;
        GestorDeDatos.Instance.GuardarDatos();
        Debug.Log("Nombre guardado en JSON: " + datos.nombreJugador);
    }

    public void CloseLevelUpPanel()
    {
        if (LevelUpPanel != null)
        {
            LevelUpPanel.SetActive(false);
        }
    }

    // Corrutinas para efectos visuales
    private IEnumerator ActivateButtonForDuration(Button button, Image image, float duration)
    {
        button.interactable = false;
        image.enabled = true;
        yield return new WaitForSeconds(duration);
        image.enabled = false;
        button.interactable = true;
    }

    private IEnumerator DeactivateOtherButtonsForDuration(Button pressedButton, float duration)
    {
        Button[] buttons = { waterButton, polinizeButton, fertilizeButton };
        foreach (Button b in buttons)
        {
            if (b != pressedButton)
            {
                b.interactable = false;
            }
        }
        yield return new WaitForSeconds(duration);
        foreach (Button b in buttons)
        {
            if (b != pressedButton)
            {
                b.interactable = true;
            }
        }
    }
}
