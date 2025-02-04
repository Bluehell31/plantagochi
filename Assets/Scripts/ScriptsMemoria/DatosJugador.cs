using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DatosJugador
{
    // Datos del jugador
    public string nombreJugador;
    public int nivel;
    public float experiencia;

    // Datos de la planta
    public float currentHealth;
    public bool receivedWater;
    public bool receivedNutrients;
    public bool receivedSparkles;
    
    // Datos de progreso
    public string lastUpdatedDate;
    public bool completedDailyTasks;   
    public bool levelUpDoneToday;      

    // Nueva informacion de posicion y rotacion de la planta
    public Vector3 plantPosition;
    public Quaternion plantRotation;


    public int cantidadPolen;
    public int cantidadFertilizante;
    public int monedas;
}

