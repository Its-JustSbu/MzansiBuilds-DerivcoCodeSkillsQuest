using Backend.Models.Views;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DTOs
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(255)]
        public string? Name { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<ProjectStage>? Stages { get; set; } = [];
        public ICollection<Support>? Support { get; set; } = [];
        public ICollection<Collaboration>? Collaborations { get; set; } = [];
        public ICollection<Comment>? Comments { get; set; } = [];
        public Project() { }
        public Project(CreateProjectView createProject)
        {
            Name = createProject.Name;
            Description = createProject.Description;

            if (createProject.Stages is not null)
            {
                Stages = [.. createProject.Stages.Select(s => new ProjectStage
                {
                    StageNumber = s.StageNumber,
                    StageTitle = s.StageTitle
                })];
            }

            if (createProject.Support is not null)
            {
                Support = [.. createProject.Support.Select(s => new Support
                {
                    Description = s.Description,
                    SupportType = s.SupportType
                })];
            }
        }
        public void UpdateProject(string name, string description, List<ProjectStage> stage, List<Support> support)
        {
            Name = name;
            Description = description;
            if (stage.Count > 0)
            {
                for (int i = 0; i < stage.Count; i++)
                {
                    stage[i].StageStatus = null;
                }
                Stages = stage;
            }
            if (support.Count > 0)
            {
                for (int i = 0; i < support.Count; i++)
                {
                    support[i].SupportType = null;
                }
                Support = support;
            }
        }
    }
}
