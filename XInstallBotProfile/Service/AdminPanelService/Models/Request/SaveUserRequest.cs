namespace XInstallBotProfile.Service.AdminPanelService.Models.Request
{
    public class SaveUserRequest
    {
        public int Id { get; set; }
        public UpdateUsernameRequest UsernameRequest { get; set; }
        public UpdateFlagsRequest FlagsRequest { get; set; }
    }
}
