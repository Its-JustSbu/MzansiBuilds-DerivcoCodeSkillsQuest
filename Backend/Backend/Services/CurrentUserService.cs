using Backend.Models.DTOs;
using System.Security.Claims;

namespace Backend.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public User GetUserDetails()
        {
            var user = new User();
            try
            {
                var identity = ClaimsPrincipal.Current!.Identities.First();
                user.Username = identity.Claims.First(c => c.Type == "Email").Value;
                user.Id = int.Parse(identity.Claims.First(c => c.Type == "NameIdentifier").Value);

                return user;
            }
            catch (Exception)
            {
                return user;
            }
        }
    }
}
