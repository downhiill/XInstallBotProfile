using XInstallBotProfile.Models;

namespace XInstallBotProfile.Service.AdminPanelService.Models.Response
{
    public class GetStatisticResponse
    {
        public List<UserStatistic> UserStatistics { get; set; }
        public StatisticTotal Total { get; set; }
        public StatisticAverages Averages { get; set; }
    }
}
