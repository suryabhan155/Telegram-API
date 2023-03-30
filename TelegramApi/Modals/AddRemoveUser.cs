using TL;

namespace TelegramApi.Modals
{
    public class AddUser
    {
        public long id { get; set; }
        public string? rdoPhoneNumber { get; set; }
        public string? rdoUsername { get; set; }
        public string? username { get; set; }
    }
    public class RemoveUser
    {
        public long id { get; set; }
        public long userid { get; set; }
        public long access_hash { get; set; }
    }
}
