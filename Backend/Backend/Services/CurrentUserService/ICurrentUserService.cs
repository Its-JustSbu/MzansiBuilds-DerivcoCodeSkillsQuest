using Backend.Models.DTOs;

namespace Backend.Services.CurrentUserService
{
    public interface ICurrentUserService
    {
        User GetUserDetails();
    }
}
