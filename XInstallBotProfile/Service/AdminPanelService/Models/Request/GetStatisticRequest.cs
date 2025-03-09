namespace XInstallBotProfile.Service.AdminPanelService.Models.Request
{
    public class GetStatisticRequest
    {
        public int UserId { get; set; }
        
        public bool IsDsp { get; set; }
        public bool IsDspInApp { get; set; }
        public bool IsDspBanner { get; set; }
        public DateTime? StartDate { get; set; }  // Дата начала
        public DateTime? EndDate { get; set; }    // Дата окончания
    }
}
