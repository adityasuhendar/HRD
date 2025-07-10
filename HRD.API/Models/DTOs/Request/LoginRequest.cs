
// HRD.API/Models/DTOs/Request/LoginRequest.cs
using System.ComponentModel.DataAnnotations;

namespace HRD.API.Models.DTOs.Request
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username wajib diisi")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi")]
        public string Password { get; set; } = string.Empty;
    }
}