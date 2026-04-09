using Backend.Models.IDTOs;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    public class SupportType : IStatus
    {
        int IStatus.Id { get; set; }
        string? IStatus.Name { get; set; }
    }
}
