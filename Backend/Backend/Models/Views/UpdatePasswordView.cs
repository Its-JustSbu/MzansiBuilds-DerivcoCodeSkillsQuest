namespace Backend.Models.Views
{
    public class UpdatePasswordView
    {
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
