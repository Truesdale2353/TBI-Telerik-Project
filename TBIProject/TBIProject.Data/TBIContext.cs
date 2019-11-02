using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TBIProject.Data.Models;

namespace TBIProject.Data
{
    public class TBIContext : IdentityDbContext<User>
    {
        public TBIContext(DbContextOptions<TBIContext> options)
            : base(options)
        {
        }

        public DbSet<Loan> Loans { get; set; }

        public DbSet<Application> Applications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasMany(u => u.Loans)
                .WithOne(l => l.User)
                .HasForeignKey(u => u.UserId);
        }
    }
}
