namespace TelegramApi.Modals
{
    public class ApproveRejectModel
    {
        public long userId { get; set; }
        public long channelId { get; set; }
        public string? link { get; set; }
        public bool approved { get; set; }
        public bool requested { get; set; }
    }
}
