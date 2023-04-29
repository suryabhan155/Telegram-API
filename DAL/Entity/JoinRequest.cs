using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
    public class JoinRequest
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long userId { get; set; }
        [Required]
        public long channelId { get; set; }
        public string? link { get; set; }
        public bool Approved { get; set; }
    }
}
