using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthAndExpSprites : MonoBehaviour
{
    public static HealthAndExpSprites Instance { get; private set; }


    private GestorDeDatos gestorDeDatos;
    private DatosJugador datosJugador;


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
    public bool receivedWater = false;
    public bool receivedNutrients = false;
    public bool receivedSparkles = false;
    public bool completedDailyTasks = false;

    private DateTime lastUpdatedDate;

    public CenterObjectInCanvas centerObjectScript;

    // Referencias a los iconos de las necesidades
    public Image waterIcon;
    public Image nutrientsIcon;
    public Image sparklesIcon;

    public float GetCurrentHealth() => datosJugador.currentHealth;
    public float GetCurrentExp() => datosJugador.experiencia; 
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
        gestorDeDatos = FindObjectOfType<GestorDeDatos>();
        datosJugador = gestorDeDatos.CargarDatos();

        currentHealth = datosJugador.currentHealth;
        lastUpdatedDate = DateTime.Parse(datosJugador.lastUpdatedDate);

        LoadPlantNeeds();
        InvokeRepeating("UpdateStats", 1f, 1f);
    }


    void UpdateStats()
    {
        Debug.Log("Estoy Actualizando las stats");
        UpdateHealthBar();
        UpdateExpBar();
        HandleEnergyAndHealth();
        UpdateNeedIcons();
        CheckDailyTasks();
    }

    void CheckDailyTasks()
    {
        if(receivedWater && receivedNutrients && receivedSparkles)
        {
            completedDailyTasks = true;
            datosJugador.completedDailyTasks = completedDailyTasks;
        }
        Debug.Log(completedDailyTasks ? "✔ Tareas diarias completadas." : "❌ No se completaron todas las tareas diarias.");
    }

    public void HandleEnergyAndHealth()
    {
        // Verifica si ya subió de nivel hoy
        bool levelUpDoneToday = datosJugador.levelUpDoneToday;

        if (completedDailyTasks && !levelUpDoneToday)
        {
            PlantLevelManager.Instance.IncreaseLevel();
            levelUpDoneToday = true;
            datosJugador.levelUpDoneToday = levelUpDoneToday; // Marcamos que ya subió de nivel hoy
            gestorDeDatos.GuardarDatos(datosJugador);
            Debug.Log("🌟 Nivel aumentado. No volverá a subir hoy.");
        }

        // Si ha pasado un día, reiniciamos la salud o la planta muere
        if (DateTime.Now.Date > DateTime.Parse(datosJugador.lastUpdatedDate).Date)
        {
            if (completedDailyTasks)
            {
                ResetHealth();
            }
            else
            {
                DieAndReset();
            }

            // Actualizamos la fecha y reiniciamos valores diarios
            datosJugador.lastUpdatedDate = DateTime.Now.ToString("yyyy-MM-dd");
            completedDailyTasks = false;
            datosJugador.completedDailyTasks = completedDailyTasks;
            datosJugador.levelUpDoneToday = false; // Permite subir de nivel nuevamente
            gestorDeDatos.GuardarDatos(datosJugador);

            ResetNeedIcons(); // Vuelve los iconos a gris solo cuando cambia el día
            ButtonHandler buttonHandler = FindObjectOfType<ButtonHandler>();//actualiza la ui de los botones para que se reactiven pasando un dia
            if (buttonHandler != null)
            {
                buttonHandler.UpdateUI();
            }
        }
    }


    void DecreaseHealth(float amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0); // Asegura que no sea menos que 0
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
        currentHealth = minHealth;
        SavePlantState();
        Debug.Log("🔄 La planta ha reiniciado su vida al 10%.");
    }

    void DieAndReset()
    {
        currentHealth = 0;
        SavePlantState();
        Debug.Log("☠️ La planta ha muerto. Debe ser revivida.");
    }

    public void RevivePlant()
    {
        currentHealth = maxHealth;
        SavePlantState();
        Debug.Log("🟢 La planta ha sido revivida.");
    }

    public void AddHealth(float healthAmount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + healthAmount, maxHealth);
            SavePlantState();
            Debug.Log($"❤️ Salud aumentada: {currentHealth}%");
        }
    }

    void UpdateHealthBar()
    {
        int healthIndex = (currentHealth == 100) ? 10 :
                          (currentHealth >= 70) ? 7 :
                          (currentHealth >= 40) ? 4 :
                          (currentHealth >= 10) ? 1 : 0;

        healthBarImage.sprite = healthBarSprites[healthIndex];
        Debug.Log($"🩸 Barra de salud actualizada: {currentHealth}%");
    }

    void UpdateExpBar()
    {
        int expIndex = Mathf.Clamp(PlantLevelManager.Instance.GetPlantLevel(), 0, expBarSprites.Length - 1);
        expBarImage.sprite = expBarSprites[expIndex];
        Debug.Log($"🌱 Nivel actual: {PlantLevelManager.Instance.GetPlantLevel()}");
    }

    void UpdateNeedIcons()
    {
        waterIcon.color = receivedWater ? Color.white : new Color(1, 1, 1, 0.5f);
        nutrientsIcon.color = receivedNutrients ? Color.white : new Color(1, 1, 1, 0.5f);
        sparklesIcon.color = receivedSparkles ? Color.white : new Color(1, 1, 1, 0.5f);
    }

    public void ReceiveWater()
    {
        receivedWater = true;
        AddHealth(30);
        SavePlantState();
        Debug.Log("💧 Recibió agua.");
    }


    public void ReceiveNutrients()
    {
        receivedNutrients = true;
       
        AddHealth(30);
        SavePlantState();
        Debug.Log("🌱 Recibió nutrientes.");
    }

    public void ReceiveSparkles()
    {
        receivedSparkles = true;
       
        AddHealth(30);
        SavePlantState();
        Debug.Log("✨ Recibió brillitos.");
    }

    private void LoadPlantNeeds()
    {
        receivedWater = datosJugador.receivedWater;
        receivedNutrients = datosJugador.receivedNutrients;
        receivedSparkles = datosJugador.receivedSparkles;
    }

    private void SavePlantState()
    {
        datosJugador.currentHealth = currentHealth;
        datosJugador.receivedWater = receivedWater;
        datosJugador.receivedNutrients = receivedNutrients;
        datosJugador.receivedSparkles = receivedSparkles;
        //datosJugador.lastUpdatedDate = DateTime.Now.ToString();

        gestorDeDatos.GuardarDatos(datosJugador);
    }

}
