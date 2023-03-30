using TL;

namespace TelegramApi.Modals
{
    public class UserModel
    {
        public long Id { get; set; }
        public long access_hash { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? username { get; set; }
        public string? phone { get; set; }
        public UserProfilePhoto? photo { get; set; }
        public string? status { get; set; }
        public int bot_info_version { get; set; }
        public RestrictionReason[]? restriction_reason { get; set; }
        public string? bot_inline_placeholder { get; set; }
        public string? lang_code { get; set; }
        public EmojiStatus? emoji_status { get; set; }
        public bool IsActive { get; set; }
        public TimeSpan LastSeenAgo { get; set; }
    }
}
