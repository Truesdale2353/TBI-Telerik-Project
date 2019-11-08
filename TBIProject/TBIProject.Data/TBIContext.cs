using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        public DbSet<AuditTrail> AuditTrails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasMany(u => u.Loans)
                .WithOne(l => l.User)
                .HasForeignKey(u => u.UserId);
            builder.Entity<User>()
                .HasMany(a => a.Applicatons)
                .WithOne(u => u.CurrentlyOpenedBy)
                .HasForeignKey(h => h.OperatorId);

            builder.Entity<User>()
                .HasMany(u => u.AuditTrails)
                .WithOne(a => a.User)
                .HasForeignKey(u => u.UserId);
        }

        public Task<int> SaveChangesAsync(User user, CancellationToken cancellationToken = default)
        {
            ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).ToList().ForEach(entry =>
            {
                Audit(entry, user);
            });

            return base.SaveChangesAsync();
        }

        private void Audit(EntityEntry entry, User user)
        {
            foreach (var property in entry.Properties)
            {
                if (!property.IsModified)
                    continue;

                var auditEntry = new AuditTrail
                {
                    Table = entry.Entity.GetType().Name,
                    Column = property.Metadata.Name,
                    OldValue = property.OriginalValue == null ? null : property.OriginalValue.ToString(),
                    NewValue = property.CurrentValue.ToString(),
                    Date = DateTime.UtcNow,
                    User = user,
                    UserId = user.Id
                };

                this.AuditTrails.Add(auditEntry);
            }
        }
    }
}
