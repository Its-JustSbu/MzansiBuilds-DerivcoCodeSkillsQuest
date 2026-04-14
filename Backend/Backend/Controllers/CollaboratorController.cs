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
    public class CollaboratorController(IDataRepository dataRepository, ICurrentUserService currentUserService) : ControllerBase
    {
        // POST: api/Collaborator/{ProjectId}
        [HttpPost("{ProjectId}")]
        public async Task<IActionResult> CreateCollaborator(int ProjectId, CreateCollaboratorView createCollaboratorView)
        {
            try
            {
                var user = currentUserService.GetUserDetails();
                var currentUser = dataRepository.GetOneBy<User>(x => x.Id == user.Id || x.EmailAddress == user.EmailAddress).FirstOrDefault();
                var project = dataRepository.GetOneBy<Project>(p => p.Id == ProjectId).Include(x => x.Collaborations).FirstOrDefault();

                if (project == null || currentUser == null) return NotFound("Project or User does not exist");

                if (project.Collaborations != null && project.Collaborations.Any(c => c.UserId == currentUser.Id))
                    return BadRequest(new { message = "User is already a collaborator of this project"});

                if (project.Collaborations!.Count > 0)
                {
                    createCollaboratorView.IsOwner = true;
                    createCollaboratorView.CollaboratorType = new() { Id = 1, Name = "Owner" };
                }

                var collaboration = new Collaboration(createCollaboratorView, project, currentUser);

                await dataRepository.AddAsync(collaboration);
                await dataRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(CreateCollaborator), new { id = collaboration.Id }, collaboration);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // POST: api/Collaborator/{ProjectId}/comment
        [HttpPost("{ProjectId}/comment")]
        public async Task<IActionResult> AddComment(int ProjectId, CreateCommentView createCommentView)
        {
            try
            {
                var user = currentUserService.GetUserDetails();
                var currentUser = dataRepository.GetOneBy<User>(x => x.Id == user.Id || x.EmailAddress == user.EmailAddress).FirstOrDefault();
                var project = dataRepository.GetOneBy<Project>(p => p.Id == ProjectId).FirstOrDefault();

                if (project == null || currentUser == null) return NotFound("Project or User does not exist");

                var comment = new Comment(createCommentView, project, currentUser);

                await dataRepository.AddAsync(comment);
                await dataRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(AddComment), new { id = comment.Id }, comment);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // GET: api/Collaborator
        [HttpGet]
        public async Task<IActionResult> GetCollaboratorProjects()
        {
            try
            {
                var user = currentUserService.GetUserDetails();
                var currentUser = dataRepository.GetOneBy<User>(x => x.Id == user.Id || x.EmailAddress == user.EmailAddress).FirstOrDefault();

                if (currentUser == null) return Forbid("Access Forbidden!");

                var collaborators = dataRepository.GetBy<Collaboration>(x => x.UserId == currentUser.Id)
                    .Include(x => x.Project)
                    .ThenInclude(x => x.Collaborations)
                    .Include(x => x.User)
                    .Include(x => x.RequestStatus)
                    .Include(x => x.CollaboratorType)
                    .Select(c => new Collaboration
                    {
                        Id = c.Id,
                        User = new User
                        {
                            Id = c.User!.Id,
                            Name = c.User.Name,
                            Surname = c.User.Surname,
                            EmailAddress = c.User.EmailAddress,
                            Username = c.User.Username
                        },
                        Project = c.Project,
                        RequestStatus = c.RequestStatus,
                        CollaboratorType = c.CollaboratorType,
                        JoinedAt = c.JoinedAt,
                        IsOwner = c.IsOwner
                    })
                    .ToList();

                return Ok(collaborators);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // GET: api/Collaborator/{ProjectId}
        [HttpGet("{ProjectId}")]
        public async Task<IActionResult> GetCollaborators(int ProjectId)
        {
            try
            {
                var collaborators = dataRepository.GetBy<Collaboration>(x => x.ProjectId == ProjectId)
                    .Include(x => x.Project)
                    .Include(x => x.User)
                    .Include(x => x.RequestStatus)
                    .Include(x => x.CollaboratorType)
                    .Select(c => new Collaboration
                    {
                        Id = c.Id,
                        User = new User
                        {
                            Id = c.User!.Id,
                            Name = c.User.Name,
                            Surname = c.User.Surname,
                            EmailAddress = c.User.EmailAddress,
                            Username = c.User.Username
                        },
                        Project = c.Project,
                        RequestStatus = c.RequestStatus,
                        CollaboratorType = c.CollaboratorType,
                        JoinedAt = c.JoinedAt,
                        IsOwner = c.IsOwner
                    })
                    .ToList();

                return Ok(collaborators);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // GET: api/Collaborator/{ProjectId}/comments
        [HttpGet("{ProjectId}/comments")]
        public async Task<IActionResult> GetComments(int ProjectId)
        {
            try
            {
                var comments = dataRepository.GetBy<Comment>(x => x.ProjectId == ProjectId)
                    .Include(x => x.Project)
                    .Include(x => x.User)
                    .Select(c => new Comment
                    {
                        Id = c.Id,
                        User = new User
                        {
                            Id = c.User!.Id,
                            Name = c.User.Name,
                            Surname = c.User.Surname,
                            EmailAddress = c.User.EmailAddress,
                            Username = c.User.Username
                        },
                        Project = c.Project,
                        Title = c.Title,
                        Description = c.Description,
                        CreatedAt = c.CreatedAt
                    })
                    .ToList();

                return Ok(comments);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // PUT: api/Collaborator/{Id}
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateCollaborator(int Id, CreateCollaboratorView createCollaboratorView)
        {
            try
            {
                var collaboration = dataRepository.GetOneBy<Collaboration>(c => c.Id == Id).FirstOrDefault();

                if (collaboration == null) return NotFound("Collaboration does not exist");

                collaboration.UpdatedCollaboration(createCollaboratorView.RequestStatus!.Id, createCollaboratorView.CollaboratorType!.Id, createCollaboratorView.IsOwner);
                dataRepository.Update(collaboration);
                await dataRepository.SaveChangesAsync();

                return Ok(new { message = "Collaborator successfully updated!" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // DELETE: api/Collaborator/{Id}
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteCollaborator(int Id)
        {
            try
            {
                var collaboration = dataRepository.GetOneBy<Collaboration>(c => c.Id == Id).FirstOrDefault();

                if (collaboration == null) return NotFound("Collaboration does not exist");

                dataRepository.Delete(collaboration);
                await dataRepository.SaveChangesAsync();

                return Ok(new { message = "Collaborator successfully deleted!" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
        // DELETE: api/Collaborator/{CommentId}/comment
        [HttpDelete("{CommentId}/comment")]
        public async Task<IActionResult> DeleteComment(int CommentId)
        {
            try
            {
                var comment = dataRepository.GetOneBy<Comment>(c => c.Id == CommentId).FirstOrDefault();

                if (comment == null) return NotFound("Comment does not exist");

                dataRepository.Delete(comment);
                await dataRepository.SaveChangesAsync();

                return Ok(new { message = "Comment successfully deleted!" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                throw;
            }
        }
    }
}
