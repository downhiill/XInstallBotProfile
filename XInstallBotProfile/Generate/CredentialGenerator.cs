namespace XInstallBotProfile.Generate
{
    public class CredentialGenerator
    {
        private static Random _random = new Random();

        public static string GenerateLogin()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(0, 8).Select(_ => chars[_random.Next(chars.Length)]).ToArray());
        }

        public static string GeneratePassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            return new string(Enumerable.Range(0, 12).Select(_ => chars[_random.Next(chars.Length)]).ToArray());
        }
    }

}
