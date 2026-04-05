using Backend.Controllers;
using Backend.Models.DTOs;
using Backend.Models.Views;
using Backend.Repositories.DataRepository;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.tests.ControllerTests
{
    public class UserControllerTests
    {
        private Mock<IDataRepository> mockDataRepository;
        private Mock<TokenService> mockTokenService;
        private UserController mockUserController;
        public UserControllerTests()
        {
            mockDataRepository = new Mock<IDataRepository>();
            mockUserController = new UserController(mockDataRepository.Object, mockTokenService!.Object);
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
            var newUser = new User(user);

            // Act
            mockDataRepository.Setup(repo => repo.AddAsync(newUser)).Returns(Task.CompletedTask);
            var result = await mockUserController.CreateUser(user);

            // Assert
            mockDataRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
        }
    }
}
