﻿namespace XInstallBotProfile.Service.AdminPanelService.Models.Request
{
    public class UpdateStatisticRequest
    {
        public long? Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
