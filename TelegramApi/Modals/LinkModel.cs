using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TelegramApi.Modals
{
    public class LinkModel
    {
        public long Id { get; set; }
        public long channelId { get; set; }
        public string? link { get; set; }
        public string? title { get; set; }
        public int? usage_limit { get; set; }
        public bool request_needed { get; set; }
        public bool legacy_revoke_permanent { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? expire_Date { get; set; }
    }
}
