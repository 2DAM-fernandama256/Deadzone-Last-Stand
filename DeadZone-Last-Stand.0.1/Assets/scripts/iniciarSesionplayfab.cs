
using PlayFab;                         
using PlayFab.ClientModels;           
using UnityEngine;                    
using UnityEngine.UI;                
using TMPro;                      

public class IniciarSesionPlayFab : MonoBehaviour
{
    
    [Header("UI References - Login")]
    public TMP_InputField emailInput;              
    public TMP_InputField passwordInput;          
    public TextMeshProUGUI loginStatusText;        
    public GameObject loginPanel;                 

    // Referencias UI para el panel de Registro
    [Header("UI References - Register")]
    public TMP_InputField registerEmailInput;      
    public TMP_InputField registerPasswordInput;   
    public TMP_InputField usernameInput;           
    public TextMeshProUGUI registerStatusText;     
    public GameObject registerPanel;               

    // Configuración general
    [Header("Configuración")]
    public float minimoTamanioPass = 6;            
    public GameObject loadingSpinner;             

    public GameObject gamePanel;                   

    // Almacena datos de sesión devueltos por PlayFab
    private string _playFabId;
    private string _sessionTicket;

    // Al iniciar la escena, se muestra el panel de login
    void Start()
    {
        VerPanelLogin();
    }

    #region Login
    // Se ejecuta cuando el usuario presiona el botón de "Iniciar sesión"
    public void botonloginClicked()
    {
        // Validamos los campos antes de hacer la petición
        if (!ValidartextosLogin())
            return;

        loadingSpinner.SetActive(true);  // Mostrar animación de carga
        SetInteractable(false);          // Desactivar campos de entrada

        // Crear la solicitud de login
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text.Trim(),
            Password = passwordInput.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        // Enviar solicitud de login a PlayFab
        PlayFabClientAPI.LoginWithEmailAddress(request, SucesoLogin, OnLoginFailure);
        loginStatusText.text = "Iniciando sesión...";
    }

    // Validación de campos de login
    private bool ValidartextosLogin()
    {
        if (string.IsNullOrWhiteSpace(emailInput.text))
        {
            loginStatusText.text = "Por favor ingresa tu email";
            return false;
        }

        if (string.IsNullOrWhiteSpace(passwordInput.text))
        {
            loginStatusText.text = "Por favor ingresa tu contraseña"; 
            return false;
        }

        if (!ValidarEmail(emailInput.text.Trim()))
        {
            loginStatusText.text = "El formato del email no es válido";
            return false;
        }

        if (passwordInput.text.Length < minimoTamanioPass)
        {
            loginStatusText.text = $"La contraseña debe tener al menos {minimoTamanioPass} caracteres";
            return false;
        }

        return true;
    }
    #endregion

    #region Registro
    // Se ejecuta cuando el usuario presiona "Registrar"
    public void BotonRegistrarClicked()
    {
        // Validar campos antes de enviar la solicitud
        if (!ValidarRegistroInputs())
            return;

        loadingSpinner.SetActive(true);
        SetInteractable(false); 

        // Crear solicitud de registro
        var request = new RegisterPlayFabUserRequest
        {
            Email = registerEmailInput.text.Trim(),
            Password = registerPasswordInput.text,
            Username = usernameInput.text.Trim(),
            RequireBothUsernameAndEmail = true
        };

        // Enviar solicitud de registro a PlayFab
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
        registerStatusText.text = "Registrando cuenta...";
    }

    // Validación de campos de registro 
    private bool ValidarRegistroInputs()
    {
        if (string.IsNullOrWhiteSpace(registerEmailInput.text))
        {
            registerStatusText.text = "Por favor ingresa un email";
            return false;
        }

        if (!ValidarEmail(registerEmailInput.text.Trim()))
        {
            registerStatusText.text = "El formato del email no es válido";
            return false;
        }

        if (string.IsNullOrWhiteSpace(usernameInput.text))
        {
            registerStatusText.text = "Por favor ingresa un nombre de usuario";
            return false;
        }

        if (usernameInput.text.Trim().Length < 3)
        {
            registerStatusText.text = "El nombre de usuario debe tener al menos 3 caracteres";
            return false;
        }

        if (string.IsNullOrWhiteSpace(registerPasswordInput.text))
        {
            registerStatusText.text = "Por favor ingresa una contraseña";
            return false;
        }

        if (registerPasswordInput.text.Length < minimoTamanioPass)
        {
            registerStatusText.text = $"La contraseña debe tener al menos {minimoTamanioPass} caracteres";
            return false;
        }

        return true;
    }

    // Muestra el panel de registro y limpia el contenido previo
    public void VerPanelRegistro() 
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        registerStatusText.text = "";
        limpiarInputsRegistro();
    }

    // Muestra el panel de login y limpia su contenido
    public void VerPanelLogin()
    {
        registerPanel.SetActive(false); 
        loginPanel.SetActive(true);
        loginStatusText.text = "";
        LimpiarInputsLogin();
    }

    // Limpia campos del login
    private void LimpiarInputsLogin()
    {
        emailInput.text = "";
        passwordInput.text = "";
    }

    // Limpia campos del registro 
    private void limpiarInputsRegistro()
    {
        registerEmailInput.text = "";
        usernameInput.text = "";
        registerPasswordInput.text = "";
    }
    #endregion

    #region Callbacks
    // Callback: éxito en login
    private void SucesoLogin(LoginResult result)
    {
        loadingSpinner.SetActive(false); 
        SetInteractable(true);

        // Guardamos datos de sesion
        _playFabId = result.PlayFabId;
        _sessionTicket = result.SessionTicket;

        // Guardamos localmente PlayerPrefs es como almacenamiento local
        PlayerPrefs.SetString("PLAYFAB_ID", _playFabId);
        PlayerPrefs.SetString("SESSION_TICKET", _sessionTicket);

        loginStatusText.text = "¡Bienvenido!";
        Debug.Log("Login exitoso");

        // Cambiamos al panel del juego
        loginPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    // Callback: fallo en login
    private void OnLoginFailure(PlayFabError error)
    {
        loadingSpinner.SetActive(false);
        SetInteractable(true);

        string errorMessage = "Error al iniciar sesión";

        // Interpretamos errores comunes de PlayFab
        switch (error.Error)
        {
            case PlayFabErrorCode.InvalidEmailOrPassword:
                errorMessage = "Email o contraseña incorrectos";
                break;
            case PlayFabErrorCode.AccountNotFound:
                errorMessage = "Cuenta no encontrada. ¿Quieres registrarte?";
                break;
            case PlayFabErrorCode.InvalidParams:
                errorMessage = $"Verifica tus datos: {error.ErrorMessage}";
                break;
        }

        loginStatusText.text = errorMessage;
        Debug.LogError(error.GenerateErrorReport());
    }

    // Callback: éxito en registro
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        loadingSpinner.SetActive(false);
        SetInteractable(true);

        registerStatusText.text = "¡Registro exitoso!";
        VerPanelLogin();                      // Redirige al login automáticamente
        loginStatusText.text = "Ahora puedes iniciar sesión";
    }

    // Callback: fallo en registro
    private void OnRegisterFailure(PlayFabError error)
    {
        loadingSpinner.SetActive(false);
        SetInteractable(true);

        string errorMessage = "Error al registrar";

        // Interpretamos errores comunes
        switch (error.Error)
        {
            case PlayFabErrorCode.EmailAddressNotAvailable:
                errorMessage = "El email ya está en uso";
                break;
            case PlayFabErrorCode.UsernameNotAvailable:
                errorMessage = "El nombre de usuario no está disponible";
                break;
            case PlayFabErrorCode.InvalidPassword:
                errorMessage = $"La contraseña debe tener al menos {minimoTamanioPass} caracteres";
                break;
        }

        registerStatusText.text = $"{errorMessage}: {error.ErrorMessage}";
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    #region Helpers
    // Validación básica de formato de email
    private bool ValidarEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    // Activa o desactiva los campos de entrada mientras carga
    private void SetInteractable(bool interactable)
    {
        emailInput.interactable = interactable;
        passwordInput.interactable = interactable;
        registerEmailInput.interactable = interactable;
        registerPasswordInput.interactable = interactable;
        usernameInput.interactable = interactable;
    }
    #endregion

    // Función para cerrar sesion limpia datos y vuelve al login
    public void Logout()
    {
        PlayerPrefs.DeleteKey("PLAYFAB_ID");
        PlayerPrefs.DeleteKey("SESSION_TICKET");
        _playFabId = null;
        _sessionTicket = null;

        VerPanelLogin();
        loginStatusText.text = "Sesión cerrada";
    }
}

