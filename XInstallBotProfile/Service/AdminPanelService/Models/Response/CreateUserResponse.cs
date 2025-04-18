﻿namespace XInstallBotProfile.Service.AdminPanelService.Models.Response
{
    public class CreateUserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Login { get; set; }
        public List<int> PanelTypes { get; set; } // Изменено на List<int> для возвращения идентификаторов типов панелей

        public bool IsDsp;
        public bool IsDspInApp;
        public bool IsDspBanner;
        public bool IsXInstallApp;
    }
}
