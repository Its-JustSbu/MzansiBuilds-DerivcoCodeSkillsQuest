using Backend.Controllers;
using Backend.Models.DTOs;
using Backend.Models.Views;
using Backend.Repositories.DataRepository;
using Backend.Services.CurrentUserService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Backend.tests.ControllerTests
{
    public class CollaboratorControllerTests
    {
        private Mock<IDataRepository> mockDataRepository;
        private Mock<ICurrentUserService> mockCurrentUserService;
        private CollaboratorController collaboratorController;
        public CollaboratorControllerTests()
        {
            mockDataRepository = new Mock<IDataRepository>();
            mockCurrentUserService = new Mock<ICurrentUserService>();
            collaboratorController = new CollaboratorController(mockDataRepository.Object, mockCurrentUserService.Object);
        }
        // Global Arrange variables for tests
        public static User user = new() { Id = 1, EmailAddress = "john.doe@example.com" };
        public static Project project = new() { Id = 1 };
        [Theory]
        [InlineData(1)]
        public async Task CreateCollaborator_ReturnsCreatedAtActionResult_WhenSuccessful(int projectId)
        {
            // Arrange
            var createCollaboratorView = new CreateCollaboratorView { };

            // Act
            mockCurrentUserService.Setup(s => s.GetUserDetails()).Returns(user);
            mockDataRepository.Setup(x => x.GetOneBy(It.IsAny<Expression<Func<User, bool>>>())).Returns(new List<User> { user }.AsQueryable());
            mockDataRepository.Setup(r => r.GetOneBy<Project>(x => x.Id == projectId)).Returns(new List<Project> { project }.AsQueryable());
            mockDataRepository.Setup(r => r.AddAsync(It.IsAny<Collaboration>())).Returns(Task.CompletedTask);
            mockDataRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await collaboratorController.CreateCollaborator(projectId, createCollaboratorView);

            // Assert
            mockDataRepository.Verify(r => r.GetOneBy<Project>(x => x.Id == projectId), Times.Once);
            mockDataRepository.Verify(x => x.GetOneBy(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
            mockDataRepository.Verify(r => r.AddAsync(It.IsAny<Collaboration>()), Times.Once);
            mockDataRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            mockCurrentUserService.Verify(s => s.GetUserDetails(), Times.Once);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task AddComment_AddACommentToAProject(int projectId)
        {
            // Arrange
            var createCommentView = new CreateCommentView { Title = "Test Comment", Description = "This is a test comment." };

            // Act
            mockCurrentUserService.Setup(s => s.GetUserDetails()).Returns(user);
            mockDataRepository.Setup(x => x.GetOneBy<Project>(p => p.Id == projectId)).Returns(new List<Project> { project }.AsQueryable());
            mockDataRepository.Setup(x => x.GetOneBy(It.IsAny<Expression<Func<User, bool>>>())).Returns(new List<User> { user }.AsQueryable());
            mockDataRepository.Setup(x => x.AddAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);
            mockDataRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await collaboratorController.AddComment(projectId, createCommentView);

            // Assert
            mockCurrentUserService.Verify(s => s.GetUserDetails(), Times.Once);
            mockDataRepository.Verify(x => x.GetOneBy<Project>(p => p.Id == projectId), Times.Once);
            mockDataRepository.Verify(x => x.GetOneBy(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
            mockDataRepository.Verify(x => x.AddAsync(It.IsAny<Comment>()), Times.Once);
            mockDataRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
        }
        [Fact]
        public async Task GetCollaboratorProjects_GetsAllProjectsBelongingToUser_ReturnsOkObjectResult()
        {
            // Arrange
            var collaborations = new List<Collaboration>
            {
                new() { Id = 1, Project = project, ProjectId = project.Id, User = user, UserId = 1, IsOwner = true, CollaboratorType = new CollaboratorType() { Id = 1 }, CollaboratorTypeId = 1, RequestStatus = new RequestStatus() { Id = 1 }, RequestStatusId = 1 },
                new() { Id = 2, Project = new Project { Id = 2 }, ProjectId = 2, User = user, UserId = 1, IsOwner = false, CollaboratorType = new CollaboratorType() { Id = 2 }, CollaboratorTypeId = 2, RequestStatus = new RequestStatus() { Id = 2 }, RequestStatusId = 2 }
            };
            mockCurrentUserService.Setup(s => s.GetUserDetails()).Returns(user);
            mockDataRepository.Setup(x => x.GetOneBy(It.IsAny<Expression<Func<User, bool>>>())).Returns(new List<User> { user }.AsQueryable());
            mockDataRepository.Setup(r => r.GetBy(It.IsAny<Expression<Func<Collaboration, bool>>>())).Returns(collaborations.AsQueryable());

            // Act
            var result = await collaboratorController.GetCollaboratorProjects();

            // Assert
            mockCurrentUserService.Verify(s => s.GetUserDetails(), Times.Once);
            mockDataRepository.Verify(x => x.GetOneBy(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
            mockDataRepository.Verify(r => r.GetBy(It.IsAny<Expression<Func<Collaboration, bool>>>()), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task GetCollaborators_GetsAllCollaboratorsOfAProject_ReturnsOkObjectResult(int ProjectId)
        {
            // Arrange
            var collaborations = new List<Collaboration>
            {
                new() { Id = 1, Project = project, ProjectId = project.Id, User = user, UserId = 1, IsOwner = true, CollaboratorType = new CollaboratorType() { Id = 1 }, CollaboratorTypeId = 1, RequestStatus = new RequestStatus() { Id = 1 }, RequestStatusId = 1 },
                new() { Id = 2, Project = new Project { Id = 2 }, ProjectId = 2, User = user, UserId = 1, IsOwner = false, CollaboratorType = new CollaboratorType() { Id = 2 }, CollaboratorTypeId = 2, RequestStatus = new RequestStatus() { Id = 2 }, RequestStatusId = 2 }
            };
            mockDataRepository.Setup(r => r.GetBy<Collaboration>(c => c.ProjectId == ProjectId)).Returns(collaborations.AsQueryable());

            // Act
            var result = await collaboratorController.GetCollaborators(ProjectId);

            // Assert
            mockDataRepository.Verify(r => r.GetBy<Collaboration>(c => c.ProjectId == ProjectId), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task GetComment_GetsAllCommentsOfAProject_ReturnsOkObjectResult(int ProjectId)
        {
            // Arrange
            var comments = new List<Comment>
            {
                new() { Id = 1, Project = project, ProjectId = project.Id, User = user, UserId = 1, Title = "Can be Better", Description = "This Project Comment" },
            };
            mockDataRepository.Setup(r => r.GetBy<Comment>(c => c.ProjectId == ProjectId)).Returns(comments.AsQueryable());

            // Act
            var result = await collaboratorController.GetComments(ProjectId);

            // Assert
            mockDataRepository.Verify(r => r.GetBy<Comment>(c => c.ProjectId == ProjectId), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task UpdateCollaborator_ModifiesTheDetailsOfACollaborator_ReturnsOkObjectResult(int collaboratorId)
        {
            // Arrange
            var updateCollaboratorView = new CreateCollaboratorView { IsOwner = true, CollaboratorType = new CollaboratorType() { Id = 2 }, RequestStatus = new RequestStatus() { Id = 2 } };
            var collaboration = new Collaboration { Id = collaboratorId, Project = project, ProjectId = project.Id, User = user, UserId = 1, IsOwner = true, CollaboratorType = new CollaboratorType() { Id = 1 }, CollaboratorTypeId = 1, RequestStatus = new RequestStatus() { Id = 1 }, RequestStatusId = 1 };

            // Act
            mockDataRepository.Setup(r => r.GetOneBy<Collaboration>(c => c.Id == collaboratorId)).Returns(new List<Collaboration> { collaboration }.AsQueryable());
            mockDataRepository.Setup(r => r.Update(It.IsAny<Collaboration>())).Verifiable();
            mockDataRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await collaboratorController.UpdateCollaborator(collaboratorId, updateCollaboratorView);

            // Assert
            mockDataRepository.Verify(r => r.GetOneBy<Collaboration>(c => c.Id == collaboratorId), Times.Once);
            mockDataRepository.Verify(r => r.Update(It.IsAny<Collaboration>()), Times.Once);
            mockDataRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task DeleteCollaborator_RemovesACollaborator_ReturnsOkObjectResult(int collaboratorId)
        {
            // Arrange
            var collaboration = new Collaboration { Id = collaboratorId, Project = project, ProjectId = project.Id, User = user, UserId = 1, IsOwner = true, CollaboratorType = new CollaboratorType() { Id = 1 }, CollaboratorTypeId = 1, RequestStatus = new RequestStatus() { Id = 1 }, RequestStatusId = 1 };

            // Act
            mockDataRepository.Setup(r => r.GetOneBy<Collaboration>(c => c.Id == collaboratorId)).Returns(new List<Collaboration> { collaboration }.AsQueryable());
            mockDataRepository.Setup(r => r.Delete(It.IsAny<Collaboration>())).Verifiable();
            mockDataRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await collaboratorController.DeleteCollaborator(collaboratorId);

            // Assert
            mockDataRepository.Verify(r => r.GetOneBy<Collaboration>(c => c.Id == collaboratorId), Times.Once);
            mockDataRepository.Verify(r => r.Delete(It.IsAny<Collaboration>()), Times.Once);
            mockDataRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task DeleteComment_RemovesAComment_ReturnsOkObjectResult(int commentId)
        {
            // Arrange
            var comment = new Comment { Id = commentId, Project = project, ProjectId = project.Id, User = user, UserId = 1, Title = "Can be Better", Description = "This Project Comment" };

            // Act
            mockDataRepository.Setup(r => r.GetOneBy<Comment>(c => c.Id == commentId)).Returns(new List<Comment> { comment }.AsQueryable());
            mockDataRepository.Setup(r => r.Delete(It.IsAny<Comment>())).Verifiable();
            mockDataRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await collaboratorController.DeleteComment(commentId);

            // Assert
            mockDataRepository.Verify(r => r.GetOneBy<Comment>(c => c.Id == commentId), Times.Once);
            mockDataRepository.Verify(r => r.Delete(It.IsAny<Comment>()), Times.Once);
            mockDataRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }
    }
}
