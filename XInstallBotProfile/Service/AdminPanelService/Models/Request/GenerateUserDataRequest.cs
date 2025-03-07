namespace XInstallBotProfile.Service.AdminPanelService.Models.Request
{
    public class GenerateUserDataRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
