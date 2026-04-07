using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DTOs
{
    public class RefreshToken(string token, int userId)
    {
        [Key]
        public int Id { get; set; }
        public string? Token { get; set; } = token;
        public DateTime ExpiredAt { get; set; } = DateTime.Now.AddDays(7);
        public bool IsValid { get; set; } = true;
        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; } = userId;
        public User? User { get; set; }
        public void RevokeToken()
        {
            IsValid = false;
        }
    }
}
