using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventPanelUserInRange;
    [SerializeField] private GameObject eventPanelUserOutOfRange;
    [SerializeField] private GameObject eventPanelAlreadyClaimed;
    [SerializeField] private TextMeshProUGUI pointsText;

    private bool isUIPanelActive;

    // Clave para guardar/cargar el historial de uso
    private const string POINTER_HISTORY_KEY = "PointerHistory";

    // Lista de fechas/horas en las que el usuario usó cualquier hotspot
    private List<DateTime> usageHistory;

    void Start()
    {
        if (GestorDeDatos.Instance == null)
        {
            Debug.LogError("GestorDeDatos no se encontró en la escena.");
            return;
        }

        // Se usa la copia en memoria
        // DatosJugador datosJugador = GestorDeDatos.Instance.GetDatosJugador(); // No es necesario guardarlo en variable local

        // Cargar historial de uso desde PlayerPrefs
        usageHistory = LoadUsageHistory();

        // Actualizar el texto de monedas en pantalla
        UpdatePointsText();
    }

    private void UpdatePointsText()
    {
        if (pointsText != null)
        {
            int monedas = GestorDeDatos.Instance.GetDatosJugador().monedas;
            pointsText.text = monedas.ToString() + " PUNTOS";
        }
    }

    // Muestra el panel de inicio de evento si se cumplen las condiciones
    public void DisplayStartEventPanel()
    {
        if (!isUIPanelActive && CanCollectReward())
        {
            eventPanelUserInRange.SetActive(true);
            isUIPanelActive = true;
        }
        else
        {
            DisplayUserInRangePanel();
        }
    }

    public void OnCollectRewardButtonClicked()
    {
        if (!CanCollectReward())
        {
            Debug.Log("No puedes recolectar más recompensas hoy o esta semana.");
            CloseButtonClick();
            DisplayAlreadyClaimed();
            return;
        }

        CollectReward();
    }

    public void CollectReward()
    {
        AddPoints();
        SaveUsage();
        CloseButtonClick();
    }

    private void AddPoints()
    {
        int pointsToAdd = 200;
        // Se trabaja directamente con la copia en memoria
        DatosJugador datos = GestorDeDatos.Instance.GetDatosJugador();
        datos.monedas += pointsToAdd;
        GestorDeDatos.Instance.GuardarDatos();
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

    // Se permite 1 uso por día y máximo 2 usos en los últimos 7 días
    private bool CanCollectReward()
    {
        DateTime now = DateTime.Now;
        int usesLast7Days = 0;
        foreach (var date in usageHistory)
        {
            if ((now - date).TotalDays <= 7)
            {
                usesLast7Days++;
            }
        }

        if (usesLast7Days >= 2)
        {
            return false;
        }

        foreach (var date in usageHistory)
        {
            if (date.Date == now.Date)
            {
                return false;
            }
        }

        return true;
    }

    // Guarda el historial de uso en PlayerPrefs
    private void SaveUsage()
    {
        usageHistory.Add(DateTime.Now);
        PlayerPrefs.SetString(POINTER_HISTORY_KEY, SerializeUsageHistory(usageHistory));
        PlayerPrefs.Save();
    }

    private List<DateTime> LoadUsageHistory()
    {
        string data = PlayerPrefs.GetString(POINTER_HISTORY_KEY, "");
        return DeserializeUsageHistory(data);
    }

    private string SerializeUsageHistory(List<DateTime> history)
    {
        List<string> dateStrings = new List<string>();
        foreach (var date in history)
        {
            dateStrings.Add(date.ToString());
        }
        return string.Join(";", dateStrings);
    }

    private List<DateTime> DeserializeUsageHistory(string data)
    {
        List<DateTime> history = new List<DateTime>();
        if (string.IsNullOrEmpty(data))
        {
            return history;
        }
        string[] entries = data.Split(';');
        foreach (string entry in entries)
        {
            if (DateTime.TryParse(entry, out DateTime parsedDate))
            {
                history.Add(parsedDate);
            }
        }
        return history;
    }
}
