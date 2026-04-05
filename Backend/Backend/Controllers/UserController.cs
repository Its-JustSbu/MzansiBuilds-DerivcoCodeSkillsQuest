using Backend.Models.DTOs;
using Backend.Models.Views;
using Backend.Repositories.DataRepository;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IDataRepository dataRepository, TokenService tokenService) : ControllerBase
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
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginView user)
        {
            try
            {
                var currentUser = await dataRepository.GetByAsync<User>(x => x.EmailAddress == user.Username || x.Username == user.Username);

                if (currentUser.FirstOrDefault() == null || currentUser.FirstOrDefault()!.VerifyPassword(user.Password!)) return BadRequest("Invalid login details");

                var refreshToken = new RefreshToken(tokenService.GenerateRefreshToken(), currentUser.FirstOrDefault()!.Id);

                await dataRepository.AddAsync(refreshToken);
                await dataRepository.SaveChangesAsync();

                return Ok(new { message = "Login successful", token = tokenService.GenerateJWTToken(currentUser.FirstOrDefault()!), refreshToken = refreshToken.Token });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }

        // POST: api/User/logout
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);
                var refreshToken = await dataRepository.GetByAsync<RefreshToken>(x => x.Id == userId);

                if (refreshToken.FirstOrDefault() == null) return Unauthorized("Action Impossible!");

                refreshToken.FirstOrDefault()!.RevokeToken();
                dataRepository.Update(refreshToken.FirstOrDefault()!);
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
        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await dataRepository.GetByAsync<User>(x => x.Id == id);

                if (user == null) return NotFound("User not found");

                return Ok(user);
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
        public async Task<IActionResult> UpdateUser(UserView user)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);
                var userEmail = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)!.Value;
                var currentUser = await dataRepository.GetByAsync<User>(x => x.Id == userId || x.EmailAddress == userEmail);

                if (currentUser.FirstOrDefault() == null) return NotFound("User not found");

                currentUser.FirstOrDefault()!.Name = user.Name;
                currentUser.FirstOrDefault()!.Surname = user.Surname;
                currentUser.FirstOrDefault()!.EmailAddress = user.EmailAddress;
                currentUser.FirstOrDefault()!.Username = user.Username;

                dataRepository.Update(currentUser.FirstOrDefault()!);
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
                var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);
                var userEmail = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)!.Value;
                var currentUser = await dataRepository.GetByAsync<User>(x => x.Id == userId || x.EmailAddress == userEmail);

                if (currentUser.FirstOrDefault() == null) return NotFound("User not found");

                if (currentUser.FirstOrDefault()!.VerifyPassword(passwordView.OldPassword!)) return BadRequest("Old password is incorrect");

                if (passwordView.NewPassword != passwordView.ConfirmPassword) return BadRequest("New password and confirm password do not match");

                currentUser.FirstOrDefault()!.UpdatePassword(passwordView.NewPassword!);
                dataRepository.Update(currentUser.FirstOrDefault()!);
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
        [HttpPut("RefreshToken/{Token}")]
        public async Task<IActionResult> RefreshToken(string Token)
        {
            try
            {
                var refreshToken = dataRepository.GetOneByAsync<RefreshToken>(x => x.Token == Token).Include(x => x.User).FirstOrDefault();

                if (refreshToken == null) return Unauthorized("Invalid refresh token");

                if (!refreshToken.IsValid) return BadRequest("Refresh token is revoked");

                if (refreshToken.ExpiredAt < DateTime.UtcNow)
                {
                    refreshToken.RevokeToken();

                    dataRepository.Update(refreshToken);
                    await dataRepository.SaveChangesAsync();

                    return BadRequest("Refresh token is expired");
                }

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
                var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);
                var userEmail = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)!.Value;
                var currentUser = await dataRepository.GetByAsync<User>(x => x.Id == userId || x.EmailAddress == userEmail);

                if (currentUser.FirstOrDefault() == null) return NotFound("User not found");

                dataRepository.Delete(currentUser.FirstOrDefault()!);
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