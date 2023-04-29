using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
    public class ExceptionLog
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public string ErrorMessage { get; set; }
        public string ExceptionType { get; set; }
        public string Notes { get; set; }
        public string StackTrace { get; set;}
        public DateTime CreatedDate { get; set; }
    }
}
