using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
    public class GeneralContext : DbContext
    {
        public GeneralContext(DbContextOptions<GeneralContext> options):base(options)
        {
            
        }

        public GeneralContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Register>().HasData(
            //    new Register { id = 1, fname = "super", lname = "admin", email = "admin@gmail.com", contactno = "7777777777", role = "superAdmin", description = "", status = "Active", createddate = DateTime.UtcNow, field1 = "", field2 = "" }
            //);
        }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<JoinRequest> JoinRequests { get; set; }
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
    }
}
