namespace XInstallBotProfile.Service.AdminPanelService.Models.Request
{
    public class GetStatisticXInstallAppRequest
    {
        public int UserId { get; set; }
        public DateTime? StartDate { get; set; }  // Дата начала
        public DateTime? EndDate { get; set; }    // Дата окончания
    }
}
