using Backend.Controllers;
using Backend.Models.DTOs;
using Backend.Models.Views;
using Backend.Repositories.DataRepository;
using Backend.Services.CurrentUserService;
using Backend.Services.TokenService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.tests.ControllerTests
{
    public class UserControllerTests
    {
        private Mock<ICurrentUserService> mockCurrentUserService;
        private Mock<IDataRepository> mockDataRepository;
        private Mock<ITokenService> mockTokenService;
        private UserController mockUserController;
        public UserControllerTests()
        {
            mockCurrentUserService = new Mock<ICurrentUserService>();
            mockTokenService = new Mock<ITokenService>();
            mockDataRepository = new Mock<IDataRepository>();
            mockUserController = new UserController(mockDataRepository.Object, mockTokenService.Object, mockCurrentUserService.Object);
        }
        [Fact]
        public async Task CreateUser_AddingANewUser_ReturnsCreatedAtAction()
        {
            // Arrange
            var user = new UserView()
            {
                Name = "John",
                Surname = "Doe",
                EmailAddress = "john.doe@test.com",
                Password = "Password@123",
                ConfirmPassword = "Password@123",
                Username = "JohnDoe425"
            };

            // Act
            mockDataRepository.Setup(repo => repo.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);
            var result = await mockUserController.CreateUser(user);

            // Assert
            mockDataRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
        }
        [Fact]
        public async Task Login_VerifyUserAccess_ReturnsAccessToken()
        {
            // Arrange
            var user = new LoginView()
            {
                Username = "JohnDoe425",
                Password = "Password@123"
            };
            var currentUser = new UserView()
            {
                Name = "John",
                Surname = "Doe",
                Password = "Password@123",
                ConfirmPassword = "Password@123",
                EmailAddress = "john.doe@example.com",
                Username = "JohnDoe425"
            };
            var activeUser = new List<User>() { new(currentUser) };

            // Act
            mockDataRepository.Setup(repo => repo.GetOneBy<User>(x => x.EmailAddress == user.Username || x.Username == user.Username)).Returns(activeUser.AsQueryable());
            mockDataRepository.Setup(repo => repo.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);
            var result = await mockUserController.Login(user);

            // Assert
            mockDataRepository.Verify(repo => repo.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var LoginActionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, LoginActionResult.StatusCode);
        }
        [Fact]
        public async Task Logout_RemoveRefreshToken_ReturnsOkResult()
        {
            // Arrange
            var user = new User() { Id = 1, EmailAddress = "johndoe@exampl.com" };
            var refreshToken = new List<RefreshToken>() { new("test-refresh-token", user.Id) };

            // Act
            mockCurrentUserService.Setup(service => service.GetUserDetails()).Returns(user);
            mockDataRepository.Setup(repo => repo.GetOneBy<RefreshToken>(x => x.Id == user.Id)).Returns(refreshToken.AsQueryable());
            mockDataRepository.Setup(repo => repo.Update(refreshToken.FirstOrDefault()!)).Verifiable();
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockUserController.Logout();

            // Assert
            mockDataRepository.Verify(repo => repo.Update(It.IsAny<RefreshToken>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var LogoutActionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, LogoutActionResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task GetUserById(int id)
        {
            // Arrange
            var users = new List<User>() { new() { Id = 1 } };

            // Act
            mockDataRepository.Setup(repo => repo.GetOneBy<User>(x => x.Id == id)).Returns(users.AsQueryable());

            var result = await mockUserController.GetUserById(id);

            // Assert
            var GetActionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, GetActionResult.StatusCode);
        }
        [Fact]
        public async Task UpdateUser_ModifyExistingUserData_ReturnsOkResult()
        {
            // Arrange
            var user = new User() { Id = 1, EmailAddress = "johndoe@exampl.com", Name = "John", Surname = "Doe", Username = "JohnDoe425" };
            var ModifiedUserDetails = new UpdateUserView { Name = "Joe", Surname = "Doe", Username = "JoeDoe425" };

            // Act
            mockCurrentUserService.Setup(service => service.GetUserDetails()).Returns(user);
            mockDataRepository.Setup(repo => repo.GetOneBy<User>(x => x.Id == user.Id || x.EmailAddress == user.EmailAddress)).Returns(new List<User>() { user }.AsQueryable());
            mockDataRepository.Setup(repo => repo.Update(It.IsAny<User>())).Verifiable();
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockUserController.UpdateUser(ModifiedUserDetails);

            // Assert
            mockDataRepository.Verify(repo => repo.Update(It.IsAny<User>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var UpdateActionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, UpdateActionResult.StatusCode);
        }
        [Fact]
        public async Task Password_ChangePassword_ReturnsOkResult()
        {
            // Arrange
            var changedPassword = new UpdatePasswordView()
            {
                OldPassword = "Password@123",
                NewPassword = "NewPassword@123",
                ConfirmPassword = "NewPassword@123"
            };
            var user = new UserView()
            {
                Name = "John",
                Surname = "Doe",
                EmailAddress = "john.doe@test.com",
                Password = "Password@123",
                ConfirmPassword = "Password@123",
                Username = "JohnDoe425"
            };
            var currentUser = new User(user);

            // Act
            mockCurrentUserService.Setup(service => service.GetUserDetails()).Returns(currentUser);
            mockDataRepository.Setup(repo => repo.GetOneBy<User>(x => x.Id == currentUser.Id || x.EmailAddress == currentUser.EmailAddress)).Returns(new List<User>() { currentUser }.AsQueryable());
            mockDataRepository.Setup(repo => repo.Update(It.IsAny<User>()));
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockUserController.Password(changedPassword);

            // Assert
            mockDataRepository.Verify(repo => repo.Update(It.IsAny<User>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var PasswordActionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, PasswordActionResult.StatusCode);
        }
        [Theory]
        [InlineData("test-refresh-token")]
        public async Task RefreshToken_RefreshFromValidRefreshToken_ReturnsOkResult(string refreshToken)
        {
            // Arrange
            var token = new RefreshToken(refreshToken, 1) { User = new User() { Id = 1, EmailAddress = "john.doe@example.com" } };

            // Act
            mockDataRepository.Setup(repo => repo.GetOneBy<RefreshToken>(x => x.Token == refreshToken && x.IsValid == true)).Returns(new List<RefreshToken>() { token }.AsQueryable());
            mockDataRepository.Setup(repo => repo.Update(It.IsAny<RefreshToken>())).Verifiable();
            mockDataRepository.Setup(repo => repo.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockUserController.RefreshToken(refreshToken);

            // Assert
            mockDataRepository.Verify(repo => repo.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
            mockDataRepository.Verify(repo => repo.Update(It.IsAny<RefreshToken>()), Times.Never);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var RefreshTokenActionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, RefreshTokenActionResult.StatusCode);
        }
        [Fact]
        public async Task DeleteUser_RemovesCurrentUser_ReturnsOkResult()
        {
            // Arrange
            var user = new User() { Id = 1, EmailAddress = "john.doe@example.com" };

            // Act
            mockCurrentUserService.Setup(service => service.GetUserDetails()).Returns(user);
            mockDataRepository.Setup(repo => repo.GetOneBy<User>(x => x.Id == user.Id || x.EmailAddress == user.EmailAddress)).Returns(new List<User>() { user }.AsQueryable());
            mockDataRepository.Setup(repo => repo.Delete(It.IsAny<User>())).Verifiable();
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockUserController.DeleteUser();

            // Assert
            mockDataRepository.Verify(repo => repo.Delete(It.IsAny<User>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var DeleteActionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, DeleteActionResult.StatusCode);
        }
    }
}
