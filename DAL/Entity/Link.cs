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
        [Required]
        public string link { get; set; }
        public bool IsOnceClicked { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
