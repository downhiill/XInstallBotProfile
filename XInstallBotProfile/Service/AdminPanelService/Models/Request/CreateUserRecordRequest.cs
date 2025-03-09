namespace XInstallBotProfile.Service.AdminPanelService.Models.Request
{
    public class CreateUserRecordRequest
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public long Total { get; set; }
        public long Ack { get; set; }
        public long Win { get; set; }
        public long ImpsCount { get; set; }
        public decimal ShowRate { get; set; }
        public long ClicksCount { get; set; }
        public long Ctr { get; set; }
        public long StartsCount { get; set; }
        public long CompletesCount { get; set; }
        public long Vtr { get; set; }
        public bool IsDsp { get; set; }
        public bool IsDspInApp { get; set; }
        public bool IsDspBanner { get; set; }
    }
}
