﻿// HRD.Web/Models/DTOs/ApiResponse.cs
namespace HRD.Web.Models.DTOs

{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
    }
}