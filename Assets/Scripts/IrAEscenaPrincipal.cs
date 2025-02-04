using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class IrAEscenaPrincipal : MonoBehaviour
{
    // Start is called before the first frame update
    public void MainMenu()
    {
        // Desactivar objetos de la escena principal

        // Cambiar a la escena QR
        SceneManager.LoadScene("EscenaPrincipalCrucial", LoadSceneMode.Single);
    }

}
