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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
