using Backend.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Views
{
    public class CreateProjectView
    {
        [Required, MaxLength(255)]
        public string? Name { get; set; }
        [Required, MaxLength(500)]
        public string? Description { get; set; }
        public List<StagesView>? Stages { get; set; }
        public List<SupportView>? Support { get; set; }
    }
    public class StagesView
    {
        [Required, MaxLength(7)]
        public int StageNumber { get; set; }
        [Required, MaxLength(150)]
        public string? StageTitle { get; set; }
    }
    public class  SupportView
    {
        [Required, MaxLength(255)]
        public string? Description { get; set; }
        public SupportType? SupportType { get; set; }
    }
}
