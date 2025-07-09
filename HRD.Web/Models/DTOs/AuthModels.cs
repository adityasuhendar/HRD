// ===================================
// UPDATED: HRD.Web/Models/DTOs/AuthModels.cs
// ===================================

namespace HRD.Web.Models.DTOs
{
    public class LoginRequestDto 
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
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