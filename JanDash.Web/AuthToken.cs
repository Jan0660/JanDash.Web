using System;
using System.Security.Cryptography;

namespace JanDash
{
    public static class AuthToken
    {
        public static readonly RNGCryptoServiceProvider Random = new();

        public static string Generate()
        {
            var bytes = new byte[32];
            Random.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}