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
    public class ProjectController(IDataRepository DataRepository) : ControllerBase
    {
        // POST: api/Project
        [HttpPost]
        public async Task<IActionResult> CreateProject(CreateProjectView CreateProject)
        {
            try
            {
                var newProject = new Project(CreateProject);

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
                var project = DataRepository.GetOneByAsync<Project>(p => p.Id == ProjectId).FirstOrDefault();

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
                    .ToList();

                return Ok(projects);
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
                var project = DataRepository.GetOneByAsync<Project>(p => p.Id == id).FirstOrDefault();

                if (project == null) return NotFound("Project not found");

                project.UpdateProject(updatedProject.Name!, updatedProject.Description!);
                DataRepository.Update(project);
                await DataRepository.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // PUT: api/Project/{ProjectId}/{StageId}/stage
        [HttpPut("{ProjectId}/{StageId}/stage")]
        public async Task<IActionResult> UpdateProjectStage(int ProjectId, int StageId, List<StagesView> updatedStages)
        {
            try
            {
                var projectStages = await DataRepository.GetByAsync<ProjectStage>(s => s.Id == StageId && s.ProjectId == ProjectId);

                if (projectStages.Count == 0) return NotFound("Project stage not found");

                var sortedStages = projectStages.OrderBy(x => x.StageNumber).ToList();
                var sortedUpdatedStages = updatedStages.OrderBy(x => x.StageNumber).ToList();

                for (int i = 0; i < sortedStages.Count; i++)
                {
                    sortedStages[i].UpdateProjectStage(sortedUpdatedStages[i].StageNumber, sortedUpdatedStages[i].StageTitle!, ProjectId);
                }

                DataRepository.UpdateRange(sortedStages);
                await DataRepository.SaveChangesAsync();

                return Ok();
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
                var supportRequest = DataRepository.GetOneByAsync<Support>(s => s.Id == SupportId && s.ProjectId == ProjectId).FirstOrDefault();

                if (supportRequest == null) return NotFound("Support request not found");

                supportRequest.UpdateSupport(updatedSupport.Description!, updatedSupport.SupportType!.Id, ProjectId);
                DataRepository.Update(supportRequest);
                await DataRepository.SaveChangesAsync();

                return Ok();
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
                var project = DataRepository.GetOneByAsync<Project>(p => p.Id == id).FirstOrDefault();

                if (project == null) return NotFound("Project not found");

                DataRepository.Delete(project);
                await DataRepository.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // DELETE: api/Project/{ProjectId}/{StageId}/stage
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
                    if (i >= sortedStages.Count) break;

                    if (sortedProjectStages[i].StageNumber == sortedStages[i].StageNumber)
                    {
                        stagesToDelete.Add(sortedProjectStages[i]);
                    }
                }

                DataRepository.DeleteRange(stagesToDelete);
                await DataRepository.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // DELETE: api/Project/{ProjectId}/{SupportId}/support
        [HttpDelete("{ProjectId}/{SupportId}/support")]
        public async Task<IActionResult> DeleteSupportRequest(int ProjectId, int SupportId)
        {
            try
            {
                var supportRequest = DataRepository.GetOneByAsync<Support>(s => s.Id == SupportId && s.ProjectId == ProjectId).FirstOrDefault();

                if (supportRequest == null) return NotFound("Support request not found");

                DataRepository.Delete(supportRequest);
                await DataRepository.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
    }
}
