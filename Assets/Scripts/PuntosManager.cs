using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PuntosManager
{
    private static int _puntos = 0;

    public static int Puntos
    {
        get { return _puntos; }
        set { _puntos = value; }
    }

    // Métodos para aumentar o disminuir los puntos
    public static void AumentarPuntos(int cantidad)
    {
        _puntos += cantidad;
    }

    public static void DisminuirPuntos(int cantidad)
    {
        _puntos -= cantidad;
        if (_puntos < 0)
        {
            _puntos = 0; // Asegurarse de que los puntos nunca sean negativos
        }
    }
}
