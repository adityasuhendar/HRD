// HRD.Web/Models/ViewModels/DashboardViewModel.cs
using HRD.Web.Models.DTOs;

namespace HRD.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public string NamaKaryawan { get; set; } = string.Empty;
        public string Peran { get; set; } = string.Empty;
        public DateTime TanggalHariIni { get; set; }

        // Employee Dashboard
        public PresensiResponse? PresensiHariIni { get; set; }
        public List<CutiResponse> CutiTerbaru { get; set; } = new();
        public int TotalCutiMenunggu { get; set; }

        // HRD Dashboard
        public int TotalKaryawan { get; set; }
        public int KaryawanHadirHariIni { get; set; }
        public int CutiMenungguApproval { get; set; }
        public decimal AbsensiRate { get; set; }
        public List<CutiResponse> CutiMenungguList { get; set; } = new();
    }
}