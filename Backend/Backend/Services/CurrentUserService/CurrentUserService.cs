using Backend.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend.Services.CurrentUserService
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public User? GetUserDetails()
        {
            var user = new User();
            try
            {
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                    return null;

                // 1. Extract the raw token string
                var token = authHeader["Bearer ".Length..].Trim();

                // 2. Parse the JWT
                var handler = new JwtSecurityTokenHandler();

                if (handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);
                    user.Username = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    user.Id = int.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");
                }

                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
