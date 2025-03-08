namespace XInstallBotProfile.Service.AdminPanelService.Models.Request
{
    public class SaveUserDataRequest
    {
        public int Id { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string login { get; set; }
    }
}
