using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    public class CollaboratorType
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(255)]
        public string? Name { get; set; }
    }
}
