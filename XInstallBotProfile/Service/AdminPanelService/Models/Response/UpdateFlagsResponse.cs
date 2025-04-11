namespace XInstallBotProfile.Service.AdminPanelService.Models.Response
{
    public class UpdateFlagsResponse
    {
        public int Id { get; set; }
        public bool IsDsp { get; set; }
        public bool IsDspInApp { get; set; }
        public bool IsDspBanner { get; set; }
        public bool IsXInstallApp { get; set; }
    }
}
