namespace XInstallBotProfile.Service.AdminPanelService.Models.Response
{
    public class UserModel
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Login { get; set; } // Логин
        public string PasswordHash { get; set; } // Пароль (в хэшированном виде)
        public List<int> PanelTypes { get; set; } // Список доступных типов статистики (ID)
    }
}
