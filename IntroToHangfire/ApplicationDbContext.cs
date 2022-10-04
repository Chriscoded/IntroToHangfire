using IntroToHangfire.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntroToHangfire
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        //public DbSet<Person> People => Set<Person>();
        public DbSet<Person> People { get; set; }
    }
}
