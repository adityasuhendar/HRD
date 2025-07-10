// HRD.API/Models/DTOs/Response/LoginResponse.cs
namespace HRD.API.Models.DTOs.Response
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Peran { get; set; } = string.Empty;
        public string NamaLengkap { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; } // ADD THIS FIELD

    }
}