using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace PedalacomLibrary
{
    public class Password
    {
        public static KeyValuePair<string, string> EncryptSaltPassword(string password)
        {
            byte[] byteSalt = new byte[16];
            string encryptedResult = string.Empty;
            string encryptedSalt = string.Empty;

            try
            {
                //pippo
                RandomNumberGenerator.Fill(byteSalt);

                encryptedResult = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: password,
                        salt: byteSalt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 10000,
                        numBytesRequested: 16)
                        );

                encryptedSalt = Convert.ToBase64String(byteSalt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new KeyValuePair<string, string>(encryptedSalt, encryptedResult);
        }

        public static KeyValuePair<string, string> DecryptSaltPassword(string salt, string password)
        {
            byte[] byteSalt = Convert.FromBase64String(salt);
            string encryptedResult = string.Empty;
            string encryptedSalt = string.Empty;

            try
            {
                encryptedResult = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: password,
                        salt: byteSalt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 10000,
                        numBytesRequested: 16)
                        );

                encryptedSalt = Convert.ToBase64String(byteSalt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new KeyValuePair<string, string>(encryptedSalt, encryptedResult);
        }
    }
}