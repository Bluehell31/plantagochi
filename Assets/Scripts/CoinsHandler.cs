using UnityEngine;
using TMPro;

public class CoinsHandler : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI coinsText; // Texto donde se muestran las monedas

    [Header("Referencias a scripts")]
    private GestorDeDatos gestorDeDatos;    
    private DatosJugador datosJugador;       

    // ---------------------------------------------------------------------
    // Inicialización
    // ---------------------------------------------------------------------
    private void Start()
    {
        // Buscamos el GestorDeDatos (si está en la escena secundaria)
        gestorDeDatos = FindObjectOfType<GestorDeDatos>();
        if (gestorDeDatos == null)
        {
            Debug.LogWarning("❌ No se encontró GestorDeDatos en la escena actual. " +
                             "Si no necesitas recargar datos en esta escena, ignora este mensaje.");
        }

        // Revisar si el texto de monedas está asignado
        if (coinsText == null)
        {
            Debug.LogError("❌ coinsText no está asignado en el Inspector.");
            return;
        }

        // Si queremos mostrar de inmediato las monedas actuales al entrar a esta escena,
        // (re)cargamos datos y actualizamos la UI.
        RefreshCoinsFromData();
    }

    // ---------------------------------------------------------------------
    // Método para recargar los datos desde JSON y actualizar el texto de monedas
    // ---------------------------------------------------------------------
    public void RefreshCoinsFromData()
    {
        if (gestorDeDatos == null) return;  // Puede que no exista en esta escena

        // Cargamos los datos del JSON
        datosJugador = gestorDeDatos.CargarDatos();

        // Actualizamos el texto de las monedas (si lo deseamos)
        UpdateCoinsUI();
    }

    // ---------------------------------------------------------------------
    // Actualiza el texto de monedas en la UI
    // ---------------------------------------------------------------------
    public void UpdateCoinsUI()
    {
        // Si por alguna razón datosJugador es nulo, no hacemos nada
        if (datosJugador == null)
        {
            Debug.LogWarning("⚠ No hay datos de jugador cargados. Monedas no actualizadas.");
            return;
        }

        coinsText.text = datosJugador.monedas.ToString();
    }
}
