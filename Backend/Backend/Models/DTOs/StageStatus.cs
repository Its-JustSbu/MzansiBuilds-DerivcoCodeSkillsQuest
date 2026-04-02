using Backend.Models.IDTOs;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    public class StageStatus : IStatus
    {
        int IStatus.Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string? IStatus.Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
