using NanoidDotNet;
using System.Security.Cryptography;


namespace GalaxyPvP.Extensions
{
    public class GenerateExtension
    {
        public static string GenerateRandomCode(int length)
        {
            return Nanoid.Generate("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length);
        }

        public static string GeneratePassword(int length)
        {
            return Nanoid.Generate("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_-+=<>?", length);
        }
    }
}
