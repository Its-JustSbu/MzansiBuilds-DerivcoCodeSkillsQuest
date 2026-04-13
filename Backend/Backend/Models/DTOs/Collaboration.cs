using Backend.Models.Views;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.DTOs
{
    public class Collaboration
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        public int RequestStatusId { get; set; }
        [ForeignKey(nameof(RequestStatusId))]
        public RequestStatus? RequestStatus { get; set; }
        public int CollaboratorTypeId { get; set; } = 1;
        [ForeignKey(nameof(CollaboratorTypeId))]
        public CollaboratorType? CollaboratorType { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime JoinedAt { get; set; } = DateTime.Now;
        public bool IsOwner { get; set; }
        public Collaboration() { }
        public Collaboration(CreateCollaboratorView createCollaboratorView, Project project, User user)
        {
            if (createCollaboratorView.RequestStatus != null)
            {
                RequestStatusId = createCollaboratorView.RequestStatus.Id;
            }
            if (createCollaboratorView.CollaboratorType != null)
            {
                CollaboratorTypeId = createCollaboratorView.CollaboratorType.Id;
            }
            if (project != null)
            {
                ProjectId = project.Id;
            }
            if (user != null)
            {
                UserId = user.Id;
            }
            IsOwner = createCollaboratorView.IsOwner;
        }
        public Collaboration(User user)
        {
            UserId = user.Id;
            RequestStatusId = 2;
            CollaboratorTypeId = 1;
            IsOwner = true;
        }
        public void UpdatedCollaboration(int requestStatusId, int collaboratorTypeId, bool isOwner)
        {
            RequestStatusId = requestStatusId;
            CollaboratorTypeId = collaboratorTypeId;
            IsOwner = isOwner;
            if (RequestStatus != null)
            {
                RequestStatus = null;
            }
            if (CollaboratorType != null)
            {
                CollaboratorType = null;
            }
        }
    }
}
