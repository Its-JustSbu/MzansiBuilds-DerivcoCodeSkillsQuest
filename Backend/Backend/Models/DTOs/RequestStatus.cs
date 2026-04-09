using Backend.Models.IDTOs;

namespace Backend.Models.DTOs
{
    public class RequestStatus : IStatus
    {
        int IStatus.Id { get; set; }
        string? IStatus.Name { get; set; }
    }
}
