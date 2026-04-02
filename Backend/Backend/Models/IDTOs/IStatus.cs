using System.ComponentModel.DataAnnotations;

namespace Backend.Models.IDTOs
{
    public interface IStatus
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string? Name { get; set; }
    }
}
