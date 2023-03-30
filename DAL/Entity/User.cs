using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
    public class User
    {
        [Key]
        public long Id { get; set; }
        public string access_hash { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string phone { get; set; }
        public string photo { get; set; }
        public string status { get; set; }
        public string bot_info_version { get; set; }
        public string restriction_reason { get; set; }
        public string bot_inline_placeholder { get; set; }
        public string lang_code { get; set; }
        public string emoji_status { get; set; }
        public string IsActive { get; set; }
        public string LastSeenAgo { get; set; }
        public string Email { get; set; }

    }
}
