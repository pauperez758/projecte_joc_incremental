using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class CloudSaveManager : MonoBehaviour
{
    public GameController game;

    private const string API_URL = "https://pau-joc-backend.onrender.com/api";
    private const string TOKEN_KEY = "cloudToken";

    // UI
    [Header("Crear Usuari")]
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerPasswordInput;

    [Header("Iniciar Sessió")]
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;

    public TMP_Text statusText;

    public bool IsLoggedIn => PlayerPrefs.HasKey(TOKEN_KEY);
    private string Token => PlayerPrefs.GetString(TOKEN_KEY);

    // AUTH
    public void Register() => StartCoroutine(RegisterCoroutine());
    public void Login() => StartCoroutine(LoginCoroutine());
    public void Logout()
    {
        PlayerPrefs.DeleteKey(TOKEN_KEY);
        SetStatus("Sessió tancada.");
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
            PlayerPrefs.Save();
            SetStatus("<color=#4AE054>Sessió iniciada</color> correctament!");
        }
        else
        {
            SetStatus($"<color=#E05454>Error:</color> {ParseError(req.downloadHandler.text)}");
        }
    }

    // GUARDAR AL NÚVOL

    public void CloudSave() => StartCoroutine(CloudSaveCoroutine());

    private IEnumerator CloudSaveCoroutine()
    {
        SetStatus("<color=#E05454>Has d'iniciar sessió</color> primer.");

        SetStatus("Guardant al núvol...");

        // Convertim les dades actuals a base64 (igual que SaveSystem)
        string base64 = DataToBase64(game.data);
        string json = $"{{\"saveData\":\"{base64}\"}}";

        using var req = new UnityWebRequest(API_URL + "/save", "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Authorization", Token);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
            SetStatus("<color=#4AE054>Partida guardada</color> al núvol!");
        else
            SetStatus($"<color=#E05454>Error en guardar:</color> {ParseError(req.downloadHandler.text)}");
    }

    // CARREGAR DEL NÚVOL

    public void CloudLoad() => StartCoroutine(CloudLoadCoroutine());

    private IEnumerator CloudLoadCoroutine()
    {
        if (!IsLoggedIn) { SetStatus("Has d'iniciar sessió primer."); yield break; }

        SetStatus("Carregant del núvol...");

        using var req = UnityWebRequest.Get(API_URL + "/save");
        req.SetRequestHeader("Authorization", Token);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<SaveResponse>(req.downloadHandler.text);
            if (string.IsNullOrEmpty(response.saveData))
            {
                SetStatus("<color=#E05454>No hi ha cap partida</color> guardada al núvol.");
                yield break;
            }

            Data loadedData = Base64ToData(response.saveData);
            if (loadedData != null)
            {
                game.data = loadedData;
                SetStatus("<color=#4AE054>Partida carregada</color> del núvol!");
            }
            else
            {
                SetStatus("<color=#E05454>Error en llegir</color> les dades del núvol.");
            }
        }
        else
        {
            SetStatus($"<color=#E05454>Error en carregar:</color> {ParseError(req.downloadHandler.text)}");
        }
    }

    // HELPERS — Conversió Data <-> Base64
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
            Debug.LogError($"Error deserialitzant: {e.Message}");
            return null;
        }
    }

    // HELPERS — UI i JSON
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

    [Serializable] private class TokenResponse { public string token; }
    [Serializable] private class SaveResponse { public string saveData; }
    [Serializable] private class ErrorResponse { public string error; }
}