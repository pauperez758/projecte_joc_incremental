using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
public static class SimpleAES
{
    private const string initVector = "14rukl893fhjl9r5";
    private const int keysize = 256;

    public static string EncryptString(string plainText, string passPhrase)
    {
        byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
        byte[] keyBytes = password.GetBytes(keysize / 8);

        RijndaelManaged symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC };
        ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

        MemoryStream memoryStream = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        cryptoStream.FlushFinalBlock();

        byte[] cipherTextBytes = memoryStream.ToArray();

        memoryStream.Close();
        cryptoStream.Close();

        return Convert.ToBase64String(cipherTextBytes);
    }

    // Decrypt
    public static string DecryptString(string cipherText, string passPhrase)
    {
        byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
        byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

        PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
        byte[] keyBytes = password.GetBytes(keysize / 8);

        RijndaelManaged symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC };
        ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

        MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
        CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

        byte[] plainTextBytes = new byte[cipherTextBytes.Length];
        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

        memoryStream.Close();
        cryptoStream.Close();

        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
    }
}
public class SaveSystem : MonoBehaviour
{
    public TMP_InputField importValue;
    public TMP_InputField exportValue;

    private static string encryptKey = "3xhhr44dj1sc5g6h6f0pvq4g";
    // Ruta per a les partides guardades. Ruta absoluta: C:\Users\user\AppData\LocalLow\JocsDePau\PROJECTE Joc Incremental\saves
    // DEBUG: En el meu cas (ELIMINAR DESPRÉS): C:\Users\klaud\AppData\LocalLow\JocsDePau\PROJECTE Joc Incremental\saves
    private static string savePath = Application.persistentDataPath + "/saves/";
    private static string savePathBackUP = Application.persistentDataPath + "/backups/";
    private static int backUpCount = 0;

    public static void SavePlayer<T>(T Data, string name)
    {
        Directory.CreateDirectory(savePath);
        Directory.CreateDirectory(savePathBackUP);

        backUpCount++;
        string targetPath = (backUpCount % 4 == 0) ? savePathBackUP : savePath;

        Save(targetPath);

        void Save(string path)
        {
            using (var writer = new StreamWriter(path + name + ".txt"))
            {
                var formatter = new BinaryFormatter();
                var memoryStream = new MemoryStream();

                formatter.Serialize(memoryStream, Data);

                string dataToWrite = SimpleAES.EncryptString(
                    Convert.ToBase64String(memoryStream.ToArray()),
                    encryptKey
                );

                writer.WriteLine(dataToWrite);
            }
        }
    }

    public static T LoadPlayer<T>(string name)
    {
        Directory.CreateDirectory(savePath);
        Directory.CreateDirectory(savePathBackUP);

        T returnValue = default;
        bool backUpNeeded = false;

        Load(savePath);

        if (backUpNeeded)
        {
            Load(savePathBackUP);
        }

        return returnValue;

        void Load(string path)
        {
            string fullPath = path + name + ".txt";
            if (!File.Exists(fullPath))
            {
                backUpNeeded = true;
                return;
            }

            try
            {
                using (var reader = new StreamReader(fullPath))
                {
                    var formatter = new BinaryFormatter();
                    string dataToRead = reader.ReadToEnd();

                    byte[] decryptedBytes = Convert.FromBase64String(
                        SimpleAES.DecryptString(dataToRead, encryptKey)
                    );

                    var memoryStream = new MemoryStream(decryptedBytes);
                    returnValue = (T)formatter.Deserialize(memoryStream);
                    backUpNeeded = false;
                }
            }
            catch
            {
                returnValue = default;
                backUpNeeded = true;
            }
        }
    }

    public static void DeleteLocalSave(string name)
    {
        string path = savePath + name + ".txt";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveSystem] Partida local '{name}' eliminada.");
        }
    }

    public static bool SaveExists(string key)
    {
        string path = savePath + key + ".txt";
        return File.Exists(path);
    }

    // Nota: Aquesta funció necessita la variable 'name'. 
    // Si 'name' no és global, s'ha de passar per paràmetre.
    public void ImportPlayer(string name)
    {
        Directory.CreateDirectory(savePath);
        using (var writer = new StreamWriter(savePath + name + ".txt"))
        {
            writer.WriteLine(importValue.text);
        }
    }

    public void ExportPlayer(string name)
    {
        string dataToRead = "";
        bool backUpNeeded = false;

        Load(savePath);
        if (backUpNeeded)
        {
            Load(savePathBackUP);
        }

        exportValue.text = dataToRead;

        void Load(string path)
        {
            string fullPath = path + name + ".txt";
            if (!File.Exists(fullPath))
            {
                backUpNeeded = true;
                return;
            }

            try
            {
                using (var reader = new StreamReader(fullPath))
                {
                    dataToRead = reader.ReadToEnd();
                    backUpNeeded = false;
                }
            }
            catch
            {
                backUpNeeded = true;
            }
        }
    }

    public void ClearFields()
    {
        exportValue.text = "";
        importValue.text = "";
    }
}