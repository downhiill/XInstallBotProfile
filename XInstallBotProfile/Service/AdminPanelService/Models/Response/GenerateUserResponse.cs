namespace XInstallBotProfile.Service.AdminPanelService.Models.Response
{
    public class GenerateUserResponse
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public string Nickname { get; set; }
    }
}
