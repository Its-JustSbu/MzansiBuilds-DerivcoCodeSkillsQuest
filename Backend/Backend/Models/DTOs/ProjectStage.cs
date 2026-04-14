using Backend.Models.Views;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DTOs
{
    public class ProjectStage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int StageNumber { get; set; }
        [Required, MaxLength(150)]
        public string? StageTitle { get; set; }
        [DataType(DataType.Date)]
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
        public int StageStatusId { get; set; } = 1;
        [ForeignKey(nameof(StageStatusId))]
        public StageStatus? StageStatus { get; set; }
        public int ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }
        public ICollection<Milestone>? Milestones { get; set; } = [];
        public ProjectStage() { }
        public ProjectStage(int projectId, StagesView stage)
        {
            ProjectId = projectId;
            StageNumber = stage.StageNumber;
            StageTitle = stage.StageTitle;
            if (stage.Milestones != null)
            {
                Milestones = [.. stage.Milestones.Select(m => new Milestone(m))];
            }
            if (stage.StageStatus != null)
            {
                StageStatusId = stage.StageStatus.Id;
            }
        }
        public void UpdateProjectStage(int stageNumber, string stageTitle, int stageStatusId, List<Milestone> milestones)
        {
            StageNumber = stageNumber;
            StageTitle = stageTitle;
            StageStatusId = stageStatusId;
            ModifiedAt = DateTime.Now;
            if (StageStatus != null)
            {
                StageStatus = null;
            }
            if (Milestones != null)
            {
                Milestones.Clear();
                foreach (var milestone in milestones)
                {
                    Milestones.Add(milestone);
                }
            }
        }
    }
}
