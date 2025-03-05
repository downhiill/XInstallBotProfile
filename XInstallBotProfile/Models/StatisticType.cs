namespace XInstallBotProfile.Models
{
    public class StatisticType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Связь с пользователями
        public ICollection<User> Users { get; set; } // Коллекция пользователей с доступом к этому типу панели
    }
}
