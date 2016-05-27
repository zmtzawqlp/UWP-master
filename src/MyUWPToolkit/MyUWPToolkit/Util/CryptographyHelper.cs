using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace MyUWPToolkit.Util
{
    /// <summary>
    /// 加密帮助类
    /// </summary>
    public static class CryptographyHelper
    {
        public static string DesEncrypt(string key, string plaintext)
        {
            SymmetricKeyAlgorithmProvider des = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.DesEcbPkcs7);
            IBuffer keyMaterial = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            CryptographicKey symmetricKey = des.CreateSymmetricKey(keyMaterial);

            IBuffer plainBuffer = CryptographicBuffer.ConvertStringToBinary(plaintext, BinaryStringEncoding.Utf8);

            IBuffer cipherBuffer = CryptographicEngine.Encrypt(symmetricKey, plainBuffer, null);
            return CryptographicBuffer.EncodeToHexString(cipherBuffer);
        }

        public static string TripleDesDecrypt(string key, string ciphertext)
        {
            SymmetricKeyAlgorithmProvider tripleDes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.TripleDesEcb);
            IBuffer keyMaterial = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            CryptographicKey symmetricKey = tripleDes.CreateSymmetricKey(keyMaterial);

            IBuffer cipherBuffer = CryptographicBuffer.DecodeFromHexString(ciphertext);

            IBuffer plainBuffer = CryptographicEngine.Decrypt(symmetricKey, cipherBuffer, null);
            return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, plainBuffer);
        }

        public static string Md5Encrypt(string value)
        {
            IBuffer data = CryptographicBuffer.ConvertStringToBinary(value, BinaryStringEncoding.Utf8);
            return Md5Encrypt(data);
        }

        public static string Md5Encrypt(IBuffer data)
        {
            HashAlgorithmProvider md5 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer hashedData = md5.HashData(data);
            return CryptographicBuffer.EncodeToHexString(hashedData);
        }

        public static string EncodeToBase64String(string value)
        {
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(value, BinaryStringEncoding.Utf8);
            return CryptographicBuffer.EncodeToBase64String(buffer);
        }

        public static string DecodeFromBase64String(string value)
        {
            IBuffer buffer = CryptographicBuffer.DecodeFromBase64String(value);
            return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffer);
        }
    }
}
