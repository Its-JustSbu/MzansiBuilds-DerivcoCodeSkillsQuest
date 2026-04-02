using Backend.Models.IDTOs;

namespace Backend.Models.DTOs
{
    public class RequestStatus : IStatus
    {
        int IStatus.Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string? IStatus.Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
