using Backend.Models.Views;
using Backend.Repositories.DataRepository;
using Backend.Services.CurrentUserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController(IDataRepository dataRepository, ICurrentUserService currentUserService) : ControllerBase
    {
        // POST: api/Project
        public async Task<IActionResult> CreateProject(CreateProjectView newProject)
        {
            try
            {
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // POST: api/Project/{ProjectId}/stage

        // POST: api/Project/{ProjectId}/support

        // GET: api/Projects
        // TODO: Add pagination, filtering and view to include all dependant entities, collaborators and comments

        // PUT: api/Project/{id}

        // PUT: api/Project/{ProjectId}/{StageId}/stage

        // PUT: api/Project/{ProjectId}/{SupportId}/support

        // DELETE: api/Project/{id}

        // DELETE: api/Project/{ProjectId}/{StageId}/stage

        // DELETE: api/Project/{ProjectId}/{SupportId}/support

    }
}
