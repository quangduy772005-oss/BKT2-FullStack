using Microsoft.EntityFrameworkCore;
using PCM.Api.Models;

namespace PCM.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Registration> Registrations => Set<Registration>();
    public DbSet<User> Users => Set<User>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Registration>()
            .HasOne(r => r.Member)
            .WithMany(m => m.Registrations)
            .HasForeignKey(r => r.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Registration>()
            .HasOne(r => r.Activity)
            .WithMany(a => a.Registrations)
            .HasForeignKey(r => r.ActivityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
