using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace PedalacomLibrary
{
    public class Password
    {
        public static string EncryptPassword(string password)
        {
            string encryptedPassword = string.Empty;

            try
            { 
                SHA256 sha256 = SHA256.Create();

                //codifica password in bytes
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                //trasforma bytes da utf-8 a utf1-6
                for (int i = 0; i < bytes.Length; i++)
                {
                    encryptedPassword += bytes[i].ToString("x2");
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return encryptedPassword;
        }

        public static KeyValuePair<string, string> EncryptSaltPassword(string password)
        {
            byte[] byteSalt = RandomNumberGenerator.GetBytes(16);

            try
            {
                string encryptedResult = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: password,
                        salt: byteSalt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 32)
                        );

                string encryptedSalt = Convert.ToBase64String(byteSalt);
                
                return new KeyValuePair<string, string>(encryptedSalt, encryptedResult);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static KeyValuePair<string, string> DecryptSaltPassword(string salt, string password)
        {
            byte[] byteSalt = Convert.FromBase64String(salt);

            try
            {
                string encryptedResult = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: password,
                        salt: byteSalt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 32)
                        );

                string encryptedSalt = Convert.ToBase64String(byteSalt);
                
                return new KeyValuePair<string, string>(encryptedSalt, encryptedResult);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
