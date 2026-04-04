using Backend.Models.DTOs;
using Backend.Models.Views;
using Backend.Repositories.DataRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IDataRepository dataRepository) : ControllerBase
    {
        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserView user)
        {
            try
            {
                var newUser = new User(user);

                await dataRepository.AddAsync(newUser);
                await dataRepository.SaveChangesAsync();

                return CreatedAtAction("User Successfully Registered", new { id = newUser.Id }, newUser);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }

        // POST: api/User/login

        // POST: api/User/logout

        // GET: api/User/{id}

        // PUT: api/User/{id}

        // PUT: api/User/{id}/password

        // PUT: api/User/{id}/refresh-token

        // DELETE: api/User/{id}
    }
}
