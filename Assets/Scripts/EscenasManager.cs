using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscenasManager : MonoBehaviour
{
    void Start()
    {
        // Si necesitas hacer algo al iniciar, agr�galo aqu�.
    }

    public void IrAEscenaQR()
    {
        // Desactivar objetos de la escena principal
        DesactivarObjetosEscenaPrincipal();

        // Cambiar a la escena QR
        SceneManager.LoadScene("EscenaQR", LoadSceneMode.Single);
    }

    private void DesactivarObjetosEscenaPrincipal()
    {
        GameObject[] objetosEscenaPrincipal = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject objeto in objetosEscenaPrincipal)
        {
            objeto.SetActive(false);
        }
    }

    public void RegresarAEscenaPrincipal()
    {
        // Activar objetos de la escena principal
        ActivarObjetosEscenaPrincipal();

        // Cambiar a la escena principal
        SceneManager.LoadScene("EscenaPrincipalCrucial", LoadSceneMode.Single);
    }

    private void ActivarObjetosEscenaPrincipal()
    {
        GameObject[] objetosEscenaPrincipal = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject objeto in objetosEscenaPrincipal)
        {
            objeto.SetActive(true);
        }
    }
    public void IrAEscenaMap()
    {
        // Desactivar objetos de la escena principal
        DesactivarObjetosEscenaPrincipal();

        // Cambiar a la escena QR
        SceneManager.LoadScene("Location-basedGame", LoadSceneMode.Single);
    }


    private IEnumerator CargarEscenaAsync(string escenaNombre)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(escenaNombre, LoadSceneMode.Single);

        // Esperar hasta que la escena haya cargado
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

}


