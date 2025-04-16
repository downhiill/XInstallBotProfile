namespace XInstallBotProfile.Service.AdminPanelService.Models.Request
{
    public class CreateUserRecordXInstallAppRequest
    {
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public long Total { get; set; }
        public string AppLink { get; set; }
        public string AppName { get; set; }
        public string Region { get; set; }
        public List<string> Keywords { get; set; } = new List<string>();
        public long TotalInstall { get; set; }
        public decimal Complited { get; set; }
    }
}
