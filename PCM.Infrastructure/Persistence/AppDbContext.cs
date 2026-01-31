using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PCM.Core.Entities;
using System.Reflection;

namespace PCM.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    #region DbSets - Identity & Admin
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<News> News => Set<News>();
    public DbSet<NewsImage> NewsImages => Set<NewsImage>();
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Registration> Registrations => Set<Registration>();
    #endregion

    #region DbSets - Members & Wallet
    public DbSet<Member> Members => Set<Member>();
    public DbSet<TransactionCategory> TransactionCategories => Set<TransactionCategory>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
    public DbSet<DepositRequest> DepositRequests => Set<DepositRequest>();
    #endregion

    #region DbSets - Bookings
    public DbSet<Court> Courts => Set<Court>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<RecurringBooking> RecurringBookings => Set<RecurringBooking>();
    #endregion

    #region DbSets - Tournaments & Matches
    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<TournamentParticipant> TournamentParticipants => Set<TournamentParticipant>();
    public DbSet<TournamentMatch> TournamentMatches => Set<TournamentMatch>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<MiniGameResult> MiniGameResults => Set<MiniGameResult>();
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Identity tables with custom prefix (using MSSV: 025 as example)
        // You can change this based on your MSSV
        modelBuilder.Entity<AppUser>().ToTable("AspNetUsers");
        modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");

        // Configure entities
        ConfigureMemberEntity(modelBuilder);
        ConfigureBookingEntity(modelBuilder);
        ConfigureWalletEntity(modelBuilder);
        ConfigureTournamentEntity(modelBuilder);
        ConfigureMatchEntity(modelBuilder);
        ConfigureNewsEntity(modelBuilder);
        ConfigureClubActivityEntity(modelBuilder);
        ConfigureRefreshTokenEntity(modelBuilder);

        // Seed default data
        SeedData(modelBuilder);
    }

    private void ConfigureMemberEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsRequired();
            
            entity.Property(e => e.Email)
                .HasMaxLength(255);
            
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20);
            
            entity.Property(e => e.WalletBalance)
                .HasPrecision(18, 2);
            
            entity.Property(e => e.RankELO)
                .HasPrecision(10, 2);

            entity.Property(e => e.RowVersion)
                .IsRowVersion();

            entity.HasOne(e => e.User)
                .WithOne(u => u.Member)
                .HasForeignKey<Member>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.UserId)
                .IsUnique();
        });
    }

    private void ConfigureBookingEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Court>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PricePerHour).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2);

            entity.Property(e => e.RowVersion)
                .IsRowVersion(); // Optimistic locking

            entity.HasOne(e => e.Court)
                .WithMany(c => c.Bookings)
                .HasForeignKey(e => e.CourtId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Member)
                .WithMany(m => m.Bookings)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prevent overlapping bookings on same court
            entity.HasIndex(e => new { e.CourtId, e.StartTime, e.EndTime })
                .HasDatabaseName("IX_Court_TimeSlot");
        });

        modelBuilder.Entity<RecurringBooking>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.PricePerSession)
                .HasPrecision(10, 2);

            entity.HasOne(e => e.Member)
                .WithMany()
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Court)
                .WithMany()
                .HasForeignKey(e => e.CourtId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureWalletEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransactionCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<WalletTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2);

            entity.Property(e => e.ReferenceId)
                .HasMaxLength(100);

            entity.HasOne(e => e.Member)
                .WithMany(m => m.Transactions)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => new { e.MemberId, e.CreatedDate });
        });

        modelBuilder.Entity<DepositRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.RequestAmount)
                .HasPrecision(18, 2);

            entity.HasOne(e => e.Member)
                .WithMany(m => m.DepositRequests)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.MemberId, e.CreatedDate });
        });
    }

    private void ConfigureTournamentEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.EntryFee).HasPrecision(10, 2);
            entity.Property(e => e.PrizePool).HasPrecision(18, 2);
        });

        modelBuilder.Entity<TournamentParticipant>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Tournament)
                .WithMany(t => t.Participants)
                .HasForeignKey(e => e.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Member)
                .WithMany(m => m.TournamentParticipations)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.TournamentId, e.MemberId })
                .IsUnique();
        });

        modelBuilder.Entity<TournamentMatch>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Tournament)
                .WithMany(t => t.Matches)
                .HasForeignKey(e => e.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Match)
                .WithMany()
                .HasForeignKey(e => e.MatchId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.NextMatch)
                .WithMany()
                .HasForeignKey(e => e.NextMatchId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private void ConfigureMatchEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Team1Player1)
                .WithMany(m => m.MatchesAsTeam1Player1)
                .HasForeignKey(e => e.Team1Player1Id)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Team1Player2)
                .WithMany(m => m.MatchesAsTeam1Player2)
                .HasForeignKey(e => e.Team1Player2Id)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Team2Player1)
                .WithMany(m => m.MatchesAsTeam2Player1)
                .HasForeignKey(e => e.Team2Player1Id)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Team2Player2)
                .WithMany(m => m.MatchesAsTeam2Player2)
                .HasForeignKey(e => e.Team2Player2Id)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Tournament)
                .WithMany()
                .HasForeignKey(e => e.TournamentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<MiniGameResult>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Match)
                .WithMany()
                .HasForeignKey(e => e.MatchId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureNewsEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<NewsImage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.News)
                .WithMany(n => n.Images)
                .HasForeignKey(e => e.NewsId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureClubActivityEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Club>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Location).HasMaxLength(500);

            entity.HasOne(e => e.Club)
                .WithMany(c => c.Activities)
                .HasForeignKey(e => e.ClubId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Member)
                .WithMany(m => m.Registrations)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Activity)
                .WithMany(a => a.Registrations)
                .HasForeignKey(e => e.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Member>()
            .HasOne(m => m.Club)
            .WithMany(c => c.Members)
            .HasForeignKey(m => m.ClubId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private void ConfigureRefreshTokenEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasOne(e => e.AppUser)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed transaction categories
        modelBuilder.Entity<TransactionCategory>().HasData(
            new TransactionCategory 
            { 
                Id = 1, 
                Name = "Nạp tiền", 
                Type = Core.Enums.TransactionType.Deposit,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            },
            new TransactionCategory 
            { 
                Id = 2, 
                Name = "Phí sân", 
                Type = Core.Enums.TransactionType.PayBooking,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            },
            new TransactionCategory 
            { 
                Id = 3, 
                Name = "Thưởng giải đấu", 
                Type = Core.Enums.TransactionType.ReceivePrize,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            },
            new TransactionCategory 
            { 
                Id = 4, 
                Name = "Hoàn tiền hủy sân", 
                Type = Core.Enums.TransactionType.Refund,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            }
        );

        // Seed sample courts
        modelBuilder.Entity<Court>().HasData(
            new Court 
            { 
                Id = 1, 
                Name = "Sân 1", 
                Description = "Sân cỏ tự nhiên",
                PricePerHour = 150000,
                Capacity = 4,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            },
            new Court 
            { 
                Id = 2, 
                Name = "Sân 2", 
                Description = "Sân nhân tạo",
                PricePerHour = 120000,
                Capacity = 4,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            },
            new Court 
            { 
                Id = 3, 
                Name = "Sân 3", 
                Description = "Sân trong nhà",
                PricePerHour = 180000,
                Capacity = 4,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            }
        );
    }
}
