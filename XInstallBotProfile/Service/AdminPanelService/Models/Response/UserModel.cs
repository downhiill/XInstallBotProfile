namespace XInstallBotProfile.Service.AdminPanelService.Models.Response
{
    public class UserModel
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Flag1 { get; set; }
        public bool? Flag2 { get; set; }
        public bool? Flag3 { get; set; }

    }
}
