using Lokallagsportalen.Models;
using Microsoft.EntityFrameworkCore;

namespace Lokallagsportalen.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<City> Cities => Set<City>();
    public DbSet<Parish> Parishes => Set<Parish>();
    public DbSet<Lokallag> Lokallags => Set<Lokallag>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(100);
        });

        // Restrict all the way down: deleting a city/parish/lokallag that
        // still has children fails instead of cascading into events.
        modelBuilder.Entity<Parish>(e =>
        {
            e.Property(p => p.Name).HasMaxLength(100);
            e.HasOne(p => p.City)
                .WithMany(c => c.Parishes)
                .HasForeignKey(p => p.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Lokallag>(e =>
        {
            e.Property(l => l.Name).HasMaxLength(100);
            e.HasOne(l => l.Parish)
                .WithMany(p => p.Lokallags)
                .HasForeignKey(l => l.ParishId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Event>(e =>
        {
            e.Property(ev => ev.Name).HasMaxLength(100);
            e.Property(ev => ev.Description).HasMaxLength(2000);
            e.Property(ev => ev.Location).HasMaxLength(200);
            e.HasOne(ev => ev.Lokallag)
                .WithMany(l => l.Events)
                .HasForeignKey(ev => ev.LokallagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(e =>
        {
            e.Property(u => u.FirstName).HasMaxLength(100);
            e.Property(u => u.LastName).HasMaxLength(100);
            e.Property(u => u.Email).HasMaxLength(254);
            e.Property(u => u.MobileNumber).HasMaxLength(20);
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Registration>(e =>
        {
            e.HasKey(r => new { r.UserId, r.EventId });
        });
    }

    // The parameterless SaveChanges/SaveChangesAsync overloads route through these two.
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        SetTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void SetTimestamps()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Property(x => x.CreatedAt).IsModified = false;
            }
        }
    }
}
