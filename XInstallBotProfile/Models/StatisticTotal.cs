namespace XInstallBotProfile.Models
{
    public class StatisticTotal
    {
        public long Total { get; set; }
        public long Ack { get; set; }
        public long Win { get; set; }
        public long ImpsCount { get; set; }
        public long ClicksCount { get; set; }
        public long StartsCount { get; set; }
        public decimal ShowRate { get; set; }
        public long CompletesCount { get; set; }
        public decimal Ctr { get; set; }
        public decimal Vtr { get; set; }
    }
}
