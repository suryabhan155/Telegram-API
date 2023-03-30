using System.ComponentModel.DataAnnotations;

namespace TelegramApi.Modals
{
    public class ChannelViewModel
    {
        public int channelId { get; set; }
        public string title { get; set; }
        public string about { get; set; }
        public bool broadcast { get; set; }
        public bool megagroup { get; set; }
        public bool for_import { get; set; }
        public string geo_point { get; set; }
        public string address { get; set; }
    }
}
