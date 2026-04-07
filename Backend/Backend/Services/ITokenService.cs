using Backend.Models.DTOs;

namespace Backend.Services
{
    public interface ITokenService
    {
        string GenerateJWTToken(User user);
        string GenerateRefreshToken();
    }
}
