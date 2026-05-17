using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class SaveManager : MonoBehaviour
{
    public GameController game;
    public OfflineProductionManager offlineProductionManager;

    private const string API_URL = "https://pau-joc-backend.onrender.com/api";
    private const string TOKEN_KEY = "cloudToken";
    private const string SAVE_INTERVAL_KEY = "saveInterval";
    private const string LAST_SAVE_TIME_KEY = "lastSaveTime";

    [Header("Crear Usuari")]
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerPasswordInput;

    [Header("Iniciar Sessió")]
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;

    [Header("UI")]
    public TMP_Text statusText;
    public Slider saveIntervalSlider;
    public TMP_Text saveIntervalText;
    public TMP_Text sessionStatusText;

    private float saveTimer = 0f;
    private bool autoSaveEnabled = false;
    private bool cloudSaveEnabled = false;
    private float SaveInterval => saveIntervalSlider != null ? saveIntervalSlider.value : 60f;

    public bool IsLoggedIn => PlayerPrefs.HasKey(TOKEN_KEY);
    private string Token => PlayerPrefs.GetString(TOKEN_KEY);

    // INICIALITZACIÓ

    private void Start()
    {
        game.data = SaveSystem.SaveExists("playerData") ? SaveSystem.LoadPlayer<Data>("playerData") : new Data();
        offlineProductionManager.CalculateOfflineProduction();

        if (saveIntervalSlider != null)
        {
            saveIntervalSlider.minValue = 10f;
            saveIntervalSlider.maxValue = 300f;
            saveIntervalSlider.value = PlayerPrefs.GetFloat(SAVE_INTERVAL_KEY, 60f);
            saveIntervalSlider.onValueChanged.AddListener(OnSliderChanged);
            UpdateSliderText(saveIntervalSlider.value);
        }

        UpdateSessionStatus(PlayerPrefs.GetString("cloudUsername", ""));
    }

    private void OnSliderChanged(float value)
    {
        PlayerPrefs.SetFloat(SAVE_INTERVAL_KEY, value);
        UpdateSliderText(value);
    }

    private void UpdateSliderText(float seconds)
    {
        if (saveIntervalText == null) return;

        if (seconds < 60f)
            saveIntervalText.text = $"Guardat automàtic: cada {seconds:N0} segons";
        else
            saveIntervalText.text = $"Guardat automàtic: cada {seconds / 60f:N1} minuts";
    }

    // AUTOGUARDAT

    private void Update()
    {
        if (!autoSaveEnabled) return;
        saveTimer += Time.deltaTime;
        if (saveTimer < SaveInterval) return;
        saveTimer = 0f;
        AutoSave();
    }

    private void AutoSave()
    {
        SaveSystem.SavePlayer(game.data, "playerData");
        Debug.Log("[AutoSave] Partida guardada localment.");

        if (IsLoggedIn && cloudSaveEnabled)
            StartCoroutine(CloudSaveCoroutine(isAuto: true));
    }

    public void EnableAutoSave()
    {
        autoSaveEnabled = true;
        saveTimer = 0f;
    }

    public void EnableCloudSave() { cloudSaveEnabled = true; }
    public void DisableCloudSave() { cloudSaveEnabled = false; }

    // GUARDAT MANUAL

    public void ManualSave()
    {
        SaveSystem.SavePlayer(game.data, "playerData");

        if (IsLoggedIn)
            StartCoroutine(CloudSaveCoroutine(isAuto: false));
        else
            SetStatus("<color=#4AE054>Partida guardada</color> localment.");
    }

    // ELIMINAR GUARDAT LOCAL

    public void DeleteLocalSave()
    {
        SaveSystem.DeleteLocalSave("playerData");
        game.data = new Data();
        cloudSaveEnabled = false; // simplement desactivo el cloud save per evitar que el núvol es quedi sense dades
        SetStatus("<color=#E05454>Dades locals eliminades.</color> Partida nova iniciada.");
    }

    // NOVA PARTIDA
    public void NewGame()
    {
        game.data = new Data();
        SetStatus("<color=#4AE054>Nova partida</color> iniciada!");
        EnableAutoSave();
    }

    // CÀRREGA

    public void LocalLoad()
    {
        if (!SaveSystem.SaveExists("playerData"))
        {
            SetStatus("<color=#E05454>No hi ha cap partida</color> guardada localment.");
            return;
        }
        game.data = SaveSystem.LoadPlayer<Data>("playerData");
        SetStatus("<color=#4AE054>Partida carregada</color> localment.");
        EnableAutoSave();

        if (IsLoggedIn) EnableCloudSave();
    }

    public void CloudLoad() => StartCoroutine(CloudLoadCoroutine());

    // AUTH

    public void Register() => StartCoroutine(RegisterCoroutine());
    public void Login() => StartCoroutine(LoginCoroutine());
    public void Logout()
    {
        PlayerPrefs.DeleteKey(TOKEN_KEY);
        PlayerPrefs.DeleteKey("cloudUsername");
        DisableCloudSave(); // Parem només el guardat al núvol. Al fer Logout es manté la lògica d'autoguardat local
        SetStatus("Sessió tancada.");
        UpdateSessionStatus();
    }

    private IEnumerator RegisterCoroutine()
    {
        SetStatus("Registrant...");
        string json = $"{{\"username\":\"{registerUsernameInput.text}\",\"password\":\"{registerPasswordInput.text}\"}}";

        using var req = new UnityWebRequest(API_URL + "/register", "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
            SetStatus("<color=#4AE054>Registre correcte!</color> Ara inicia sessió.");
        else
            SetStatus($"<color=#E05454>Error:</color> {ParseError(req.downloadHandler.text)}");
    }

    private IEnumerator LoginCoroutine()
    {
        SetStatus("Iniciant sessió...");
        string json = $"{{\"username\":\"{loginUsernameInput.text}\",\"password\":\"{loginPasswordInput.text}\"}}";

        using var req = new UnityWebRequest(API_URL + "/login", "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<TokenResponse>(req.downloadHandler.text);
            PlayerPrefs.SetString(TOKEN_KEY, response.token);
            PlayerPrefs.SetString("cloudUsername", loginUsernameInput.text);
            PlayerPrefs.Save();
            SetStatus("<color=#4AE054>Sessió iniciada</color> correctament!");
            UpdateSessionStatus(loginUsernameInput.text);
            if (autoSaveEnabled) EnableCloudSave();
        }
        else
        {
            SetStatus($"<color=#E05454>Error:</color> {ParseError(req.downloadHandler.text)}");
        }
    }

    // CLOUD SAVE / LOAD COROUTINES
    private IEnumerator CloudSaveCoroutine(bool isAuto = false)
    {
        if (!IsLoggedIn) { SetStatus("<color=#E05454>Has d'iniciar sessió</color> primer."); yield break; }

        if (!isAuto) SetStatus("Guardant al núvol...");

        string base64 = DataToBase64(game.data);
        string json = $"{{\"saveData\":\"{base64}\"}}";

        using var req = new UnityWebRequest(API_URL + "/save", "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Authorization", Token);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
            SetStatus("<color=#4AE054>Partida guardada</color> localment i al núvol!");
        else
            SetStatus($"<color=#E05454>Error en guardar al núvol:</color> {ParseError(req.downloadHandler.text)}");
    }

    private IEnumerator CloudLoadCoroutine()
    {
        if (!IsLoggedIn) { SetStatus("<color=#E05454>Has d'iniciar sessió</color> primer."); yield break; }

        SetStatus("Carregant del núvol...");

        using var req = UnityWebRequest.Get(API_URL + "/save");
        req.SetRequestHeader("Authorization", Token);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            SetStatus($"<color=#E05454>Error en carregar:</color> {ParseError(req.downloadHandler.text)}");
            yield break;
        }

        var response = JsonUtility.FromJson<SaveResponse>(req.downloadHandler.text);

        if (string.IsNullOrEmpty(response.saveData))
        {
            SetStatus("<color=#E05454>No hi ha cap partida</color> guardada al núvol.");
            yield break;
        }

        Data loadedData = Base64ToData(response.saveData);

        if (loadedData == null)
        {
            SetStatus("<color=#E05454>Error en llegir</color> les dades del núvol.");
            yield break;
        }

        game.data = loadedData;

        SetStatus("<color=#4AE054>Partida carregada</color> del núvol!");

        EnableAutoSave();
        EnableCloudSave();
    }

    // HELPERS
    private string DataToBase64(Data data)
    {
        var formatter = new BinaryFormatter();
        var memoryStream = new MemoryStream();
        formatter.Serialize(memoryStream, data);
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    private Data Base64ToData(string base64)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(base64);
            var memoryStream = new MemoryStream(bytes);
            var formatter = new BinaryFormatter();
            return (Data)formatter.Deserialize(memoryStream);
        }
        catch (Exception e)
        {
            Debug.LogError($"[Cloud] Error deserialitzant: {e.Message}");
            return null;
        }
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
        Debug.Log($"[Cloud] {msg}");
    }

    private string ParseError(string json)
    {
        try { return JsonUtility.FromJson<ErrorResponse>(json).error; }
        catch { return json; }
    }

    private void UpdateSessionStatus(string username = "")
    {
        if (sessionStatusText == null) return;

        if (IsLoggedIn && !string.IsNullOrEmpty(username))
            sessionStatusText.text = $"<color=#4AE054>S'ha iniciat sessió.</color>\nBenvingut, {username}!";
        else if (IsLoggedIn)
            sessionStatusText.text = $"<color=#4AE054>S'ha iniciat sessió.</color>";
        else
            sessionStatusText.text = $"<color=#E05454>No s'ha iniciat sessió.</color>";
    }

    [Serializable] private class TokenResponse { public string token; }
    [Serializable] private class SaveResponse { public string saveData; }
    [Serializable] private class ErrorResponse { public string error; }


    // Al marxar del joc (per calcular la producció offline amb series de taylor)
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString(LAST_SAVE_TIME_KEY, DateTime.UtcNow.ToString("o"));
        PlayerPrefs.Save();
        SaveSystem.SavePlayer(game.data, "playerData");
    }
}