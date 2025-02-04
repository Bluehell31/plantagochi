using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventPanelUserInRange;
    [SerializeField] private GameObject eventPanelUserOutOfRange;
    [SerializeField] private GameObject eventPanelAlreadyClaimed;

    [SerializeField] private TextMeshProUGUI pointsText;

    private GestorDeDatos gestorDeDatos;
    private DatosJugador datosJugador;


    private bool isUIPanelActive;

    // Clave para guardar/cargar el historial de uso
    private const string POINTER_HISTORY_KEY = "PointerHistory";

    // Lista de fechas/horas en las que el usuario usó cualquier hotspot
    private const string COINS_KEY = "Coins";
    private List<System.DateTime> usageHistory;

    private int monedas;

    void Start()
    {
        // Inicializa GestorDeDatos y carga los datos del jugador
        gestorDeDatos = FindObjectOfType<GestorDeDatos>();
        datosJugador = gestorDeDatos.CargarDatos();

        // Deserializa el historial de uso desde datosJugador
        usageHistory = LoadUsageHistory();

        // Actualiza el texto en pantalla de las monedas
        UpdatePointsText();
    }


    private void UpdatePointsText()
    {
        if (pointsText != null)
        {
            monedas = datosJugador.monedas;
            pointsText.text = monedas.ToString() + " PUNTOS";
        }
    }


    // -------------------------------
    //     LÓGICA DE LA UI
    // -------------------------------
    // Ejemplo: este método se llama cuando el usuario está cerca de un hotspot y se muestra el panel
    public void DisplayStartEventPanel()
    {
        // Si NO hay panel activo y se puede reclamar la recompensa...
        if (!isUIPanelActive && CanCollectReward())
        {
            eventPanelUserInRange.SetActive(true);
            isUIPanelActive = true;
        }
        else
        {
            // Si ya no se puede (por límite diario/semanal), también puedes mostrar un panel
            DisplayUserInRangePanel();
        }
    }
    public void OnCollectRewardButtonClicked()
    {
    // Antes de recolectar, verifica si cumple las reglas
    if (!CanCollectReward())
    {
        // Aquí puedes mostrar un mensaje de error o panel informando
        // que no puede recolectar más recompensas
        Debug.Log("No puedes recolectar más recompensas hoy o esta semana.");
        CloseButtonClick();
        DisplayAlreadyClaimed();
        return;
    }

    // Si cumple, recolecta
    CollectReward();
        }


    // Método llamado cuando el usuario confirma que quiere reclamar la recompensa
    public void CollectReward()
    {
        // Añadir puntos a fertilizante y polinización
        AddPoints();

        // Guardar registro de que hoy se usó un hotspot
        SaveUsage();

        // Cerrar el panel
        CloseButtonClick();
        
    }
    private void AddPoints()
    {
        int pointsToAdd = 100;
        monedas = datosJugador.monedas;

        monedas += pointsToAdd;

        datosJugador.monedas = monedas;

        // Guarda los datos actualizados en JSON
        gestorDeDatos.GuardarDatos(datosJugador);

        UpdatePointsText();
    }


    public void DisplayUserInRangePanel()
    {
        if (!isUIPanelActive)
        {
            eventPanelUserInRange.SetActive(true);
            isUIPanelActive = true;
        }
    }

    public void DisplayUserOutOfRangePanel()
    {
        if (!isUIPanelActive)
        {
            eventPanelUserOutOfRange.SetActive(true);
            isUIPanelActive = true;
        }
    }
    public void DisplayAlreadyClaimed()
    {
        if (!isUIPanelActive)
        {
            eventPanelAlreadyClaimed.SetActive(true);
            isUIPanelActive = true;
        }
    }

    public void CloseButtonClick()
    {
        eventPanelUserInRange.SetActive(false);
        eventPanelUserOutOfRange.SetActive(false);
        eventPanelAlreadyClaimed.SetActive(false);
        isUIPanelActive = false;
    }

    // -------------------------------
    //      LÓGICA DE RECOMPENSAS
    // -------------------------------

    // -------------------------------
    //      LÓGICA DE LÍMITES
    // -------------------------------
    // Aplica la regla:
    // * 1 uso por día
    // * Máx 2 usos en los últimos 7 días
    private bool CanCollectReward()
    {
        System.DateTime now = System.DateTime.Now;
        
        // 1) Verificar cuántas veces se ha usado en la última semana
        int usesLast7Days = 0;
        foreach (var date in usageHistory)
        {
            // Si la diferencia es menor o igual a 7 días, cuenta
            if ((now - date).TotalDays <= 7)
            {
                usesLast7Days++;
            }
        }

        // Si ya se usó 2 veces en la semana, no se puede
        if (usesLast7Days >= 2)
        {
            return false;
        }

        // 2) Verificar si ya se usó hoy
        foreach (var date in usageHistory)
        {
            if (date.Date == now.Date)
            {
                // Si hay un registro con la misma fecha, ya se usó hoy
                return false;
            }
        }

        // Si pasa ambas validaciones, se puede recolectar
        return true;
    }

    // -------------------------------
    //     GUARDADO Y CARGA DE USOS
    // -------------------------------
    private void SaveUsage()
    {
        usageHistory.Add(System.DateTime.Now);
        PlayerPrefs.SetString(POINTER_HISTORY_KEY, SerializeUsageHistory(usageHistory));
        PlayerPrefs.Save();
    }

    private List<System.DateTime> LoadUsageHistory()
    {
        string data = PlayerPrefs.GetString(POINTER_HISTORY_KEY, "");
        return DeserializeUsageHistory(data);
    }

    private string SerializeUsageHistory(List<System.DateTime> history)
    {
        List<string> dateStrings = new List<string>();
        foreach (var date in history)
        {
            dateStrings.Add(date.ToString());
        }
        return string.Join(";", dateStrings);
    }

    private List<System.DateTime> DeserializeUsageHistory(string data)
    {
        List<System.DateTime> history = new List<System.DateTime>();
        
        if (string.IsNullOrEmpty(data))
        {
            return history;
        }

        string[] entries = data.Split(';');
        foreach (string entry in entries)
        {
            if (System.DateTime.TryParse(entry, out System.DateTime parsedDate))
            {
                history.Add(parsedDate);
            }
        }

        return history;
    }
}