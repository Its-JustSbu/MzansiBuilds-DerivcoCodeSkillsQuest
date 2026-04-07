using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DTOs
{
    public class ProjectStage
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(7)]
        public int StageNumber { get; set; }
        [Required, MaxLength(150)]
        public string? StageTitle { get; set; }
        [DataType(DataType.Date)]
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
        [ForeignKey(nameof(StageStatusId))]
        public int StageStatusId { get; set; }
        public StageStatus? StageStatus { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        public void UpdateProjectStage(int stageNumber, string stageTitle, int stageStatusId)
        {
            StageNumber = stageNumber;
            StageTitle = stageTitle;
            StageStatusId = stageStatusId;
            ModifiedAt = DateTime.Now;
        }
    }
}
