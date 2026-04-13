using Backend.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Views
{
    public class CreateProjectView
    {
        [Required, DataType(DataType.Text)]
        public string? Name { get; set; }
        [Required, DataType(DataType.Text)]
        public string? Description { get; set; }
        public List<StagesView>? Stages { get; set; }
        public List<SupportView>? Support { get; set; }
    }
    public class StagesView
    {
        [Required]
        public int StageNumber { get; set; }
        [Required, DataType(DataType.Text)]
        public string? StageTitle { get; set; }
        public StageStatus? StageStatus { get; set; }
        public List<MilestoneView>? Milestones { get; set; }

    }
    public class MilestoneView
    {
        [Required, DataType(DataType.Text)]
        public string? Description { get; set; }
    }
    public class  SupportView
    {
        [Required, DataType(DataType.Text)]
        public string? Description { get; set; }
        public SupportType? SupportType { get; set; }
    }
}
