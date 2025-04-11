namespace XInstallBotProfile.Service.AdminPanelService.Models.Response
{
    public class GetUserByIdResponse
    {
        public int UserId { get; set; }
        public string username { get; set; }
        public bool IsDsp { get; set; }
        public bool IsDspInApp { get; set; }
        public bool IsDspBanner { get; set; }
        public bool IsXInstallApp { get; set; }
    }
}
