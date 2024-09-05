using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using Newtonsoft.Json;

public static class DataManager
{
    private static readonly string saveFileName = "OBTGameData.json";
    
    private static readonly string keyFileName = "encryptionKey.dat";
    private static readonly string ivFileName = "encryptionIV.dat";
    private static byte[] encryptionKey;
    private static byte[] encryptionIV;

    public static void LoadEncryptionKeyAndIv()
    {
        string keyPath = Path.Combine(Application.persistentDataPath, keyFileName);
        string ivPath = Path.Combine(Application.persistentDataPath, ivFileName);

        if (File.Exists(keyPath) && File.Exists(ivPath))
        {
            encryptionKey = File.ReadAllBytes(keyPath);
            encryptionIV = File.ReadAllBytes(ivPath);
        }
        else
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                encryptionKey = aes.Key;
                encryptionIV = aes.IV;
            }

            File.WriteAllBytes(keyPath, encryptionKey);
            File.WriteAllBytes(ivPath, encryptionIV);
        }
    }

    public static void SaveFile(GameData data)
    {
        string json = JsonConvert.SerializeObject(data);
        // string encrypt = Encrypt(json);

        // To-Do: 임시  변경
        // File.WriteAllText(Path.Combine(Application.persistentDataPath, saveFileName), encrypt);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, saveFileName), json);
    }

    public static GameData LoadFile()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        if (!File.Exists(path))
        {
            return null;
        }

        // string encrypt = File.ReadAllText(path);
        // string decrypt = Decrypt(encrypt);

        // To-Do: 임시  변경
        // return JsonConvert.DeserializeObject<GameData>(decrypt);
        return JsonConvert.DeserializeObject<GameData>(File.ReadAllText(path));
    }

    private static string Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = encryptionKey;
        aes.IV = encryptionIV;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using MemoryStream ms = new MemoryStream();
        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (StreamWriter sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    private static string Decrypt(string cipherText)
    {
        byte[] buffer = Convert.FromBase64String(cipherText);
        using Aes aes = Aes.Create();
        aes.Key = encryptionKey;
        aes.IV = encryptionIV;

        ICryptoTransform decrypt = aes.CreateDecryptor(aes.Key, aes.IV);
        using MemoryStream ms = new MemoryStream(buffer);
        using CryptoStream cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Read);
        using StreamReader sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}
