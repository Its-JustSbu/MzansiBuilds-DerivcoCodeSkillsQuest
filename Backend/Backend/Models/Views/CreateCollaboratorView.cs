using Backend.Models.DTOs;

namespace Backend.Models.Views
{
    public class CreateCollaboratorView
    {
        public RequestStatus? RequestStatus { get; set; }
        public CollaboratorType? CollaboratorType { get; set; }
        public bool IsOwner { get; set; }
    }
}
