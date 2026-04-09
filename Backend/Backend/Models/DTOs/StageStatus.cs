using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    public class StageStatus
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string? Name { get; set; }
    }
}
