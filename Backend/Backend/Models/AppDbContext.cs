using Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        DbSet<User> Users { get; set; }
        DbSet<Project> Projects { get; set; }
        DbSet<Collaboration> Collaborations { get; set; }
        DbSet<Comment> Comments { get; set; }
        DbSet<Milestone> Milestones { get; set; }
        DbSet<ProjectStage> ProjectStages { get; set; }
        DbSet<StageStatus> StageStatuses { get; set; }
        DbSet<RequestStatus> RequestStatuses { get; set; }
        DbSet<CollaboratorType> CollaboratorTypes { get; set; }
        DbSet<Support> Supports { get; set; }
        DbSet<SupportType> SupportTypes { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.EmailAddress).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();
            });

            modelBuilder.Entity<RefreshToken>(token =>
            {
                token.HasOne(t => t.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Collaboration>(collaboration =>
            {
                collaboration.HasOne(c => c.User)
                .WithMany(u => u.Collaborations)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                collaboration.HasOne(c => c.Project)
                .WithMany(c => c.Collaborations)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Comment>(comment =>
            {
                comment.HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                comment.HasOne(c => c.Project)
                .WithMany(c => c.Comments)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Milestone>(milestone =>
            {
                milestone.HasOne(m => m.ProjectStage)
                .WithMany(p => p.Milestones)
                .HasForeignKey(m => m.ProjectStageId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProjectStage>(projectStage =>
            {
                projectStage.HasOne(p => p.Project)
                .WithMany(p => p.Stages)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Support>(support =>
            {
                support.HasOne(s => s.Project)
                .WithMany(p => p.Support)
                .HasForeignKey(s => s.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SupportType>().HasData(new List<SupportType>
            {
                new() { Id = 1, Name = "Financial" },
                new() { Id = 2, Name = "Developers" },
                new() { Id = 3, Name = "UI/UX Designers" },
                new() { Id = 4, Name = "Analysts" },
                new() { Id = 5, Name = "Other" },
            });

            modelBuilder.Entity<StageStatus>().HasData(new List<StageStatus>
            {
                new() { Id = 1, Name = "Not Started" },
                new() { Id = 2, Name = "In Progress" },
                new() { Id = 3, Name = "Completed" },
            });

            modelBuilder.Entity<CollaboratorType>().HasData(new List<CollaboratorType>
            {
                new() { Id = 1, Name = "Owner" },
                new() { Id = 2, Name = "Contributor" },
                new() { Id = 3, Name = "Viewer" },
            });

            modelBuilder.Entity<RequestStatus>().HasData(new List<RequestStatus>
            {
                new() { Id = 1, Name = "Pending" },
                new() { Id = 2, Name = "Approved" },
                new() { Id = 3, Name = "Rejected" },
            });
        }
    }
}
