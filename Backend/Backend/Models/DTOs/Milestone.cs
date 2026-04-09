using Backend.Models.IDTOs;
using Backend.Models.Views;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DTOs
{
    public class Milestone
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string? Description { get; set; }
        [ForeignKey(nameof(ProjectStageId))]
        public int ProjectStageId { get; set; }
        public ProjectStage? ProjectStage { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
        public Milestone() { }
        public Milestone(MilestoneView milestone)
        {
            Description = milestone.Description;
        }
        public void UpdateMilestone(string description, int projectStageId)
        {
            ProjectStageId = projectStageId;
            Description = description;
            ModifiedAt = DateTime.Now;
        }
    }
}
