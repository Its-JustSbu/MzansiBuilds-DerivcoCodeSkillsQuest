using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Views
{
    public class CreateCommentView
    {
        [Required, MaxLength(255)]
        public string? Title { get; set; }
        [Required, MaxLength(255)]
        public string? Description { get; set; }
    }
}
