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
    public class ProjectControllerTests
    {
        private Mock<IDataRepository> mockDataRepository;
        private Mock<ICurrentUserService> mockCurrentUserService;
        private ProjectController mockProjectController;
        public ProjectControllerTests()
        {
            mockDataRepository = new Mock<IDataRepository>();
            mockCurrentUserService = new Mock<ICurrentUserService>();
            mockProjectController = new ProjectController(mockDataRepository.Object, mockCurrentUserService.Object);
        }
        // Global Arrange
        public static List<Project> projects = [
            new() { Id = 1, Name = "Test", Description = "Test Project" },
            new() { Id = 2, Name = "Test", Description = "Test Project" },
            new() { Id = 3, Name = "Test", Description = "Test Project" },
            new() { Id = 4, Name = "Test", Description = "Test Project" },
            new() { Id = 5, Name = "Test", Description = "Test Project" },
            new() { Id = 6, Name = "Test", Description = "Test Project" },
            new() { Id = 7, Name = "Test", Description = "Test Project" },
            new() { Id = 8, Name = "Test", Description = "Test Project" },
            new() { Id = 9, Name = "Test", Description = "Test Project" },
            new() { Id = 10, Name = "Test", Description = "Test Project" },
            new() { Id = 11, Name = "Test", Description = "Test Project" },
        ];
        public static List<SupportType> supportTypes = [
            new() { Id = 1, Name = "Financial" },
            new() { Id = 2, Name = "Developers" },
            new() { Id = 3, Name = "UI/UX Designers" },
            new() { Id = 4, Name = "Analysts" },
            new() { Id = 5, Name = "Other" },
        ];
        public static List<StageStatus> stageStatuses = [
            new() { Id = 1, Name = "Not Started" },
            new() { Id = 2, Name = "In Progress" },
            new() { Id = 3, Name = "Completed" },
        ];
        [Fact]
        public async Task CreateProject_AddsANewProject_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var project = new CreateProjectView
            {
                Name = "Test Project",
                Description = "This is a test project",
                Stages = [
                    new() { StageTitle = "Stage 1", StageNumber = 1, StageStatus = stageStatuses[0] },
                    new() { StageTitle = "Stage 2", StageNumber = 2, StageStatus = stageStatuses[0] }
                ],
                Support = [
                    new() { Description = "This is support 1", SupportType = supportTypes[0] },
                    new() { Description = "This is support 2", SupportType = supportTypes[1] }
                ]
            };
            User user = new() { Id = 1, EmailAddress = "john.doe@example.com" };

            // Act
            mockCurrentUserService.Setup(s => s.GetUserDetails()).Returns(user);
            mockDataRepository.Setup(x => x.GetOneBy(It.IsAny<Expression<Func<User, bool>>>())).Returns(new List<User> { user }.AsQueryable());
            mockDataRepository.Setup(repo => repo.AddAsync(It.IsAny<Project>())).Returns(Task.CompletedTask);
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockProjectController.CreateProject(project);

            // Assert
            mockDataRepository.Verify(repo => repo.AddAsync(It.IsAny<Project>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            mockCurrentUserService.Verify(s => s.GetUserDetails(), Times.Once);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task CreateProjectStage_AddNewProjectStages_ReturnCreatedAtActionResult(int ProjectId)
        {
            // Arrange
            var projectStages = new List<ProjectStage>()
            {
                new() { Project = projects[0], Id = 1, StageTitle = "Stage 1", StageNumber = 1, StageStatusId = stageStatuses[0].Id, StageStatus = stageStatuses[0] },
                new() { Project = projects[0], Id = 2, StageTitle = "Stage 2", StageNumber = 2, StageStatusId = stageStatuses[0].Id, StageStatus = stageStatuses[0] },
            };
            var newProjectStages = new List<StagesView>()
            {
                new() { StageTitle = "Stage 3", StageNumber = 3, StageStatus = stageStatuses[0] },
                new() { StageTitle = "Stage 4", StageNumber = 4, StageStatus = stageStatuses[0] },
            };

            // Act
            mockDataRepository.Setup(repo => repo.GetByAsync<Project>(x => x.Id == ProjectId)).ReturnsAsync([projects[0]]);
            mockDataRepository.Setup(repo => repo.AddRangeAsync(It.IsAny<List<ProjectStage>>())).Returns(Task.CompletedTask);
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockProjectController.CreateProjectStage(ProjectId, newProjectStages);

            // Assert
            mockDataRepository.Verify(repo => repo.GetByAsync<Project>(x => x.Id == ProjectId), Times.Once);
            mockDataRepository.Verify(repo => repo.AddRangeAsync(It.IsAny<List<ProjectStage>>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task CreateSupportRequest_AddANewSupportRequest_ReturnCreatedAtActionResult(int ProjectId)
        {
            // Arrange
            var supportRequests = new List<Support>()
            {
                new() { Project = projects[0], Id = 1, Description = "This is support 1", SupportTypeId = supportTypes[0].Id, SupportType = supportTypes[0] },
                new() { Project = projects[0], Id = 2, Description = "This is support 2", SupportTypeId = supportTypes[1].Id, SupportType = supportTypes[1] },
            };
            var newSupportRequest = new SupportView() { Description = "This is support 4", SupportType = supportTypes[1] };

            // Act
            mockDataRepository.Setup(repo => repo.GetOneBy<Project>(p => p.Id == ProjectId)).Returns(new List<Project>() { projects[0] }.AsQueryable());
            mockDataRepository.Setup(repo => repo.AddAsync(It.IsAny<Support>())).Returns(Task.CompletedTask);
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockProjectController.CreateSupportRequest(ProjectId, newSupportRequest);

            // Assert
            mockDataRepository.Verify(repo => repo.GetOneBy<Project>(p => p.Id == ProjectId), Times.Once);
            mockDataRepository.Verify(repo => repo.AddAsync(It.IsAny<Support>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
        }
        [Theory]
        [InlineData(1, 1)]
        public async Task CreateMilestone_AddANewMilestone_ReturnCreatedAtActionResult(int ProjectStageId, int MilestoneId)
        {
            // Arrange
            var milestones = new List<Milestone>()
            {
                new() { Id = 1, Description = "This is a milestone", ProjectStage = new ProjectStage() { Id = ProjectStageId } },
                new() { Id = 2, Description = "This is another milestone", ProjectStage = new ProjectStage() { Id = ProjectStageId } },
            };
            var newMilestone = new MilestoneView() { Description = "This is a new milestone" };

            // Act
            mockDataRepository.Setup(repo => repo.GetOneBy<ProjectStage>(p => p.Id == ProjectStageId)).Returns(new List<ProjectStage>() { new() { Id = ProjectStageId } }.AsQueryable());
            mockDataRepository.Setup(repo => repo.AddAsync(It.IsAny<Milestone>())).Returns(Task.CompletedTask);
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockProjectController.CreateMilestone(ProjectStageId, newMilestone);

            // Assert
            mockDataRepository.Verify(repo => repo.GetOneBy<ProjectStage>(p => p.Id == ProjectStageId), Times.Once);
            mockDataRepository.Verify(repo => repo.AddAsync(It.IsAny<Milestone>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task GetProjects_RetrievesPaginatedProjects_ReturnsOkObjectResult(int pageNumber)
        {
            // Arrange
            int pageSize = 10;
            var paginatedProjects = projects.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Act
            mockDataRepository.Setup(repo => repo.GetAll<Project>()).Returns(projects.AsQueryable());

            var result = await mockProjectController.GetProjects(pageNumber);

            // Assert
            mockDataRepository.Verify(repo => repo.GetAll<Project>(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
            var returnedProjects = Assert.IsType<List<Project>>(okObjectResult.Value);
            Assert.Equal(paginatedProjects.Count, returnedProjects.Count);
        }
        [Theory]
        [InlineData(1)]
        public async Task UpdateProject_ModifyProjectDetails_ReturnsOkObjectResult(int ProjectId)
        {
            // Arrange
            var updatedProject = new CreateProjectView() { Name = "Updated Test Project", Description = "This is an updated test project" };

            // Act
            mockDataRepository.Setup(repo => repo.GetOneBy<Project>(p => p.Id == ProjectId)).Returns(new List<Project>() { projects[0] }.AsQueryable());
            mockDataRepository.Setup(repo => repo.Update(It.IsAny<Project>())).Verifiable();
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockProjectController.UpdateProject(ProjectId, updatedProject);

            // Assert
            mockDataRepository.Verify(repo => repo.GetOneBy<Project>(p => p.Id == ProjectId), Times.Once);
            mockDataRepository.Verify(repo => repo.Update(It.IsAny<Project>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task UpdateProjectStages_ModifiesExistingProjectStages_ReturnsOkResult(int ProjectId)
        {
            // Arrange
            var existingProjectStages = new List<ProjectStage>() {
                new() { Id = 1, Project = projects[0], StageTitle = "Stage 1", StageNumber = 1, StageStatusId = stageStatuses[0].Id, StageStatus = stageStatuses[0] },
                new() { Id = 2, Project = projects[0], StageTitle = "Stage 2", StageNumber = 2, StageStatusId = stageStatuses[0].Id, StageStatus = stageStatuses[0] }
            };
            var updatedProjectStages = new List<StagesView>() {
                new() { StageTitle = "Stage 2", StageNumber = 2, StageStatus = stageStatuses[0] },
                new() { StageTitle = "Updated Stage 1", StageNumber = 1, StageStatus = stageStatuses[1] },
            };
            // Act
            mockDataRepository.Setup(repo => repo.GetByAsync<ProjectStage>(s => s.ProjectId == ProjectId)).ReturnsAsync(existingProjectStages);
            mockDataRepository.Setup(repo => repo.UpdateRange(It.IsAny<List<ProjectStage>>())).Verifiable();
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockProjectController.UpdateProjectStage(ProjectId, updatedProjectStages);

            // Assert
            mockDataRepository.Verify(repo => repo.GetByAsync<ProjectStage>(s => s.ProjectId == ProjectId), Times.Once);
            mockDataRepository.Verify(repo => repo.UpdateRange(It.IsAny<List<ProjectStage>>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }
        [Theory]
        [InlineData(1, 1)]
        public async Task UpdateSupportRequest_ModifiesExistingSupportRequest_ReturnsOkResult(int ProjectId, int SupportId)
        {
            // Arrange
            var existingSupportRequest = new List<Support>() { new() { Id = SupportId, Project = projects[0], Description = "This is support 1", SupportTypeId = supportTypes[0].Id, SupportType = supportTypes[0] } };
            var updatedSupportRequest = new SupportView() { Description = "This is the updated support request", SupportType = supportTypes[1] };

            // Act
            mockDataRepository.Setup(repo => repo.GetOneBy<Support>(s => s.Id == SupportId && s.ProjectId == ProjectId)).Returns(existingSupportRequest.AsQueryable());
            mockDataRepository.Setup(repo => repo.Update(It.IsAny<Support>())).Verifiable();
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockProjectController.UpdateSupportRequest(ProjectId, SupportId, updatedSupportRequest);

            // Assert
            mockDataRepository.Verify(repo => repo.GetOneBy<Support>(s => s.Id == SupportId && s.ProjectId == ProjectId), Times.Once);
            mockDataRepository.Verify(repo => repo.Update(It.IsAny<Support>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task UpdateMilestone_ModifiesExistingMilestone_ReturnsOkResult(int MilestoneId)
        {
            // Arrange
            var existingMilestone = new List<Milestone>() { new() { Id = MilestoneId, Description = "This is a milestone", ProjectStage = new ProjectStage() { Id = 1 } } };
            var updatedMilestone = new MilestoneView() { Description = "This is the updated milestone" };

            // Act
            mockDataRepository.Setup(repo => repo.GetOneBy<Milestone>(m => m.Id == MilestoneId)).Returns(existingMilestone.AsQueryable());
            mockDataRepository.Setup(repo => repo.Update(It.IsAny<Milestone>())).Verifiable();
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockProjectController.UpdateMilestone(MilestoneId, updatedMilestone);

            // Assert
            mockDataRepository.Verify(repo => repo.GetOneBy<Milestone>(m => m.Id == MilestoneId), Times.Once);
            mockDataRepository.Verify(repo => repo.Update(It.IsAny<Milestone>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task DeleteProject_RemovesProject_ReturnsOkObjectResult(int ProjectId)
        {
            // Act
            mockDataRepository.Setup(repo => repo.GetOneBy<Project>(x => x.Id == ProjectId)).Returns(new List<Project>() { projects[0] }.AsQueryable());
            mockDataRepository.Setup(repo => repo.Delete(It.IsAny<Project>())).Verifiable();
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockProjectController.DeleteProject(ProjectId);

            // Assert
            mockDataRepository.Verify(repo => repo.GetOneBy<Project>(x => x.Id == ProjectId), Times.Once);
            mockDataRepository.Verify(repo => repo.Delete(It.IsAny<Project>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task DeleteProjectStages_RemoveRangeOfProjectStages_ReturnsOkObjectResult(int ProjectId)
        {
            // Arrange
            var stagesToDelete = new List<StagesView>() {
                new() { StageTitle = "Stage 2", StageNumber = 2, StageStatus = stageStatuses[0] },
                new() { StageTitle = "Stage 1", StageNumber = 1, StageStatus = stageStatuses[1] },
            };
            var stages = new List<ProjectStage>() {
                new() { Id = 1, Project = projects[0], StageTitle = "Stage 1", StageNumber = 1, StageStatusId = stageStatuses[0].Id, StageStatus = stageStatuses[0] },
                new() { Id = 2, Project = projects[0], StageTitle = "Stage 2", StageNumber = 2, StageStatusId = stageStatuses[0].Id, StageStatus = stageStatuses[0] },
                new() { Id = 3, Project = projects[0], StageTitle = "Stage 3", StageNumber = 3, StageStatusId = stageStatuses[0].Id, StageStatus = stageStatuses[0] },
            };

            // Act
            mockDataRepository.Setup(repo => repo.GetByAsync<ProjectStage>(s => s.ProjectId == ProjectId)).ReturnsAsync(stages);
            mockDataRepository.Setup(repo => repo.DeleteRange(It.IsAny<List<ProjectStage>>())).Verifiable();
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockProjectController.DeleteProjectStage(ProjectId, stagesToDelete);

            // Assert
            mockDataRepository.Verify(repo => repo.GetByAsync<ProjectStage>(s => s.ProjectId == ProjectId), Times.Once);
            mockDataRepository.Verify(repo => repo.DeleteRange(It.IsAny<List<ProjectStage>>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task DeleteSupport_RemoveSupportFromAProject_ReturnsOkObjectResult(int SupportId)
        {
            // Arrange
            var deletedSupports = new List<Support>() { new() { Id = 1, Description = "This is some support"} };

            // Act
            mockDataRepository.Setup(repo => repo.GetOneBy<Support>(x => x.Id == SupportId)).Returns(deletedSupports.AsQueryable());
            mockDataRepository.Setup(repo => repo.Delete(It.IsAny<Support>())).Verifiable();
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockProjectController.DeleteSupportRequest(SupportId);

            // Assert
            mockDataRepository.Verify(repo => repo.GetOneBy<Support>(x => x.Id == SupportId), Times.Once);
            mockDataRepository.Verify(repo => repo.Delete(It.IsAny<Support>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async Task DeleteMilestone_RemovesMilestoneFromAProjectStage_ReturnsOkObjectResult(int MilestoneId)
        {
            // Arrange
            var deletedMilestones = new List<Milestone>() { new() { Id = 1, Description = "This is a milestone", ProjectStage = new ProjectStage() { Id = 1 } } };

            // Act
            mockDataRepository.Setup(repo => repo.GetOneBy<Milestone>(x => x.Id == MilestoneId)).Returns(deletedMilestones.AsQueryable());
            mockDataRepository.Setup(repo => repo.Delete(It.IsAny<Milestone>())).Verifiable();
            mockDataRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await mockProjectController.DeleteMilestone(MilestoneId);

            // Assert
            mockDataRepository.Verify(repo => repo.GetOneBy<Milestone>(x => x.Id == MilestoneId), Times.Once);
            mockDataRepository.Verify(repo => repo.Delete(It.IsAny<Milestone>()), Times.Once);
            mockDataRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }
    }
}
