namespace XInstallBotProfile.Service.AdminPanelService.Models.Request
{
    public class UpdateFlagsRequest
    {
        public int Id { get; set; }
        public bool IsDsp { get; set; }
        public bool IsDspInApp { get; set; }
        public bool IsDspBanner { get; set; }
        public bool IsXInstallApp { get; set; }
    }
}
