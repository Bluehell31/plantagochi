using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonHandler : MonoBehaviour
{
    [Header("Referencias a scripts")]
    public HealthAndExpSprites healthAndExpSprites;
    public CoinsHandler coinsTiendaUI; 
    private GestorDeDatos gestorDeDatos;        // Nuevo: para cargar/guardar JSON
    private DatosJugador datosJugador;          // Nuevo: datos cargados del JSON

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

    
    // ---------------------------------------------------------------------
    // Inicialización
    // ---------------------------------------------------------------------
    private void Start()
    {
        // Localizamos el gestor de datos (o podría asignarse por inspector).
        gestorDeDatos = FindObjectOfType<GestorDeDatos>();
        if (gestorDeDatos == null)
        {
            Debug.LogError("❌ No se encontró GestorDeDatos en la escena.");
            return;
        }

        // Cargamos los datos desde el archivo JSON
        datosJugador = gestorDeDatos.CargarDatos();

        // Referencia a HealthAndExpSprites (por si no está asignado en el inspector)
        if (healthAndExpSprites == null)
        {
            healthAndExpSprites = FindObjectOfType<HealthAndExpSprites>();
        }

        // Configuramos el nombre en la UI
        nombreInput.text = datosJugador.nombreJugador;
        

        // Actualizamos la interfaz con los valores del JSON
        UpdateUI();
    }

    // ---------------------------------------------------------------------
    // Actualizar interfaz
    // ---------------------------------------------------------------------
    /// <summary>
    /// Actualiza la UI de polen, fertilizante y monedas,
    /// y se encarga de habilitar/deshabilitar los botones según el estado.
    /// </summary>
    public void UpdateUI()
    {
        datosJugador = gestorDeDatos.CargarDatos();
        // Datos actuales en JSON
        int cantidadPolen         = datosJugador.cantidadPolen;
        int cantidadFertilizante  = datosJugador.cantidadFertilizante;
        int monedas               = datosJugador.monedas;

        // Actualizar textos
        CantidadPolen.text        = cantidadPolen.ToString();
        CantidadFertilizante.text = cantidadFertilizante.ToString();
        MonedasTxt.text           = monedas.ToString();

        // Comprobamos si la planta está viva (salud > 0)
        //bool plantIsAlive = healthAndExpSprites.GetCurrentHealth() > 0;

        // Comprobamos si ya se usaron las tareas diarias
        bool waterUsed     = healthAndExpSprites.receivedWater;
        bool nutrientsUsed = healthAndExpSprites.receivedNutrients;
        bool sparklesUsed  = healthAndExpSprites.receivedSparkles;

        // Botón de agua
        waterButton.interactable =  !waterUsed;

        // Botón de polinizar
        polinizeButton.interactable =  !sparklesUsed
                                       && (cantidadPolen > 0);

        // Botón de fertilizar
        fertilizeButton.interactable = !nutrientsUsed
                                       && (cantidadFertilizante > 0);
    }

    // ---------------------------------------------------------------------
    // BOTONES: Tareas diarias (agua, polinizar, fertilizar)
    // ---------------------------------------------------------------------

    public void OnWaterButtonClicked()
    {
        if (!healthAndExpSprites.receivedWater)
        {
            healthAndExpSprites.ReceiveWater();

            // Efecto visual de 3s
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
        // Verificamos que no se haya polinizado y que haya polen disponible
        if (!healthAndExpSprites.receivedSparkles && datosJugador.cantidadPolen > 0)
        {
            healthAndExpSprites.ReceiveSparkles();
            
            // Consumimos 1 de polen
            

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
        // Verificamos que no se haya fertilizado y que haya fertilizante disponible
        if (!healthAndExpSprites.receivedNutrients && datosJugador.cantidadFertilizante > 0)
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

    /// <summary>
    /// Si las 3 tareas se completaron, muestra el panel de "Tareas Diarias Completadas".
    /// </summary>
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

    // ---------------------------------------------------------------------
    // BOTONES: Comprar polen / fertilizante
    // ---------------------------------------------------------------------
    public void OnBuyPolinizeButtonClicked()
    {
        if (SpendCoins(25))  // Comprueba si hay monedas suficientes
        {
            //datosJugador = gestorDeDatos.CargarDatos();
            datosJugador.cantidadPolen += 3;
            gestorDeDatos.GuardarDatos(datosJugador);
            UpdateUI();
        }
    }

    public void OnBuyFertilizeButtonClicked()
    {
        if (SpendCoins(25))  // Comprueba si hay monedas suficientes
        {
            //datosJugador = gestorDeDatos.CargarDatos();
            datosJugador.cantidadFertilizante += 3;
            gestorDeDatos.GuardarDatos(datosJugador);
            UpdateUI();
        }
    }

    // ---------------------------------------------------------------------
    // Manejo de monedas
    // ---------------------------------------------------------------------
    public void AddCoins(int amount)
    {
        datosJugador = gestorDeDatos.CargarDatos();
        datosJugador.monedas += amount;
        gestorDeDatos.GuardarDatos(datosJugador);
        coinsTiendaUI.RefreshCoinsFromData();
        UpdateUI();
    }

    public bool SpendCoins(int amount)
    {
        datosJugador = gestorDeDatos.CargarDatos();
        if (datosJugador.monedas >= amount)
        {
            datosJugador.monedas -= amount;
            gestorDeDatos.GuardarDatos(datosJugador);
            coinsTiendaUI.RefreshCoinsFromData();
            UpdateUI();
            return true;
        }
        return false;
    }

    // ---------------------------------------------------------------------
    // Guardar nombre del jugador en JSON
    // ---------------------------------------------------------------------
    public void GuardarNombre()
    {
        datosJugador.nombreJugador = nombreInput.text;
        gestorDeDatos.GuardarDatos(datosJugador);
        Debug.Log("Nombre guardado en JSON: " + datosJugador.nombreJugador);
    }

    // ---------------------------------------------------------------------
    // Ejemplo de "Scan" o registro de datos (ya no con PlayerPrefs)
    // ---------------------------------------------------------------------
    public void OnButtonScanClicked()
    {
        // Actualizamos algunos datos del JSON (por si deseas sincronizarlos)
        datosJugador.experiencia = 1;
        datosJugador.nivel       = healthAndExpSprites.GetPlayerLevel();
        datosJugador.currentHealth = healthAndExpSprites.GetCurrentHealth();

        // Guardamos
        gestorDeDatos.GuardarDatos(datosJugador);

        Debug.Log("Datos de la planta actualizados en JSON: " +
                  "\nExp: " + datosJugador.experiencia +
                  "\nNivel: " + datosJugador.nivel +
                  "\nSalud: " + datosJugador.currentHealth);
    }

    // ---------------------------------------------------------------------
    // Panel de subida de nivel
    // ---------------------------------------------------------------------
    public void CloseLevelUpPanel()
    {
        if (LevelUpPanel != null)
        {
            LevelUpPanel.SetActive(false);
        }
    }

    // ---------------------------------------------------------------------
    // Corrutinas para efectos de UI
    // ---------------------------------------------------------------------

    private IEnumerator ActivateButtonForDuration(Button button, Image image, float duration)
    {
        // Deshabilitar momentáneamente
        button.interactable = false;
        image.enabled = true;
        yield return new WaitForSeconds(duration);
        image.enabled = false;
        // Reactivar (si la lógica del día no lo bloqueó ya)
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
