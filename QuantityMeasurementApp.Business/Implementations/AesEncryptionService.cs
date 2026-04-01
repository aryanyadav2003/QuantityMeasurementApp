using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace QuantityMeasurementApp.Business.Implementations
{
    public class AesEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AesEncryptionService(IConfiguration configuration)
        {
            string key = configuration["Aes:Key"]
                ?? throw new InvalidOperationException("AES Key not configured.");
            string iv  = configuration["Aes:IV"]
                ?? throw new InvalidOperationException("AES IV not configured.");

            // AES-256 requires exactly 32 bytes for key
            if (Encoding.UTF8.GetByteCount(key) != 32)
                throw new InvalidOperationException(
                    "AES Key must be exactly 32 characters.");

            // AES block size requires exactly 16 bytes for IV
            if (Encoding.UTF8.GetByteCount(iv) != 16)
                throw new InvalidOperationException(
                    "AES IV must be exactly 16 characters.");

            _key = Encoding.UTF8.GetBytes(key);
            _iv  = Encoding.UTF8.GetBytes(iv);
        }

        // ── ENCRYPT ───────────────────────────────────────────

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException(
                    "Plain text cannot be null or empty.");

            using Aes aes = Aes.Create();
            aes.Key     = _key;
            aes.IV      = _iv;
            aes.Mode    = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream(
                memoryStream, encryptor, CryptoStreamMode.Write);
            using StreamWriter streamWriter = new StreamWriter(cryptoStream);

            streamWriter.Write(plainText);
            streamWriter.Flush();
            cryptoStream.FlushFinalBlock();

            // Return as Base64 string for safe storage in DB
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        // ── DECRYPT ───────────────────────────────────────────

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException(
                    "Cipher text cannot be null or empty.");

            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Key     = _key;
            aes.IV      = _iv;
            aes.Mode    = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new MemoryStream(cipherBytes);
            using CryptoStream cryptoStream = new CryptoStream(
                memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new StreamReader(cryptoStream);

            return streamReader.ReadToEnd();
        }
    }
}