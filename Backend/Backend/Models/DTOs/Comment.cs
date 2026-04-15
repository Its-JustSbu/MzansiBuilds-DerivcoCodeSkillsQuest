using Backend.Models.Views;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DTOs
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; }
        public User? User { get; set; }
        [MaxLength(255)]
        public string? Title { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Comment() { }
        public Comment(CreateCommentView createCommentView, Project project, User user) 
        {
            if (project != null)
            {
                ProjectId = project.Id;
            }
            if (user != null)
            {
                UserId = user.Id;
            }
            Title = createCommentView.Title;
            Description = createCommentView.Description;
        }
        public void UpdateComment(string title, string description)
        {
            Title = title;
            Description = description;
        }
    }
}
