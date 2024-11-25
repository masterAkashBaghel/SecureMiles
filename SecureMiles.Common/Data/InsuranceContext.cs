using Microsoft.EntityFrameworkCore;
using SecureMiles.Models;

namespace SecureMiles.Common.Data
{
    public class InsuranceContext : DbContext
    {
        public InsuranceContext(DbContextOptions<InsuranceContext> options) : base(options) { }

        public required DbSet<User> Users { get; set; }
        public required DbSet<Vehicle> Vehicles { get; set; }
        public required DbSet<Policy> Policies { get; set; }
        public required DbSet<Proposal> Proposals { get; set; }
        public required DbSet<Payment> Payments { get; set; }
        public required DbSet<Claim> Claims { get; set; }
        public required DbSet<Document> Documents { get; set; }
        public required DbSet<Notification> Notifications { get; set; }

        public required DbSet<PasswordResetToken> PasswordResetTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Decimal Precision
            modelBuilder.Entity<Claim>()
                .Property(c => c.ClaimAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Policy>()
                .Property(p => p.CoverageAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Policy>()
                .Property(p => p.PremiumAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Proposal>()
                .Property(pr => pr.RequestedCoverage)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.MarketValue)
                .HasColumnType("decimal(18, 2)");

            // Relationships
            // User -> Vehicles (One-to-Many)
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.User)
                .WithMany(u => u.Vehicles)
                .HasForeignKey(v => v.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Proposals (One-to-Many)
            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.User)
                .WithMany(u => u.Proposals)
                .HasForeignKey(p => p.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Policies (One-to-Many)
            modelBuilder.Entity<Policy>()
                .HasOne(p => p.User)
                .WithMany(u => u.Policies)
                .HasForeignKey(p => p.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            // Vehicle -> Policies (One-to-Many)
            modelBuilder.Entity<Policy>()
                .HasOne(p => p.Vehicle)
                .WithMany(v => v.Policies)
                .HasForeignKey(p => p.VehicleID)
                .OnDelete(DeleteBehavior.NoAction);

            // Proposal -> Policy (One-to-One)
            modelBuilder.Entity<Policy>()
                .HasOne(p => p.Proposal)
                .WithOne(pr => pr.Policy)
                .HasForeignKey<Policy>(p => p.ProposalID)
                .OnDelete(DeleteBehavior.Cascade);

            // Policy -> Payments (One-to-Many)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Policy)
                .WithMany(pl => pl.Payments)
                .HasForeignKey(p => p.PolicyID)
                .OnDelete(DeleteBehavior.Cascade);

            // Policy -> Claims (One-to-Many)
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Policy)
                .WithMany(p => p.Claims)
                .HasForeignKey(c => c.PolicyID)
                .OnDelete(DeleteBehavior.Cascade);

            // Proposal -> Documents (One-to-Many)
            modelBuilder.Entity<Document>()
                .HasOne(d => d.Proposal)
                .WithMany(p => p.Documents)
                .HasForeignKey(d => d.ProposalID)
                .OnDelete(DeleteBehavior.NoAction);

            // Claim -> Documents (One-to-Many)
            modelBuilder.Entity<Document>()
                .HasOne(d => d.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClaimID)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Notifications (One-to-Many)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Proposal -> Vehicle (One-to-Many)
            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.Vehicle)
                .WithMany(v => v.Proposals)
                .HasForeignKey(p => p.VehicleID)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
    }
}