using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
    public class Link
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("Channel")]
        public long channelId { get; set; }
        public long linkId { get; set; }
        [Required]
        public string? link { get; set; }
        public string? title { get; set; }
        public int? usage_limit { get; set; }
        public bool request_needed { get; set; }
        public bool legacy_revoke_permanent { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? expire_Date { get; set; }
        public bool revoked { get; set; }
        public bool deleted { get; set; }
    }
}
