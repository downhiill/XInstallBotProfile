namespace XInstallBotProfile.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string? JwtToken { get; set; }
        public long ChatId { get; set; }
    }

}
