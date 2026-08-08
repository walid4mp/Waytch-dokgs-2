// =====================================================================
//  Neon Cipher — Save/Load System
//  File:    SaveSystem.cs
//  Notes:   JSON serialization + AES-encrypted slots in Application.persistentDataPath.
//           Pure file I/O; gameplay code only uses the ISaveSystem interface.
// =====================================================================
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NeonCipher.Core;
using UnityEngine;

namespace NeonCipher.Saving
{
    public sealed class SaveSystem : ISaveSystem
    {
        private readonly IGameSettings _settings;
        private const int _keyCheckByte = 0xA7;

        public SaveSystem(IGameSettings settings) => _settings = settings;

        private string SlotPath(int slot) =>
            Path.Combine(Application.persistentDataPath, "saves", $"slot_{slot:00}.json");

        public bool Save(int slot, SaveData data)
        {
            try
            {
                data.SavedAtIso = DateTime.UtcNow.ToString("o");
                Directory.CreateDirectory(Path.GetDirectoryName(SlotPath(slot))!);
                string json = JsonUtility.ToJson(data, prettyPrint: false);
                byte[] cipher = Encrypt(json, DeriveKey());
                File.WriteAllBytes(SlotPath(slot), cipher);
                Debug.Log($"[Save] slot {slot} saved ({cipher.Length} bytes).");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Save] slot {slot} failed: {e.Message}");
                return false;
            }
        }

        public bool Load(int slot, out SaveData data)
        {
            data = null;
            try
            {
                string path = SlotPath(slot);
                if (!File.Exists(path)) return false;
                byte[] cipher = File.ReadAllBytes(path);
                string json = Decrypt(cipher, DeriveKey());
                data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log($"[Save] slot {slot} loaded (version {data?.Version ?? "?"}).");
                return data != null;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Save] slot {slot} load failed: {e.Message}");
                return false;
            }
        }

        public bool Delete(int slot)
        {
            try { File.Delete(SlotPath(slot)); return true; }
            catch { return false; }
        }

        // --- crypto helpers ---
        private byte[] DeriveKey() =>
            Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(_settings.Language + "neon-cipher-v1"),
                Encoding.UTF8.GetBytes("NeonCipherSaltsAreDeliciousAndLong2026"),
                100_000, HashAlgorithmName.SHA256, 32);

        private static byte[] Encrypt(string plain, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key; aes.GenerateIV(); aes.Mode = CipherMode.CBC;
            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, 16);
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs, Encoding.UTF8))
                sw.Write(plain);
            return ms.ToArray();
        }

        private static string Decrypt(byte[] cipher, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key; aes.Mode = CipherMode.CBC;
            byte[] iv = new byte[16]; Array.Copy(cipher, 0, iv, 0, 16);
            aes.IV = iv;
            using var ms = new MemoryStream(cipher, 16, cipher.Length - 16);
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs, Encoding.UTF8);
            return sr.ReadToEnd();
        }
    }
}
