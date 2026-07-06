using System;
using BCrypt.Net;

namespace TestLogin
{
    class Program
    {
        static void Main(string[] args)
        {
            string password = "123";
            string hash = "$2a$12$3tfSUK9qvpHSoWNR.OEKlOPaHdpsYokwovZM2kBYoxBDecqKAWVji";
            string salt = "0eYnrPl71X9P9vh8r+fyGA2y8Pvoh9614uBMWGkngr8=";

            try
            {
                var saltedPassword = salt + password;
                bool isMatch = BCrypt.Net.BCrypt.Verify(saltedPassword, hash);
                Console.WriteLine($"Is Match: {isMatch}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
