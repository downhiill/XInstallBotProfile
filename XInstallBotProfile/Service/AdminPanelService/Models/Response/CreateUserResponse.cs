namespace XInstallBotProfile.Service.AdminPanelService.Models.Response
{
    public class CreateUserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDsp { get; set; }
        public bool? IsDspInApp { get; set; }
        public bool? IsDspBanner { get; set;}

    }
}
