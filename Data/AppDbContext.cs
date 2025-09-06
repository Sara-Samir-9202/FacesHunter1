
// Data/AppDbContext.cs

using FacesHunter.Models;
using Microsoft.EntityFrameworkCore;

namespace FacesHunter.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Person> Persons { get; set; } = null!;
        public DbSet<MissingReport> MissingReports { get; set; } 
        public DbSet<SuccessStory> SuccessStories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

       



    }
}





