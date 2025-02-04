using UnityEngine;
using System.Collections.Generic;

public class CenterObjectInCanvas : MonoBehaviour
{
    public static CenterObjectInCanvas Instance { get; private set; } // Singleton
    public List<GameObject> alivePlantPrefabs; // 3 modelos vivos
    public List<GameObject> deadPlantPrefabs;  // 3 modelos muertos
    private GameObject currentPlant;

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
        Debug.Log($"Indice del modelo: {modelIndex}");

        // Determinar si la planta está viva o muerta
        GameObject plantPrefab = HealthAndExpSprites.Instance.GetCurrentHealth() > 0 ? 
                                 alivePlantPrefabs[modelIndex] : 
                                 deadPlantPrefabs[modelIndex];

        if (plantPrefab != null)
        {
            if (currentPlant != null)
            {
                Destroy(currentPlant);
            }

            currentPlant = Instantiate(plantPrefab);
            DataHandler.Instance.plantModel = currentPlant;
            Debug.Log($"🌱 Modelo de planta asignado: {currentPlant.name}");
        }
        else
        {
            Debug.LogError("❌ No se pudo asignar un modelo de planta.");
        }
    }

    public void SetDeadPlantModel()
    {
        if (currentPlant != null)
        {
            Destroy(currentPlant);
        }

        int playerLevel = PlantLevelManager.Instance.GetPlantLevel();
        int modelIndex = Mathf.Clamp(playerLevel - 1, 0, deadPlantPrefabs.Count - 1);
        GameObject deadPlant = deadPlantPrefabs[modelIndex];

        if (deadPlant != null)
        {
            currentPlant = Instantiate(deadPlant);
            DataHandler.Instance.plantModel = currentPlant;
            currentPlant.SetActive(false);
            Debug.Log($"☠️ Modelo de planta muerta asignado: {currentPlant.name}");
        }
    }
    public void VerifyCurrentModel()
    {
        if (currentPlant == null)
        {
            Debug.LogWarning("⚠️ No hay modelo de planta actual. Necesita asignar uno.");
            AssignPlantModel(); // Solo asigna el modelo si no hay uno
            return;
        }

        int playerLevel = PlantLevelManager.Instance.GetPlantLevel();
        int modelIndex = Mathf.Clamp((playerLevel - 1) / 3, 0, alivePlantPrefabs.Count - 1);

        // Verifica si el modelo actual corresponde al nivel del jugador y su estado (vivo o muerto)
        GameObject expectedPlantPrefab = HealthAndExpSprites.Instance.GetCurrentHealth() > 0 ? 
                                        alivePlantPrefabs[modelIndex] : 
                                        deadPlantPrefabs[modelIndex];

        if (currentPlant.name != expectedPlantPrefab.name + "(Clone)")
        {
            Debug.Log($"🔄 El modelo actual no es el correcto. Se espera: {expectedPlantPrefab.name}");
            AssignPlantModel(); // Reasigna solo si el modelo no coincide
        }
        else
        {
            Debug.Log("✅ El modelo actual es el adecuado.");
        }
    }

}
