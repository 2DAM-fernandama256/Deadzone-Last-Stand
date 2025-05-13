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

    [Header("UI References - Register")]
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField usernameInput;
    public TextMeshProUGUI registerStatusText;
    public GameObject registerPanel;

    [Header("Configuración")]
    public float minPasswordLength = 6;
    public GameObject loadingSpinner;

    public GameObject gamePanel;

    private string _playFabId;
    private string _sessionTicket;

    void Start()
    {
        ShowLoginPanel();

    }

    #region Login
    public void OnLoginButtonClicked()
    {
        if (!ValidateLoginInputs())
            return;

        loadingSpinner.SetActive(true);
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

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        loginStatusText.text = "Iniciando sesión...";
    }

    private bool ValidateLoginInputs()
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

        if (!IsValidEmail(emailInput.text.Trim()))
        {
            loginStatusText.text = "El formato del email no es válido";
            return false;
        }

        if (passwordInput.text.Length < minPasswordLength)
        {
            loginStatusText.text = $"La contraseña debe tener al menos {minPasswordLength} caracteres";
            return false;
        }

        return true;
    }
    #endregion

    #region Registro
    public void OnRegisterButtonClicked()
    {
        if (!ValidateRegisterInputs())
            return;

        loadingSpinner.SetActive(true);
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

    private bool ValidateRegisterInputs()
    {
        if (string.IsNullOrWhiteSpace(registerEmailInput.text))
        {
            registerStatusText.text = "Por favor ingresa un email";
            return false;
        }

        if (!IsValidEmail(registerEmailInput.text.Trim()))
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

        if (registerPasswordInput.text.Length < minPasswordLength)
        {
            registerStatusText.text = $"La contraseña debe tener al menos {minPasswordLength} caracteres";
            return false;
        }

        return true;
    }

    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        registerStatusText.text = "";
        ClearRegisterInputs();
    }

    public void ShowLoginPanel()
    {
        registerPanel.SetActive(false);
        loginPanel.SetActive(true);
        loginStatusText.text = "";
        ClearLoginInputs();
    }

    private void ClearLoginInputs()
    {
        emailInput.text = "";
        passwordInput.text = "";
    }

    private void ClearRegisterInputs()
    {
        registerEmailInput.text = "";
        usernameInput.text = "";
        registerPasswordInput.text = "";
    }
    #endregion

    #region Callbacks
    private void OnLoginSuccess(LoginResult result)
    {
        loadingSpinner.SetActive(false);
        SetInteractable(true);

        _playFabId = result.PlayFabId;
        _sessionTicket = result.SessionTicket;

        PlayerPrefs.SetString("PLAYFAB_ID", _playFabId);
        PlayerPrefs.SetString("SESSION_TICKET", _sessionTicket);

        loginStatusText.text = "¡Bienvenido!";
        Debug.Log("Login exitoso");

        // 🔄 Activar panel de juego
        loginPanel.SetActive(false);
        gamePanel.SetActive(true);
    }


    private void OnLoginFailure(PlayFabError error)
    {
        loadingSpinner.SetActive(false);
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
        }

        loginStatusText.text = $"{errorMessage}";
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        loadingSpinner.SetActive(false);
        SetInteractable(true);

        registerStatusText.text = "¡Registro exitoso!";
        ShowLoginPanel();
        loginStatusText.text = "Ahora puedes iniciar sesión";
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        loadingSpinner.SetActive(false);
        SetInteractable(true);

        string errorMessage = "Error al registrar";

        switch (error.Error)
        {
            case PlayFabErrorCode.EmailAddressNotAvailable:
                errorMessage = "El email ya está en uso";
                break;
            case PlayFabErrorCode.UsernameNotAvailable:
                errorMessage = "El nombre de usuario no está disponible";
                break;
            case PlayFabErrorCode.InvalidPassword:
                errorMessage = $"La contraseña debe tener al menos {minPasswordLength} caracteres";
                break;
        }

        registerStatusText.text = $"{errorMessage}: {error.ErrorMessage}";
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    #region Helpers
    private bool IsValidEmail(string email)
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

        ShowLoginPanel();
        loginStatusText.text = "Sesión cerrada";
    }
}


