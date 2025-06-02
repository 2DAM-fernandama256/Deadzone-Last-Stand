using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{

    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {

            PlayFabSettings.staticSettings.TitleId = "42";
        }
        var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }



    //para ver si se ha conectado correctamente com playfab
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
    }

    //para ver si no se ha conectado con playfab
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    public void jugar()
    {
        //para cargar la siguiente escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void volver()
    {
        //una escena menos
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void salir()
    {
        //para cerrar la aplicacion
        Application.Quit();
        Debug.Log("Salir");
    }
}
