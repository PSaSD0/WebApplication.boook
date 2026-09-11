using Microsoft.EntityFrameworkCore;
using WebApplication.boook.Models;

namespace WebApplication.boook.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<GuestBookEntry> GuestBookEntries { get; set; }
    }
}