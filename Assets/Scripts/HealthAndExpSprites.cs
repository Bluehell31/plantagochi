using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthAndExpSprites : MonoBehaviour
{
    public static HealthAndExpSprites Instance { get; private set; }


    private GestorDeDatos gestorDeDatos;
    private DatosJugador datosJugador;
    private PlantLevelManager plantLevelManager;


    // Barras
    public Image healthBarImage;  
    public Image expBarImage;  
    public Sprite[] healthBarSprites; 
    public Sprite[] expBarSprites;    

    // Variables para la salud
    private float maxHealth = 100f;
    private float minHealth = 10f;
    private float currentHealth;
    
    /* Variables de tiempo
    private float lastReceivedTime;
    private const float healthDecrementTime = 8f * 3600f;
    private const float reviveTimeLimit = 24f * 3600f;
*/
    // Variables de necesidades
    private DateTime lastUpdatedDate;
    public bool receivedWater = false;
    public bool receivedNutrients = false;
    public bool receivedSparkles = false;
    public bool completedDailyTasks = false;

    

    public CenterObjectInCanvas centerObjectScript;

    // Referencias a los iconos de las necesidades
    public Image waterIcon;
    public Image nutrientsIcon;
    public Image sparklesIcon;

    public float GetCurrentHealth() => datosJugador.currentHealth;
    //public float GetCurrentExp() => datosJugador.experiencia; 
    public int GetPlayerLevel() => datosJugador.nivel;


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

    public void Start()
    {
        Debug.Log("ESTOY EN START ");
        gestorDeDatos = GestorDeDatos.Instance;
        plantLevelManager = PlantLevelManager.Instance;
        
        datosJugador = gestorDeDatos.CargarDatos();

        currentHealth = datosJugador.currentHealth;
        
        UpdateHealthBar();
        
        lastUpdatedDate = DateTime.Parse(datosJugador.lastUpdatedDate);
        Debug.Log("VOY A INVOKEREPEATING ");
        InvokeRepeating("UpdateStats", 1f, 1f);

        LoadPlantNeeds();
        UpdateExpBar();

        Debug.Log("VOY A ENTRAR A HANDLEENERGYANDHEALTH ");
        HandleEnergyAndHealth();
        
        
       
    }


    void UpdateStats()
    {
        Debug.Log("Estoy Actualizando las stats");
        HandleEnergyAndHealth();
    }

    void CheckDailyTasks()
    {
        if(receivedWater && receivedNutrients && receivedSparkles)
        {
            completedDailyTasks = true;
            datosJugador.completedDailyTasks = completedDailyTasks;
            
            bool levelUpDoneToday = datosJugador.levelUpDoneToday;
            gestorDeDatos.GuardarDatos(datosJugador);

            if (!levelUpDoneToday)
            {
                levelUpDoneToday = true;
                datosJugador.levelUpDoneToday = levelUpDoneToday; // Marcamos que ya subió de nivel hoy
                plantLevelManager.IncreaseLevel();
                UpdateExpBar();
                Debug.Log("🌟 Nivel aumentado. No volverá a subir hoy.");
            }
            
        }
        Debug.Log(completedDailyTasks ? "✔ Tareas diarias completadas." : "❌ No se completaron todas las tareas diarias.");
    }

    public void HandleEnergyAndHealth()
    {
        datosJugador = gestorDeDatos.CargarDatos();

        // Convertir la fecha de hoy a solo la parte de la fecha (sin la hora)
        DateTime fechaHoy = DateTime.Now.Date;

        // Variable para almacenar la última fecha guardada
        DateTime fechaUltimaActualizacion;

        // Intentar convertir la fecha guardada en el JSON a un objeto DateTime
        if (DateTime.TryParse(datosJugador.lastUpdatedDate, out fechaUltimaActualizacion))
        {
            // Asegurar que solo tomamos la fecha, sin la hora
            fechaUltimaActualizacion = fechaUltimaActualizacion.Date;

            Debug.Log($"📅 FECHA HOY: {fechaHoy}");
            Debug.Log($"📅 FECHA ÚLTIMA ACTUALIZACIÓN: {fechaUltimaActualizacion}");

            // Comparar fechas sin la hora
            if (fechaHoy > fechaUltimaActualizacion)
            {
                Debug.Log("✔ Ha pasado al menos un día desde la última actualización.");

                if (completedDailyTasks)
                {
                    ResetHealth();
                }
                else
                {
                    DieAndReset();
                }

                // Actualizar la fecha en los datos del jugador
                datosJugador.lastUpdatedDate = DateTime.Now.ToString("yyyy-MM-dd"); // Guardar solo la fecha
                datosJugador.completedDailyTasks = false;
                datosJugador.levelUpDoneToday = false;
                gestorDeDatos.GuardarDatos(datosJugador);

                ResetNeedIcons();
                ButtonHandler buttonHandler = FindObjectOfType<ButtonHandler>();
                if (buttonHandler != null)
                {
                    buttonHandler.UpdateUI();
                }
            }
            else
            {
                Debug.Log("⏳ No ha pasado un día desde la última actualización.");
            }
        }
        else
        {
            Debug.LogError("❌ No se pudo convertir 'lastUpdatedDate' a DateTime. Se inicializa con la fecha actual.");
            datosJugador.lastUpdatedDate = DateTime.Now.ToString("yyyy-MM-dd");
            gestorDeDatos.GuardarDatos(datosJugador);
        }
    }




    void ResetNeedIcons()
    {
        // Restablecer los iconos de agua, nutrientes y brillitos a su estado inicial (opaco)
        waterIcon.color = new Color(1, 1, 1, 0.5f);
        nutrientsIcon.color = new Color(1, 1, 1, 0.5f);
        sparklesIcon.color = new Color(1, 1, 1, 0.5f);

        // También puedes restablecer las variables que controlan si se han recibido o no
        receivedWater = false;
        receivedNutrients = false;
        receivedSparkles = false;
    }

    void ResetHealth()
    {
        datosJugador = gestorDeDatos.CargarDatos();
        currentHealth = minHealth;
        datosJugador.currentHealth = currentHealth;
        gestorDeDatos.GuardarDatos(datosJugador);
        UpdateHealthBar();
        Debug.Log("🔄 La planta ha reiniciado su vida al 10%.");
    }

    void DieAndReset()
    {
        datosJugador = gestorDeDatos.CargarDatos();
        currentHealth = 0;
        datosJugador.currentHealth = currentHealth;
        gestorDeDatos.GuardarDatos(datosJugador);
        Debug.Log("☠️ La planta ha muerto. Debe ser revivida.");
    }

    public void RevivePlant()
    {
        datosJugador = gestorDeDatos.CargarDatos();
        currentHealth = maxHealth;
        datosJugador.currentHealth = currentHealth;
        gestorDeDatos.GuardarDatos(datosJugador);
        Debug.Log("🟢 La planta ha sido revivida.");
    }

    public void AddHealth(float healthAmount)
    {
        datosJugador = gestorDeDatos.CargarDatos();
        if (currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + healthAmount, maxHealth);

            datosJugador.currentHealth = currentHealth;
            gestorDeDatos.GuardarDatos(datosJugador);
            UpdateHealthBar();
            Debug.Log($"❤️ Salud aumentada: {currentHealth}%");
        }
    }

    void UpdateHealthBar()
    {
        datosJugador = gestorDeDatos.CargarDatos();
        currentHealth = datosJugador.currentHealth;
        Debug.Log("ESTOY EN EL UPDATE HEALTH BAR, Y EL VALOR DE CURRENHEALTH ES: " + currentHealth);
        int healthIndex = (currentHealth == 100) ? 10 :
                          (currentHealth >= 70) ? 7 :
                          (currentHealth >= 40) ? 4 :
                          (currentHealth >= 10) ? 1 : 0;

        healthBarImage.sprite = healthBarSprites[healthIndex];
        Debug.Log($"🩸 Barra de salud actualizada: {currentHealth}%");
    }

    void UpdateExpBar()
    {
        datosJugador = gestorDeDatos.CargarDatos();
        int expIndex = Mathf.Clamp(datosJugador.nivel, 0, expBarSprites.Length - 1);
        expBarImage.sprite = expBarSprites[expIndex];
        Debug.Log($"🌱 Nivel actual: {PlantLevelManager.Instance.GetPlantLevel()}");
    }

    public void ReceiveWater()
    {
        datosJugador = gestorDeDatos.CargarDatos();
        receivedWater = true;
        datosJugador.receivedWater = receivedWater;
        waterIcon.color = receivedWater ? Color.white : new Color(1, 1, 1, 0.5f);
        gestorDeDatos.GuardarDatos(datosJugador);
        AddHealth(30);
        CheckDailyTasks();
        Debug.Log("💧 Recibió agua.");
    }


    public void ReceiveNutrients()
    {
        datosJugador = gestorDeDatos.CargarDatos();
        receivedNutrients = true;
        Debug.Log("Voy a Actualizar el ARCHIVO JSON");
        datosJugador.receivedNutrients = true;
        Debug.Log("ENTRANDO AL ARCHIVO");
        // Consumimos 1 de fertilizante
        datosJugador.cantidadFertilizante--;
        gestorDeDatos.GuardarDatos(datosJugador);
        Debug.Log((datosJugador.receivedNutrients) + " ESTE ES EL VALOR DE RECIEVED NUTUENTS QUE ESTA EN EL ARCHIVO");

        nutrientsIcon.color = receivedNutrients ? Color.white : new Color(1, 1, 1, 0.5f);
        AddHealth(30);
        CheckDailyTasks();
        Debug.Log("🌱 Recibió nutrientes.");
    }

    public void ReceiveSparkles()
    {
        datosJugador = gestorDeDatos.CargarDatos();
        receivedSparkles = true;
        datosJugador.receivedSparkles = true;
        datosJugador.cantidadPolen--;
        gestorDeDatos.GuardarDatos(datosJugador);
        sparklesIcon.color = receivedSparkles ? Color.white : new Color(1, 1, 1, 0.5f);
        AddHealth(30);
        CheckDailyTasks();
        Debug.Log("✨ Recibió brillitos.");
    }

    private void LoadPlantNeeds()
    {
        // Cargar estado de agua
        receivedWater = datosJugador.receivedWater;
        waterIcon.color = receivedWater ? Color.white : new Color(1, 1, 1, 0.5f);
        Debug.Log($"💧 Agua recibida cargada: {receivedWater}");

        // Cargar estado de nutrientes
        receivedNutrients = datosJugador.receivedNutrients;
        nutrientsIcon.color = receivedNutrients ? Color.white : new Color(1, 1, 1, 0.5f);
        Debug.Log($"🌱 Nutrientes recibidos cargados: {receivedNutrients}");

        // Cargar estado de brillitos
        receivedSparkles = datosJugador.receivedSparkles;
        sparklesIcon.color = receivedSparkles ? Color.white : new Color(1, 1, 1, 0.5f);
        Debug.Log($"✨ Brillitos recibidos cargados: {receivedSparkles}");
    }


}
