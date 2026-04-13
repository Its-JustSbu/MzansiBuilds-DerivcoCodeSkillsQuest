using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Views
{
    public class UserView
    {
        [Required, MaxLength(255)]
        public string? Name { get; set; }
        [Required, MaxLength(255)]
        public string? Surname { get; set; }
        [Required, MaxLength(255), EmailAddress]
        public string? EmailAddress { get; set; }
        [Required, MaxLength(255)]
        public string? Username { get; set; }
        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required, Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
