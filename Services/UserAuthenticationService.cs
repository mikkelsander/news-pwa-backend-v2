
using PWANews.Interfaces;
using PWANews.Models.DomainModels;
using System;
using System.Security.Cryptography;

namespace PWANews.Services
{
    public class UserAuthenticationService : IUserAuthenticationService
    {

        private readonly double TokenTTL = 60;

        public bool AuthenticateUser(User user, string password)
        {
            return ValidatePassword(password, user.Password);
        }

        public void SetOrRefreshAuthenticationToken(User user)
        {
            user.AuthenticationToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            user.AuthenticationTokenExpiration = DateTime.UtcNow.AddMinutes(TokenTTL); 
        }

        public string HashPassword(string password)
        {
            //Create the salt value with a cryptographic PRNG:
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            //Create the Rfc2898DeriveBytes and get the hash value:
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Combine the salt and password bytes for later use:
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            //Turn the combined salt+hash into a string for storage
            string savedPasswordHash = Convert.ToBase64String(hashBytes);

            return savedPasswordHash;
        }


        public bool ValidatePassword(string password, string hashedPassword)
        {

            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] hash = pbkdf2.GetBytes(20);

            /* Compare the results */
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
