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
        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; }
        public User? User { get; set; }
        [ForeignKey(nameof(RequestStatusId))]
        public int RequestStatusId { get; set; }
        public RequestStatus? RequestStatus { get; set; }
        [ForeignKey(nameof(CollaboratorTypeId))]
        public int CollaboratorTypeId { get; set; }
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
                Project = project;
            }
            if (user != null)
            {
                User = user;
            }
            IsOwner = createCollaboratorView.IsOwner;
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
