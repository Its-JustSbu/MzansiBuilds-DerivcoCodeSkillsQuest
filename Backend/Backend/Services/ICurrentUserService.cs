using Backend.Models.DTOs;

namespace Backend.Services
{
    public interface ICurrentUserService
    {
        User GetUserDetails();
    }
}
