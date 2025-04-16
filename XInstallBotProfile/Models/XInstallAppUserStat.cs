using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace XInstallBotProfile.Models
{
    public class XInstallAppUserStat
    {
        public int UserId { get; set; }
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public long Total { get; set; }
        public string AppLink { get; set; }
        public string AppName { get; set; }
        public string Region { get; set; }

        // Храним как сериализованную строку в БД
        public string? KeywordsSerialized { get; set; }

        [NotMapped]
        public List<string> Keywords
        {
            get => string.IsNullOrEmpty(KeywordsSerialized)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(KeywordsSerialized) ?? new List<string>();

            set => KeywordsSerialized = JsonConvert.SerializeObject(value);
        }

        public long TotalInstall { get; set; }
        public decimal Complited { get; set; }
    }
}
