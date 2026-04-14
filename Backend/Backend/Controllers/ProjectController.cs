using Backend.Models.DTOs;
using Backend.Models.Views;
using Backend.Repositories.DataRepository;
using Backend.Services.CurrentUserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController(IDataRepository DataRepository, ICurrentUserService CurrentUserService) : ControllerBase
    {
        private readonly IDataRepository _dataRepository = DataRepository;
        private readonly ICurrentUserService _currentUserService = CurrentUserService;
        // POST: api/Project
        [HttpPost]
        public async Task<IActionResult> CreateProject(CreateProjectView CreateProject)
        {
            try
            {
                var user = _currentUserService.GetUserDetails();
                var currentUser = _dataRepository.GetOneBy<User>(x => x.Id == user.Id || x.EmailAddress == user.EmailAddress).FirstOrDefault();

                if (currentUser == null) return NotFound("Project or User does not exist");

                var newCollaboration = new List<Collaboration>() { new(user)};
                var newProject = new Project(CreateProject)
                {
                    Collaborations = newCollaboration
                };

                await DataRepository.AddAsync(newProject);
                await DataRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(CreateProject), new { id = newProject.Id }, newProject);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // POST: api/Project/{ProjectId}/stage
        [HttpPost("{ProjectId}/stage")]
        public async Task<IActionResult> CreateProjectStage(int ProjectId, List<StagesView> stages)
        {
            try
            {
                var projectStages = await DataRepository.GetByAsync<Project>(p => p.Id == ProjectId);

                if (projectStages.Count == 0) return NotFound("Project not found");

                var newProjectStages = new List<ProjectStage>();
                foreach (var stage in stages)
                {
                    var newProjectStage = new ProjectStage(ProjectId, stage);
                    newProjectStages.Add(newProjectStage);
                }

                await DataRepository.AddRangeAsync(newProjectStages);
                await DataRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(CreateProjectStage), newProjectStages);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // POST: api/Project/{ProjectId}/support
        [HttpPost("{ProjectId}/support")]
        public async Task<IActionResult> CreateSupportRequest(int ProjectId, SupportView support)
        {
            try
            {
                var project = DataRepository.GetOneBy<Project>(p => p.Id == ProjectId).FirstOrDefault();

                if (project == null) return NotFound("Project not found");

                var newSupportRequest = new Support(ProjectId, support);

                await DataRepository.AddAsync(newSupportRequest);
                await DataRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(CreateSupportRequest), new { ProjectId, id = newSupportRequest.Id }, newSupportRequest);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // POST: api/Project/{ProjectStageId}/milestone
        [HttpPost("{ProjectStageId}/milestone")]
        public async Task<IActionResult> CreateMilestone(int ProjectStageId, MilestoneView milestone)
        {
            try
            {
                var projectStage = DataRepository.GetOneBy<ProjectStage>(s => s.Id == ProjectStageId).FirstOrDefault();

                if (projectStage == null) return NotFound("Project stage not found");

                var newMilestone = new Milestone(milestone) { ProjectStageId = ProjectStageId };
                await DataRepository.AddAsync(newMilestone);
                await DataRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(CreateMilestone), new { ProjectStageId, id = newMilestone.Id }, newMilestone);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // GET: api/Projects
        // TODO: Add SignalR for real-time updates to projects list
        [HttpGet("{pageNumber}")]
        public async Task<IActionResult> GetProjects(int pageNumber)
        {
            var maxPageSize = 10;
            try
            {
                var projects = DataRepository.GetAll<Project>()
                    .Include(x => x.Stages)!
                    .ThenInclude(x => x.Milestones)
                    .Include(x => x.Support)
                    .Include(x => x.Collaborations)
                    .Include(x => x.Comments)
                    .Skip((pageNumber - 1) * maxPageSize)
                    .Take(maxPageSize)
                    .Select(p => new Project
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        CreatedAt = p.CreatedAt,
                        Stages = p.Stages,
                        Support = p.Support,
                        Collaborations = p.Collaborations!.Select(c => new Collaboration
                        {
                            User = new User
                            {
                                Id = c.User!.Id,
                                Name = c.User.Name,
                                EmailAddress = c.User.EmailAddress,
                                Username = c.User.Username
                            }
                        }).ToList(),
                        Comments = p.Comments
                    })
                    .ToList();

                return Ok(projects);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        [HttpGet("Celebrations/{pageNumber}")]
        public async Task<IActionResult> GetCompletedProjects(int pageNumber)
        {
            var maxPageSize = 10;
            try
            {
                var projects = DataRepository.GetAll<Project>()
                    .Include(x => x.Stages)!
                    .ThenInclude(x => x.Milestones)
                    .Include(x => x.Support)
                    .Include(x => x.Collaborations)!
                    .ThenInclude(x => x.User)
                    .Include(x => x.Comments)
                    .Select(p => new Project
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        CreatedAt = p.CreatedAt,
                        Stages = p.Stages,
                        Support = p.Support,
                        Collaborations = p.Collaborations!.Select(c => new Collaboration
                        {
                            User = new User
                            {
                                Id = c.User!.Id,
                                Name = c.User.Name,
                                EmailAddress = c.User.EmailAddress,
                                Username = c.User.Username
                            }
                        }).ToList(),
                        Comments = p.Comments
                    })
                    .ToList();

                var completedProjects = projects.Where(p => p.Stages!.All(s => s.StageStatusId == 3))
                    .Skip((pageNumber - 1) * maxPageSize)
                    .Take(maxPageSize)
                    .ToList();

                if (completedProjects.Count == 0) return NotFound(new { message = "No Projects Completed Yet!"});

                return Ok(completedProjects);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // PUT: api/Project/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, CreateProjectView updatedProject)
        {
            try
            {
                var project = DataRepository.GetOneBy<Project>(p => p.Id == id).FirstOrDefault();

                if (project == null) return NotFound("Project not found");

                project.UpdateProject(updatedProject.Name!, updatedProject.Description!);
                DataRepository.Update(project);
                await DataRepository.SaveChangesAsync();

                return Ok(new { message = "Project updated successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // PUT: api/Project/{ProjectId}/{StageId}/stage
        [HttpPut("{ProjectId}/stage")]
        public async Task<IActionResult> UpdateProjectStage(int ProjectId, List<StagesView> updatedStages)
        {
            try
            {
                var projectStages = await DataRepository.GetByAsync<ProjectStage>(s => s.ProjectId == ProjectId);

                if (projectStages.Count == 0) return NotFound("Project stage not found");

                var sortedStages = projectStages.OrderBy(x => x.StageNumber).ToList();
                var sortedUpdatedStages = updatedStages.OrderBy(x => x.StageNumber).ToList();

                for (int i = 0; i < sortedStages.Count; i++)
                {
                    sortedStages[i].UpdateProjectStage(sortedUpdatedStages[i].StageNumber, sortedUpdatedStages[i].StageTitle!, sortedUpdatedStages[i].StageStatus!.Id);
                }

                DataRepository.UpdateRange(sortedStages);
                await DataRepository.SaveChangesAsync();

                return Ok(new { message = "Project stages updated successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // PUT: api/Project/{ProjectId}/{SupportId}/support
        [HttpPut("{ProjectId}/{SupportId}/support")]
        public async Task<IActionResult> UpdateSupportRequest(int ProjectId, int SupportId, SupportView updatedSupport)
        {
            try
            {
                var supportRequest = DataRepository.GetOneBy<Support>(s => s.Id == SupportId && s.ProjectId == ProjectId).FirstOrDefault();

                if (supportRequest == null) return NotFound("Support request not found");

                supportRequest.UpdateSupport(updatedSupport.Description!, updatedSupport.SupportType!.Id, ProjectId);
                DataRepository.Update(supportRequest);
                await DataRepository.SaveChangesAsync();

                return Ok(new { message = "Support request updated successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // PUT: api/Project/{MilestoneId}/milestone
        [HttpPut("{MilestoneId}/milestone")]
        public async Task<IActionResult> UpdateMilestone(int MilestoneId, MilestoneView updatedMilestone)
        {
            try
            {
                var milestone = DataRepository.GetOneBy<Milestone>(m => m.Id == MilestoneId).FirstOrDefault();

                if (milestone == null) return NotFound("Milestone not found");

                milestone.UpdateMilestone(updatedMilestone.Description!);
                DataRepository.Update(milestone);
                await DataRepository.SaveChangesAsync();

                return Ok(new { message = "Milestone updated successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // DELETE: api/Project/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                var project = DataRepository.GetOneBy<Project>(p => p.Id == id).FirstOrDefault();

                if (project == null) return NotFound("Project not found");

                DataRepository.Delete(project);
                await DataRepository.SaveChangesAsync();

                return Ok(new { message = "Project deleted successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // DELETE: api/Project/{ProjectId}/stage
        [HttpDelete("{ProjectId}/stage")]
        public async Task<IActionResult> DeleteProjectStage(int ProjectId, List<StagesView> stages)
        {
            try
            {
                var projectStages = await DataRepository.GetByAsync<ProjectStage>(s => s.ProjectId == ProjectId);

                if (projectStages.Count == 0) return NotFound("Project stage not found");

                var sortedProjectStages = projectStages.OrderBy(x => x.StageNumber).ToList();
                var sortedStages = stages.OrderBy(x => x.StageNumber).ToList();
                var stagesToDelete = new List<ProjectStage>();

                for (int i = 0; i < sortedProjectStages.Count; i++)
                {
                    if (stagesToDelete.Count == sortedStages.Count) break;

                    if (sortedProjectStages[i].StageNumber == sortedStages[stagesToDelete.Count].StageNumber)
                    {
                        stagesToDelete.Add(sortedProjectStages[i]);
                    }
                }

                DataRepository.DeleteRange(stagesToDelete);
                await DataRepository.SaveChangesAsync();

                return Ok(new { message = "Project stages deleted successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // DELETE: api/Project/{SupportId}/support
        [HttpDelete("{SupportId}/support")]
        public async Task<IActionResult> DeleteSupportRequest(int SupportId)
        {
            try
            {
                var supportRequest = DataRepository.GetOneBy<Support>(s => s.Id == SupportId).FirstOrDefault();

                if (supportRequest == null) return NotFound("Support request not found");

                DataRepository.Delete(supportRequest);
                await DataRepository.SaveChangesAsync();

                return Ok(new { message = "Support request deleted successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // DELETE: api/Project/{MilestoneId}/milestone
        [HttpDelete("{MilestoneId}/milestone")]
        public async Task<IActionResult> DeleteMilestone(int MilestoneId)
        {
            try
            {
                var milestone = DataRepository.GetOneBy<Milestone>(m => m.Id == MilestoneId).FirstOrDefault();

                if (milestone == null) return NotFound("Milestone not found");

                DataRepository.Delete(milestone);
                await DataRepository.SaveChangesAsync();

                return Ok(new { message = "Milestone deleted successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
    }
}
