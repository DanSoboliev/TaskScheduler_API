using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TaskSchedulerAPI.Models;

namespace TaskSchedulerAPI.Data {
    public static class СryptographyData {

        #region *** AES ***
        private static byte[] AES_EncryptStringToBytes(string plainText, byte[] key, byte[] iv) {
            byte[] encrypted;
            using (Aes aes = Aes.Create()) {
                aes.KeySize = 128;
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream msEncrypt = new MemoryStream()) {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            };
            return encrypted;
        }
        private static string AES_Encrypt(string plainText, string key, string iv) {
            byte[] encrypted = AES_EncryptStringToBytes(plainText, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));
            return Convert.ToBase64String(encrypted);
        }
        public static void AES_Encrypt_IUser(this IUser user) {
            string key = AES_Creation128Key(user.UserName);
            user.UserPassword = AES_Encrypt(user.UserPassword, key, Strings.StrReverse(key));
        }

        private static string AES_DecryptStringFromBytes(byte[] encrypted, byte[] key, byte[] iv) {
            string plaintext = "";
            using (Aes aes = Aes.Create()) {
                aes.KeySize = 128;
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream msDecrypt = new MemoryStream(encrypted)) {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
        private static string AES_Decrypt(string cypher, string key, string iv) {
            string plainText = AES_DecryptStringFromBytes(Convert.FromBase64String(cypher), Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));
            return plainText;
        }
        public static void AES_Decrypt_IUser(this IUser user) {
            string key = AES_Creation128Key(user.UserName);
            user.UserPassword = AES_Decrypt(user.UserPassword, key, Strings.StrReverse(key));
        }

        /// <summary>
        /// Створює 16-ти байтовий(UTF-8) ключ на основі переданої стрічки
        /// </summary>
        /// <param name="basicForKey">Стрічка на основі якої буде створено ключ</param>
        /// <returns>16-ти байтовий(UTF-8) ключ</returns>
        private static string AES_Creation128Key(string basicForKey) {
            StringBuilder stringBuilder = new StringBuilder(16);
            for (int i = 0; i < 16; i++) {
                try {
                    stringBuilder.Append(basicForKey[i]);
                }
                catch {
                    int l = i - 1;
                    for (int j = 0; j < 16 - i; j++) {
                        if (l < 0) l = i - 1;
                        stringBuilder.Append(basicForKey[l]);
                        l--;
                    }
                    break;
                }
            }
            return stringBuilder.ToString();
        }
        #endregion

        #region *** RSA ***
        public static string _publicKey => "<RSAKeyValue><Modulus>sznV+BtuE+Y+M7XCtJb783LahPYgx4EbMFFdJzROTCki6GOX2D3dQwQ9Ry1SZfOaorP5tXVyhC/ZW+/qHgkh/Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private static string _private_public_Keys => "<RSAKeyValue><Modulus>sznV+BtuE+Y+M7XCtJb783LahPYgx4EbMFFdJzROTCki6GOX2D3dQwQ9Ry1SZfOaorP5tXVyhC/ZW+/qHgkh/Q==</Modulus><Exponent>AQAB</Exponent><P>53AUvArG/sicfjEoy4eBsf80oOPtOpFwyrR01pBTB4s=</P><Q>xj82lllYMoaddQQP4cBK7tyklB/1ORgBSlId6F+87Zc=</Q><DP>0x1UbKvQFj3dMueY9P/o+Pt5gIIptlFReDbglZEVjD0=</DP><DQ>MMMrsh+XyhXCdR3iqiyaQdaTxLt3neuBpb49DQM/fVE=</DQ><InverseQ>djGPPjt44kSLQNVTL85R0X0fnoX1oZ5d2sBKFCWTGk0=</InverseQ><D>YcC8zIsheecNrCSJ4veqyfit6PFZpZbsXmkyVqy3u9+VlyN+1jvYa6q8xZha6tw1RdmSiIv1RxdT239qirOqXQ==</D></RSAKeyValue>";

        private static byte[] RSA_EncryptStringToBytes(byte[] decrypted, string key) {
            byte[] encrypted;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider()) {
                rsa.FromXmlString(key);
                encrypted = rsa.Encrypt(decrypted, false);
            }
            return encrypted;
        }
        private static string RSA_Encrypt(string plainText, string key) {
            byte[] encrypted = RSA_EncryptStringToBytes(Encoding.UTF8.GetBytes(plainText), key);
            return Convert.ToBase64String(encrypted);
        }
        /// <summary>
        /// Шифрує пароль об'єкта user, що наслідує інтерфейс IUser
        /// </summary>
        /// <param name="user">Об'єкт, що наслідує інтерфейс IUser, пароль якого шифрується</param>
        /// <param name="key"></param>
        public static void RSA_Encrypt_IUser(this IUser user, string key) {
            user.UserPassword = RSA_Encrypt(user.UserPassword, key);
        }

        private static byte[] RSA_DecryptStringToBytes(byte[] encrypted, string key) {
            byte[] decrypted;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider()) {
                rsa.FromXmlString(key);
                decrypted = rsa.Decrypt(encrypted, false);
            }
            return decrypted;
        }
        private static string RSA_Decrypt(string cypher, string key) {
            byte[] plainText = RSA_DecryptStringToBytes(Convert.FromBase64String(cypher), key);
            return Encoding.UTF8.GetString(plainText);
        }
        /// <summary>
        /// Дешифрує пароль об'єкта user, що був зашифрований відкритим ключом наданим API, використовуючи закритий ключ API
        /// </summary>
        /// <param name="user">Об'єкт, що наслідує інтерфейс IUser, пароль якого потрібно дешифрувати</param>
        public static void RSA_Decrypt_IUser(this IUser user) {
            user.UserPassword = RSA_Decrypt(user.UserPassword, _private_public_Keys);
        }

        public static void RSA_KeyGenerate(out string publicKey, out string private_public_Keys) {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(512)) {
                publicKey = rsa.ToXmlString(false);
                private_public_Keys = rsa.ToXmlString(true);
            }
        }
        #endregion
    }
}
