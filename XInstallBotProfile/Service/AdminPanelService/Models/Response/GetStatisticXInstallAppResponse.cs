using XInstallBotProfile.Models;

namespace XInstallBotProfile.Service.AdminPanelService.Models.Response
{
    public class GetStatisticXInstallAppResponse
    {
        public List<XInstallAppUserStat> UserStatistics { get; set; }
        public StatisticTotalXInstall Total { get; set; }
        public long TotalAllTime { get; set; }
    }
}
