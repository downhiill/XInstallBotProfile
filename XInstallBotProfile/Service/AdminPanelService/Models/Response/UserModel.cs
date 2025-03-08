namespace XInstallBotProfile.Service.AdminPanelService.Models.Response
{
    public class UserModel
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Login { get; set; } // Логин
        public string PasswordHash { get; set; } // Пароль (в хэшированном виде)
        public bool IsDsp { get; set; } = true;
        public bool IsDspInApp { get; set; } = false;
        public bool IsDspBanner { get; set; } = false;
    }
}
