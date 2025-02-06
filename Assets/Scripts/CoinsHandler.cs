using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinsHandler : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI coinsText; // Texto donde se muestran las monedas

    private DatosJugador datosJugador;

    private void Start()
    {
        if (GestorDeDatos.Instance == null)
        {
            Debug.LogWarning("GestorDeDatos no se encontró en la escena.");
            return;
        }

        if (coinsText == null)
        {
            Debug.LogError("coinsText no está asignado en el Inspector.");
            return;
        }

        RefreshCoinsFromData();
    }

    // Recarga los datos en memoria y actualiza la UI
    public void RefreshCoinsFromData()
    {
        if (GestorDeDatos.Instance == null)
            return;

        datosJugador = GestorDeDatos.Instance.GetDatosJugador();
        UpdateCoinsUI();
    }

    public void UpdateCoinsUI()
    {
        if (datosJugador == null)
        {
            Debug.LogWarning("No hay datos de jugador cargados. Monedas no actualizadas.");
            return;
        }

        coinsText.text = datosJugador.monedas.ToString();
    }
}
