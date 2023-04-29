using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
    public class Channel//by setting the broadcast flag, Supergroups by setting the megagroup flag=true.
    {//Gigagroups can convert a megagroup into a gigagroup using channels.convertToGigagroup
        [Key]
        public long Id { get; set; }
        public long channelId { get; set; }
        [Required]
        public string? title { get; set; }
        [Required]
        public string? about { get; set; }
        public bool broadcast { get; set; }
        public bool megagroup { get; set; }
        public bool for_import { get; set; }
        public string? geo_point { get; set; }
        public string? address { get; set; }
        public string? username { get; set; }

    }
}
