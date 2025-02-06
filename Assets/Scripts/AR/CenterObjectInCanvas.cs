using UnityEngine;
using System.Collections.Generic;

public class CenterObjectInCanvas : MonoBehaviour
{
    public static CenterObjectInCanvas Instance { get; private set; } // Singleton
    public List<GameObject> alivePlantPrefabs; // 3 modelos vivos
    public List<GameObject> deadPlantPrefabs;  // 3 modelos muertos

    void Start()
    {
        // Llama a VerifyCurrentModel() cada 5 segundos
        InvokeRepeating(nameof(VerifyCurrentModel), 0f, 5f);
    }

    public void AssignPlantModel()
    {
        int playerLevel = PlantLevelManager.Instance.GetPlantLevel();
        int modelIndex = Mathf.Clamp((playerLevel - 1) / 3, 0, alivePlantPrefabs.Count - 1);
        Debug.Log($"Nivel de la planta: {playerLevel}");
        Debug.Log($"Índice del modelo: {modelIndex}");

        float currentHealth = GestorDeDatos.Instance.GetDatosJugador().currentHealth;
        GameObject plantPrefab = currentHealth > 0 ? alivePlantPrefabs[modelIndex] : deadPlantPrefabs[modelIndex];

        if (plantPrefab != null)
        {
            // Si ya existe una instancia, destruirla
            if (DataHandler.Instance.currentPlantInstance != null)
            {
                Destroy(DataHandler.Instance.currentPlantInstance);
            }
            DataHandler.Instance.currentPlantInstance = Instantiate(plantPrefab);
            Debug.Log($"🌱 Modelo de planta asignado: {DataHandler.Instance.currentPlantInstance.name}");
        }
        else
        {
            Debug.LogError("❌ No se pudo asignar un modelo de planta.");
        }
    }

    public void SetDeadPlantModel()
    {
        if (DataHandler.Instance.currentPlantInstance != null)
        {
            Destroy(DataHandler.Instance.currentPlantInstance);
        }
        int playerLevel = PlantLevelManager.Instance.GetPlantLevel();
        int modelIndex = Mathf.Clamp(playerLevel - 1, 0, deadPlantPrefabs.Count - 1);
        GameObject deadPlant = deadPlantPrefabs[modelIndex];

        if (deadPlant != null)
        {
            DataHandler.Instance.currentPlantInstance = Instantiate(deadPlant);
            DataHandler.Instance.currentPlantInstance.SetActive(false);
            Debug.Log($"☠️ Modelo de planta muerta asignado: {DataHandler.Instance.currentPlantInstance.name}");
        }
    }

    public void VerifyCurrentModel()
    {
        if (DataHandler.Instance.currentPlantInstance == null)
        {
            Debug.LogWarning("⚠️ No hay modelo de planta actual. Se asignará uno nuevo.");
            AssignPlantModel();
            return;
        }
        int playerLevel = PlantLevelManager.Instance.GetPlantLevel();
        int modelIndex = Mathf.Clamp((playerLevel - 1) / 3, 0, alivePlantPrefabs.Count - 1);
        float currentHealth = GestorDeDatos.Instance.GetDatosJugador().currentHealth;
        GameObject expectedPlantPrefab = currentHealth > 0 ? alivePlantPrefabs[modelIndex] : deadPlantPrefabs[modelIndex];
        if (DataHandler.Instance.currentPlantInstance.name != expectedPlantPrefab.name + "(Clone)")
        {
            Debug.Log($"🔄 El modelo actual no es el correcto. Se espera: {expectedPlantPrefab.name}");
            AssignPlantModel();
        }
        else
        {
            Debug.Log("✅ El modelo actual es el adecuado.");
        }
    }
}
