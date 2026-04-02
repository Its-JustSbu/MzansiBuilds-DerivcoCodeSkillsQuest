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
    }
}
