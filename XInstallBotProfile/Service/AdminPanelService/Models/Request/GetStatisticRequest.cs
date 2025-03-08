namespace XInstallBotProfile.Service.AdminPanelService.Models.Request
{
    public class GetStatisticRequest
    {
        public int UserId { get; set; }
        public List<FlagInfo> FlagStatus { get; set; }  // Тип панели (DSP, DSP InApp, DSP Banner)
        public DateTime StartDate { get; set; }  // Дата начала
        public DateTime EndDate { get; set; }    // Дата окончания
    }
}
