using System.Security.Cryptography;
using System.Text;

namespace X10Card.Models
{
    public class AesCryptography
    {
        protected static string SecretKey = "%&2022$X10%Registration$$1309cardKey%";

        public static string Encrypt(string plainText)
        {
            try
            {
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                using var aes = GetAes(SecretKey);
                var encryptedBytes = Encrypt(plainBytes, aes);
                return Convert.ToBase64String(encryptedBytes);
            }
            catch
            {
                return " ";
            }
        }

        public static string Decrypt(string encryptedText)
        {
            try
            {
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                using var aes = GetAes(SecretKey);
                var decryptedBytes = Decrypt(encryptedBytes, aes);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch
            {
                return "";
            }
        }

        public static byte[] Encrypt(byte[] plainBytes, Aes aes)
        {
            using var encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        public static byte[] Decrypt(byte[] encryptedData, Aes aes)
        {
            using var decryptor = aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        public static Aes GetAes(string secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));

            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Key = keyBytes;
            aes.IV = keyBytes; // Using same as key (not recommended for production, but keeps old behavior)
            return aes;
        }

        public static string GetSha256FromString(string strData)
        {
            try
            {
                using SHA256 sha256 = SHA256.Create();
                var hashValue = sha256.ComputeHash(Encoding.ASCII.GetBytes(strData));
                StringBuilder hex = new StringBuilder(hashValue.Length * 2);
                foreach (byte b in hashValue)
                    hex.AppendFormat("{0:x2}", b);
                return hex.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /* public static string Encrypt(string plainText)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            try
            {
                return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged(SecretKey)));
            }
            catch
            {
                return " ";
            }
        }
        public static string Decrypt(string encryptedText)
        {
            try
            {
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                return Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged(SecretKey)));
            }
            catch
            {
                return "";
            }
        }
        
        public static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
         {
             return rijndaelManaged.CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);
         }
         public static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
         {
             return rijndaelManaged.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
         }
         public static RijndaelManaged GetRijndaelManaged(string secretKey)
         {
             var keyBytes = new byte[16];
             var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
             Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
             return new RijndaelManaged
             {
                 Mode = CipherMode.CBC,
                 Padding = PaddingMode.PKCS7,
                 KeySize = 128,
                 BlockSize = 128,
                 Key = keyBytes,
                 IV = keyBytes
             };
         }


         public static string GetSha256FromString(string strData)
         {
             try
             {
                 var message = Encoding.ASCII.GetBytes(strData);
                 SHA256Managed hashString = new SHA256Managed();
                 string hex = "";
                 var hashValue = hashString.ComputeHash(message);
                 foreach (byte x in hashValue)
                 {
                     hex += string.Format("{0:x2}", x);
                 }
                 return hex;
             }
             catch { return ""; }
         }*/
    }
}

