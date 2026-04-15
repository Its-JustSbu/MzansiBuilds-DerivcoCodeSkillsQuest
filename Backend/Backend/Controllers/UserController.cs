using Backend.Models.DTOs;
using Backend.Models.Views;
using Backend.Repositories.DataRepository;
using Backend.Services.CurrentUserService;
using Backend.Services.TokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Web;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IDataRepository dataRepository, ITokenService tokenService, ICurrentUserService currentUserService) : ControllerBase
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

                return CreatedAtAction(nameof(CreateUser), new { id = newUser.Id }, newUser);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }

        // POST: api/User/login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginView user)
        {
            try
            {
                var currentUser = dataRepository.GetOneBy<User>(x => x.EmailAddress == user.Username || x.Username == user.Username).FirstOrDefault();

                if (!currentUser!.VerifyPassword(user.Password!)) return BadRequest("Invalid login details");

                var refreshToken = new RefreshToken(tokenService.GenerateRefreshToken(), currentUser!.Id);

                await dataRepository.AddAsync(refreshToken);
                await dataRepository.SaveChangesAsync();

                var token = tokenService.GenerateJWTToken(currentUser!);

                return Ok(new { message = "Login successful", token, refreshToken = refreshToken.Token });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }

        // POST: api/User/logout
        [Authorize]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var user = currentUserService.GetUserDetails();
                var refreshToken = dataRepository.GetOneBy<RefreshToken>(x => x.Id == user.Id).FirstOrDefault();

                if (refreshToken == null) return Unauthorized("Action Impossible!");

                refreshToken.RevokeToken();
                dataRepository.Update(refreshToken);
                await dataRepository.SaveChangesAsync();

                return Ok(new { message = "Logout successful" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }

        // GET: api/User/{id}
        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var user = currentUserService.GetUserDetails();
                var currentUser = dataRepository.GetBy<User>(x => x.Id == user.Id || x.EmailAddress == user.EmailAddress).FirstOrDefault();

                if (currentUser == null) return NotFound("User not found");

                return Ok(currentUser);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // PUT: api/User
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UpdateUserView? user)
        {
            try
            {
                var parsedUser = currentUserService.GetUserDetails();
                var currentUser = dataRepository.GetOneBy<User>(x => x.Id == parsedUser.Id || x.EmailAddress == parsedUser.EmailAddress).FirstOrDefault();

                if (currentUser == null) return BadRequest("Impossible Action");

                currentUser.UpdateUser(user!.Name!, user.Surname!, user.Username!);
                dataRepository.Update(currentUser);
                await dataRepository.SaveChangesAsync();

                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }

        // PUT: api/User/Password
        [Authorize]
        [HttpPut("Password")]
        public async Task<IActionResult> Password(UpdatePasswordView passwordView)
        {
            try
            {
                var user = currentUserService.GetUserDetails();
                var currentUser = dataRepository.GetOneBy<User>(x => x.Id == user.Id || x.EmailAddress == user.EmailAddress).FirstOrDefault();

                if (currentUser == null) return NotFound("User not found");

                if (!currentUser.VerifyPassword(passwordView.OldPassword!)) return BadRequest("Old password is incorrect");

                if (passwordView.NewPassword != passwordView.ConfirmPassword) return BadRequest("New password and confirm password do not match");

                currentUser.UpdatePassword(passwordView.NewPassword!);
                dataRepository.Update(currentUser);
                await dataRepository.SaveChangesAsync();

                return Ok(new { message = "Password updated successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }

        // PUT: api/User/RefreshToken/{Token}
        [HttpGet("RefreshToken/{Token}")]
        public async Task<IActionResult> RefreshToken(string Token)
        {
            try
            {
                Token = HttpUtility.UrlDecode(Token);
                var refreshToken = dataRepository.GetOneBy<RefreshToken>(x => x.Token == Token.Replace(" ", "+") && x.IsValid == true).FirstOrDefault();

                if (refreshToken == null) return Unauthorized("Invalid refresh token");

                if (refreshToken.ExpiredAt < DateTime.UtcNow)
                {
                    refreshToken.RevokeToken();

                    dataRepository.Update(refreshToken);
                    await dataRepository.SaveChangesAsync();

                    return BadRequest("Refresh token is expired");
                }
                
                refreshToken.RevokeToken();
                dataRepository.Update(refreshToken);
                var newRefreshToken = new RefreshToken(tokenService.GenerateRefreshToken(), refreshToken.UserId);
                await dataRepository.AddAsync(newRefreshToken);
                await dataRepository.SaveChangesAsync();

                return Ok(new { message = "Token refreshed successfully", token = tokenService.GenerateJWTToken(refreshToken.User!), refreshToken = newRefreshToken.Token });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }

        // DELETE: api/User
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            try
            {
                var user = currentUserService.GetUserDetails();
                var currentUser = dataRepository.GetOneBy<User>(x => x.Id == user.Id || x.EmailAddress == user.EmailAddress).FirstOrDefault();

                if (currentUser == null) return NotFound("User not found");

                dataRepository.Delete(currentUser);
                await dataRepository.SaveChangesAsync();

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
    }
}