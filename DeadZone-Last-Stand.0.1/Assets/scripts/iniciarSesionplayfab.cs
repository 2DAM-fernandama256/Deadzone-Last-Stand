using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;

public class IniciarSesionPlayFab : MonoBehaviour
{

    [Header("UI References - Login")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI loginStatusText;
    public GameObject loginPanel;

    [Header("UI References - Register")]
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField usernameInput;
    public TextMeshProUGUI registerStatusText;
    public GameObject registerPanel;

    [Header("Configuración")]
    public float minimoTamanioPass = 6;

    public GameObject gamePanel;

    private string _playFabId;
    private string _sessionTicket;

    private static IniciarSesionPlayFab instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (PlayerPrefs.HasKey("PLAYFAB_ID") && PlayerPrefs.HasKey("SESSION_TICKET"))
            {
                _playFabId = PlayerPrefs.GetString("PLAYFAB_ID");
                _sessionTicket = PlayerPrefs.GetString("SESSION_TICKET");
                loginPanel.SetActive(false);
                registerPanel.SetActive(false);
                gamePanel.SetActive(true);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (Inicio.desdeInicio)
        {
            Debug.Log("Iniciando desde el menú principal, mostrando panel de login");
            VerPanelLogin();
        }
        else
        {
            Debug.Log("Iniciando desde el juego, mostrando panel de juego directamente");
            loginPanel.SetActive(false);
            registerPanel.SetActive(false);
            gamePanel.SetActive(true);
        }
    }

    #region Login

    public void botonloginClicked()
    {
        if (!ValidartextosLogin())
            return;

        SetInteractable(false);

        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text.Trim(),
            Password = passwordInput.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, SucesoLogin, OnLoginFailure);
        loginStatusText.text = "Iniciando sesión...";
    }

    private bool ValidartextosLogin()
    {
        if (string.IsNullOrWhiteSpace(emailInput.text))
        {
            loginStatusText.text = "Por favor ingresa tu email";
            return false;
        }

        if (!ValidarEmail(emailInput.text.Trim()))
        {
            loginStatusText.text = "El formato del email no es válido";
            return false;
        }

        if (string.IsNullOrWhiteSpace(passwordInput.text))
        {
            loginStatusText.text = "Por favor ingresa tu contraseña";
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

    public void BotonRegistrarClicked()
    {
        if (!ValidarRegistroInputs())
            return;

        SetInteractable(false);

        var request = new RegisterPlayFabUserRequest
        {
            Email = registerEmailInput.text.Trim(),
            Password = registerPasswordInput.text,
            Username = usernameInput.text.Trim(),
            RequireBothUsernameAndEmail = true
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
        registerStatusText.text = "Registrando cuenta...";
    }

    public void GuardarDisplayName(string nombreJugador)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nombreJugador
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            result => {
                Debug.Log("DisplayName guardado: " + result.DisplayName);
            },
            error => {
                Debug.LogError("Error al guardar DisplayName: " + error.GenerateErrorReport());
            });
    }

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

    public void VerPanelRegistro()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        registerStatusText.text = "";
        limpiarInputsRegistro();
    }

    public void VerPanelLogin()
    {
        registerPanel.SetActive(false);
        gamePanel.SetActive(false);
        loginPanel.SetActive(true);
        loginStatusText.text = "";
        LimpiarInputsLogin();
    }

    private void LimpiarInputsLogin()
    {
        emailInput.text = "";
        passwordInput.text = "";
    }

    private void limpiarInputsRegistro()
    {
        registerEmailInput.text = "";
        usernameInput.text = "";
        registerPasswordInput.text = "";
    }
    #endregion

    #region Callbacks

    private void SucesoLogin(LoginResult result)
    {
        SetInteractable(true);

        _playFabId = result.PlayFabId;
        _sessionTicket = result.SessionTicket;

        PlayerPrefs.SetString("PLAYFAB_ID", _playFabId);
        PlayerPrefs.SetString("SESSION_TICKET", _sessionTicket);

        loginStatusText.text = "¡Bienvenido!";
        Debug.Log("Login exitoso");
        Inicio.desdeInicio = false;

        loginPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        SetInteractable(true);

        string errorMessage = "Error al iniciar sesión";

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
            default:
                errorMessage = $"Error: {error.ErrorMessage}";
                break;
        }

        loginStatusText.text = errorMessage;
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        SetInteractable(true);

        var loginRequest = new LoginWithEmailAddressRequest
        {
            Email = registerEmailInput.text.Trim(),
            Password = registerPasswordInput.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(loginRequest, loginResult =>
        {
            GuardarDisplayName(usernameInput.text.Trim());

            registerStatusText.text = "¡Registro y login exitoso!";
            VerPanelLogin();
            loginStatusText.text = "Ahora puedes iniciar sesión";
        },
        loginError =>
        {
            registerStatusText.text = "Error al iniciar sesión después de registrar";
            SetInteractable(true);
            Debug.LogError(loginError.GenerateErrorReport());
        });
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        try
        {
            SetInteractable(true);

            string errorMessage = "Error al registrar";

            switch (error.Error)
            {
                case PlayFabErrorCode.EmailAddressNotAvailable:
                    errorMessage = "El email ya está en uso.";
                    break;
                case PlayFabErrorCode.UsernameNotAvailable:
                    errorMessage = "El nombre de usuario no está disponible.";
                    break;
                case PlayFabErrorCode.InvalidPassword:
                    errorMessage = $"La contraseña debe tener al menos {minimoTamanioPass} caracteres.";
                    break;
                default:
                    errorMessage = $"Error desconocido: {error.ErrorMessage}";
                    break;
            }

            if (registerStatusText != null)
            {
                registerStatusText.gameObject.SetActive(true);
                registerStatusText.text = errorMessage;
            }

            Debug.LogWarning(errorMessage);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Excepción inesperada en OnRegisterFailure: {ex}");
            if (registerStatusText != null)
                registerStatusText.text = "Ha ocurrido un error inesperado.";
        }
    }

    #endregion

    #region Helpers

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

    private void SetInteractable(bool interactable)
    {
        emailInput.interactable = interactable;
        passwordInput.interactable = interactable;
        registerEmailInput.interactable = interactable;
        registerPasswordInput.interactable = interactable;
        usernameInput.interactable = interactable;
    }
    #endregion

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
