using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthAndExpSprites : MonoBehaviour
{
    public static HealthAndExpSprites Instance { get; private set; }
    private PlantLevelManager plantLevelManager;

    [Header("Barras e imágenes")]
    public Image healthBarImage;
    public Image expBarImage;
    public Sprite[] healthBarSprites;
    public Sprite[] expBarSprites;

    private float maxHealth = 100f;
    private float minHealth = 10f;
    private float currentHealth;

    private DateTime lastUpdatedDate;
    public bool receivedWater = false;
    public bool receivedNutrients = false;
    public bool receivedSparkles = false;
    public bool completedDailyTasks = false;

    [Header("Iconos de necesidades")]
    public Image waterIcon;
    public Image nutrientsIcon;
    public Image sparklesIcon;

    // No es necesario mantener una variable local "datosJugador"
    // Se accederá siempre a GestorDeDatos.Instance.GetDatosJugador()

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        plantLevelManager = PlantLevelManager.Instance;
        // Usamos la única instancia en memoria para obtener los datos
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        currentHealth = datos.currentHealth;
        UpdateHealthBar();

        // Convertir la fecha almacenada
        DateTime.TryParse(datos.lastUpdatedDate, out lastUpdatedDate);

        // Invoca la actualización de stats de forma periódica
        InvokeRepeating("UpdateStats", 1f, 1f);
        LoadPlantNeeds();
        UpdateExpBar();
        HandleEnergyAndHealth();

        // (Opcional) Suscribirse al evento OnDataChanged para actualizar la UI cuando se modifiquen los datos
        GestorDeDatos.Instance.OnDataChanged += OnDatosActualizados;
    }

    void OnDatosActualizados()
    {
        // Cada vez que se actualicen los datos, actualizamos la barra y otros elementos de la UI
        UpdateHealthBar();
        UpdateExpBar();
    }

    void UpdateStats()
    {
        Debug.Log("Actualizando stats");
        HandleEnergyAndHealth();
    }

    void CheckDailyTasks()
    {
        if (receivedWater && receivedNutrients && receivedSparkles)
        {
            completedDailyTasks = true;
            var datos = GestorDeDatos.Instance.GetDatosJugador();
            datos.completedDailyTasks = true;
            if (!datos.levelUpDoneToday)
            {
                datos.levelUpDoneToday = true;
                plantLevelManager.IncreaseLevel();
                UpdateExpBar();
                Debug.Log("🌟 Nivel aumentado. No volverá a subir hoy.");
            }
            GestorDeDatos.Instance.GuardarDatos();
        }
        Debug.Log(completedDailyTasks ? "✔ Tareas diarias completadas." : "❌ Tareas diarias incompletas.");
    }

    public void HandleEnergyAndHealth()
{
    // Se obtiene la copia en memoria de los datos del jugador.
    var datos = GestorDeDatos.Instance.GetDatosJugador();
    DateTime fechaHoy = DateTime.Now.Date;
    DateTime fechaUltimaActualizacion;

    if (DateTime.TryParse(datos.lastUpdatedDate, out fechaUltimaActualizacion))
    {
        fechaUltimaActualizacion = fechaUltimaActualizacion.Date;
        Debug.Log($"FECHA HOY: {fechaHoy}");
        Debug.Log($"FECHA ÚLTIMA ACTUALIZACIÓN: {fechaUltimaActualizacion}");

        // Si ha pasado al menos un día
        if (fechaHoy > fechaUltimaActualizacion)
        {
            Debug.Log("Ha pasado un día. Reiniciando tareas diarias...");

            if (!datos.completedDailyTasks)
            {
                // Si las tareas diarias NO se completaron, la planta muere.
                Debug.Log("No se completaron las tareas diarias; la planta muere.");
                DieAndReset(); // Este método debe establecer currentHealth a 0.
            }
            else
            {
                // Si se completaron, la planta se reinicia (por ejemplo, su salud se reinicia a minHealth)
                Debug.Log("Tareas diarias completadas; la planta se mantiene viva.");
                ResetHealth(); // Este método asigna currentHealth a minHealth.
            }

            // Reiniciar la fecha y los flags diarios para el nuevo día:
            datos.lastUpdatedDate = DateTime.Now.ToString("yyyy-MM-dd");
            datos.completedDailyTasks = false;
            datos.levelUpDoneToday = false;
            datos.receivedWater = false;
            datos.receivedNutrients = false;
            datos.receivedSparkles = false;

            GestorDeDatos.Instance.GuardarDatos();
            ResetNeedIcons(); // Se actualizan los iconos (por ejemplo, volviéndolos opacos)
            
            ButtonHandler buttonHandler = FindObjectOfType<ButtonHandler>();
            if (buttonHandler != null)
            {
                buttonHandler.UpdateUI();
            }
        }
        else
        {
            Debug.Log("No ha pasado un día.");
        }
    }
    else
    {
        Debug.LogError("Error al parsear lastUpdatedDate");
        datos.lastUpdatedDate = DateTime.Now.ToString("yyyy-MM-dd");
        GestorDeDatos.Instance.GuardarDatos();
    }
}


    void ResetNeedIcons()
    {
        waterIcon.color = new Color(1, 1, 1, 0.5f);
        nutrientsIcon.color = new Color(1, 1, 1, 0.5f);
        sparklesIcon.color = new Color(1, 1, 1, 0.5f);
        receivedWater = false;
        receivedNutrients = false;
        receivedSparkles = false;
    }

    void ResetHealth()
    {
        currentHealth = minHealth;
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        datos.currentHealth = currentHealth;
        GestorDeDatos.Instance.GuardarDatos();
        UpdateHealthBar();
        Debug.Log("La planta ha reiniciado su vida al 10%.");
    }

    void DieAndReset()
    {
        currentHealth = 0;
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        datos.currentHealth = currentHealth;
        GestorDeDatos.Instance.GuardarDatos();
        Debug.Log("La planta ha muerto.");
    }

    public void RevivePlant()
    {
        currentHealth = maxHealth;
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        datos.currentHealth = currentHealth;
        GestorDeDatos.Instance.GuardarDatos();
        Debug.Log("La planta ha sido revivida.");
    }

    public void AddHealth(float healthAmount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + healthAmount, maxHealth);
            var datos = GestorDeDatos.Instance.GetDatosJugador();
            datos.currentHealth = currentHealth;
            GestorDeDatos.Instance.GuardarDatos();
            UpdateHealthBar();
            Debug.Log($"Salud aumentada: {currentHealth}%");
        }
    }

    void UpdateHealthBar()
    {
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        currentHealth = datos.currentHealth;
        int healthIndex = (currentHealth == 100) ? 10 :
                          (currentHealth >= 70) ? 7 :
                          (currentHealth >= 40) ? 4 :
                          (currentHealth >= 10) ? 1 : 0;
        healthBarImage.sprite = healthBarSprites[healthIndex];
        Debug.Log($"Barra de salud actualizada: {currentHealth}%");
    }

    void UpdateExpBar()
    {
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        int expIndex = Mathf.Clamp(datos.nivel, 0, expBarSprites.Length - 1);
        expBarImage.sprite = expBarSprites[expIndex];
        Debug.Log($"Nivel actual: {PlantLevelManager.Instance.GetPlantLevel()}");
    }

    public void ReceiveWater()
    {
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        receivedWater = true;
        datos.receivedWater = true;
        waterIcon.color = Color.white;
        GestorDeDatos.Instance.GuardarDatos();
        AddHealth(30);
        CheckDailyTasks();
        Debug.Log("Recibió agua.");
    }

    public void ReceiveNutrients()
    {
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        receivedNutrients = true;
        datos.receivedNutrients = true;
        // Consumir 1 de fertilizante
        datos.cantidadFertilizante--;
        GestorDeDatos.Instance.GuardarDatos();
        nutrientsIcon.color = Color.white;
        AddHealth(30);
        CheckDailyTasks();
        Debug.Log("Recibió nutrientes.");
    }

    public void ReceiveSparkles()
    {
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        receivedSparkles = true;
        datos.receivedSparkles = true;
        // Consumir 1 de polen
        datos.cantidadPolen--;
        GestorDeDatos.Instance.GuardarDatos();
        sparklesIcon.color = Color.white;
        AddHealth(30);
        CheckDailyTasks();
        Debug.Log("Recibió brillitos.");
    }

    private void LoadPlantNeeds()
    {
        var datos = GestorDeDatos.Instance.GetDatosJugador();
        receivedWater = datos.receivedWater;
        waterIcon.color = receivedWater ? Color.white : new Color(1, 1, 1, 0.5f);
        receivedNutrients = datos.receivedNutrients;
        nutrientsIcon.color = receivedNutrients ? Color.white : new Color(1, 1, 1, 0.5f);
        receivedSparkles = datos.receivedSparkles;
        sparklesIcon.color = receivedSparkles ? Color.white : new Color(1, 1, 1, 0.5f);
    }
    public float GetCurrentHealth()
{
    return GestorDeDatos.Instance.GetDatosJugador().currentHealth;
}

}
