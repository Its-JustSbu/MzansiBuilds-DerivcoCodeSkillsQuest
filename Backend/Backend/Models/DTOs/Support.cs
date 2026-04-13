using Backend.Models.Views;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DTOs
{
    public class Support
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string? Description { get; set; }
        [ForeignKey(nameof(SupportTypeId))]
        public int SupportTypeId { get; set; }
        public SupportType? SupportType { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime RequestedAt { get; set; } = DateTime.Now;
        public Support() { }
        public Support(int projectId, SupportView support)
        {
            ProjectId = projectId;
            Description = support.Description;
            if (support.SupportType != null)
            {
                SupportTypeId = support.SupportType.Id;
            }
        }
        public void UpdateSupport(string description, int supportTypeId, int projectId)
        {
            Description = description;
            SupportTypeId = supportTypeId;
            ProjectId = projectId;
            RequestedAt = DateTime.Now;
            if (SupportType != null)
            {
                SupportType = null;
            }
        }
    }
}
