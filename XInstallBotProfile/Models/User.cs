namespace XInstallBotProfile.Models
{
    public class User
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Role { get; set; } = "User";
        public string Login { get; set; }
        public string? Nickname { get; set; }
        public string PasswordHash { get; set; }
        public string? JwtToken { get; set; }
        public long ChatId { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsDsp { get; set; } = true;
        public bool IsDspInApp { get; set; } = false;
        public bool IsDspBanner { get; set; } = false;

    }

}
