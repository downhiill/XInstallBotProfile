namespace XInstallBotProfile.Service.AdminPanelService.Models.Request
{
    public class CreateUserRequest
    {
        public int? UserId { get; set; }
        public string? Username { get; set; } // Можно не передавать, тогда ник будет пустым
        public string? Login { get; set; }
        public string? Password { get; set; }
    }
}
