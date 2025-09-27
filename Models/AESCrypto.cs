using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace X10Card.Models
{
    public class AESCrypto
    {
        static string secretkey = "eadbccb64aa58a427b750c80445054524e960934a448e848c932ba4dd5a5c64d";
        //static string secretkey = "250e19c51658ce84e86e92433cf9a2e961f98f45460ad0eb596271f5c8bb0e6c";

        public static string EncryptAES(object obj)
        {

            AESCrypto Obj = new AESCrypto();
            string jsonstring = JsonConvert.SerializeObject(obj);
            return Obj.Encrypt(jsonstring);
        }

        public static string Decrypt(string cipherText)
        {
            byte[] cipherData = Convert.FromBase64String(cipherText);
            byte[] saltData = cipherData.Skip(8).Take(8).ToArray(); // Extract salt from cipher text

            using (var md5 = MD5.Create()) // Consider using SHA256 for better security
            {
                var keyAndIV = GenerateKeyAndIV(32, 16, 1, saltData, Encoding.UTF8.GetBytes(secretkey), md5);
                using (var aes = Aes.Create())
                {
                    aes.Key = keyAndIV[0];
                    aes.IV = keyAndIV[1];
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor())
                    {
                        byte[] encryptedData = cipherData.Skip(16).ToArray(); // Extract encrypted data (after the "Salted__")
                        byte[] decryptedData = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                        return Encoding.UTF8.GetString(decryptedData); // Return decrypted string
                    }
                }
            }
        }



        /* public string Encrypt(string plainText)
         {
             byte[] plainData = Encoding.UTF8.GetBytes(plainText);
             byte[] saltData = new byte[8];

             using (var rng = new RNGCryptoServiceProvider())
             {
                 rng.GetBytes(saltData); // Generate random salt
             }

             using (var md5 = MD5.Create()) // Consider using SHA256 for better security
             {
                 var keyAndIV = GenerateKeyAndIV(32, 16, 1, saltData, Encoding.UTF8.GetBytes(secretkey), md5);
                 using (var aes = Aes.Create())
                 {
                     aes.Key = keyAndIV[0];
                     aes.IV = keyAndIV[1];
                     aes.Mode = CipherMode.CBC;
                     aes.Padding = PaddingMode.PKCS7;

                     using (var encryptor = aes.CreateEncryptor())
                     {
                         byte[] encryptedData = encryptor.TransformFinalBlock(plainData, 0, plainData.Length);
                         byte[] cipherData = new byte[8 + saltData.Length + encryptedData.Length];
                         Array.Copy(Encoding.UTF8.GetBytes("Salted__"), 0, cipherData, 0, 8); // Add the "Salted__" header
                         Array.Copy(saltData, 0, cipherData, 8, saltData.Length); // Add the salt
                         Array.Copy(encryptedData, 0, cipherData, 16, encryptedData.Length); // Add the encrypted data
                         return Convert.ToBase64String(cipherData); // Return base64-encoded ciphertext
                     }
                 }
             }
         }
 */
        public string Encrypt(string plainText)
        {
            byte[] plainData = Encoding.UTF8.GetBytes(plainText);
            byte[] saltData = new byte[8];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltData); // Generate random salt
            }

            using (var md5 = MD5.Create()) // Consider using SHA256 for better security
            {
                var keyAndIV = GenerateKeyAndIV(32, 16, 1, saltData, Encoding.UTF8.GetBytes(secretkey), md5);
                using (var aes = Aes.Create())
                {
                    aes.Key = keyAndIV[0];
                    aes.IV = keyAndIV[1];
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var encryptor = aes.CreateEncryptor())
                    {
                        byte[] encryptedData = encryptor.TransformFinalBlock(plainData, 0, plainData.Length);
                        byte[] cipherData = new byte[8 + saltData.Length + encryptedData.Length];
                        Array.Copy(Encoding.UTF8.GetBytes("Salted__"), 0, cipherData, 0, 8); // Add the "Salted__" header
                        Array.Copy(saltData, 0, cipherData, 8, saltData.Length); // Add the salt
                        Array.Copy(encryptedData, 0, cipherData, 16, encryptedData.Length); // Add the encrypted data
                        return Convert.ToBase64String(cipherData); // Return base64-encoded ciphertext
                    }
                }
            }
        }

        private static byte[][] GenerateKeyAndIV(int keyLength, int ivLength, int iterations, byte[] salt, byte[] password, MD5 md5)
        {
            int digestLength = md5.HashSize / 8;
            int requiredLength = ((keyLength + ivLength + digestLength - 1) / digestLength) * digestLength;
            byte[] generatedData = new byte[requiredLength];
            int generatedLength = 0;
            using (var ms = new MemoryStream())
            {
                while (generatedLength < keyLength + ivLength)
                {
                    if (generatedLength > 0)
                        ms.Write(generatedData, generatedLength - digestLength, digestLength);

                    ms.Write(password, 0, password.Length);
                    if (salt != null)
                        ms.Write(salt, 0, salt.Length); // Writing salt to stream

                    byte[] input = ms.ToArray();
                    ms.SetLength(0);
                    byte[] hash = md5.ComputeHash(input);

                    for (int i = 1; i < iterations; i++)
                    {
                        hash = md5.ComputeHash(hash);
                    }

                    Array.Copy(hash, 0, generatedData, generatedLength, hash.Length);
                    generatedLength += hash.Length;
                }
            }

            byte[][] result = new byte[2][];
            result[0] = new byte[keyLength];
            Array.Copy(generatedData, 0, result[0], 0, keyLength);

            if (ivLength > 0)
            {
                result[1] = new byte[ivLength];
                Array.Copy(generatedData, keyLength, result[1], 0, ivLength);
            }

            return result;
        }

    }
}
