using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HRD.Web.Models.DTOs;
using HRD.Web.Services;

namespace HRD.Web.Controllers
{
    [Authorize(Roles = "HRD")] // Only HRD can manage employees
    public class KaryawanController : Controller
    {
        private readonly ApiService _apiService;

        public KaryawanController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // List all employees
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _apiService.GetAsync<List<KaryawanResponse>>("api/karyawan");
                if (result?.Success == true)
                {
                    return View(result.Data ?? new List<KaryawanResponse>());
                }
                else
                {
                    ViewBag.Error = result?.Message ?? "Gagal memuat data karyawan";
                    return View(new List<KaryawanResponse>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<KaryawanResponse>());
            }
        }

        // Create new employee - GET
        public IActionResult Create()
        {
            // Set default values
            var model = new CreateKaryawanRequest
            {
                TglBergabung = DateTime.Today,
                GajiPokok = 5000000, // Default 5M
                Password = "password123" // Default password
            };

            return View(model);
        }

        // Create new employee - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateKaryawanRequest model)
        {
            // ===== DEEP DEBUG START =====
            Console.WriteLine("=== CREATE KARYAWAN DEBUG ===");
            Console.WriteLine($"Model.GajiPokok: {model.GajiPokok}");
            Console.WriteLine($"Model.Username: {model.Username}");
            Console.WriteLine($"Model.NamaLengkap: {model.NamaLengkap}");

            // Check raw form data
            Console.WriteLine("=== RAW FORM DATA ===");
            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"Form[{key}]: '{Request.Form[key]}'");
            }

            // Check ModelState for GajiPokok specifically
            Console.WriteLine("=== MODEL STATE DEBUG ===");
            if (ModelState.ContainsKey("GajiPokok"))
            {
                var gajiState = ModelState["GajiPokok"];
                Console.WriteLine($"GajiPokok.AttemptedValue: '{gajiState.AttemptedValue}'");
                Console.WriteLine($"GajiPokok.RawValue: '{gajiState.RawValue}'");
                Console.WriteLine($"GajiPokok.ValidationState: {gajiState.ValidationState}");
                if (gajiState.Errors.Any())
                {
                    Console.WriteLine($"GajiPokok.Errors: {string.Join(", ", gajiState.Errors.Select(e => e.ErrorMessage))}");
                }
            }
            else
            {
                Console.WriteLine("GajiPokok key NOT FOUND in ModelState!");
            }

            // Try manual parse from form
            if (Request.Form.ContainsKey("GajiPokok"))
            {
                var gajiFormValue = Request.Form["GajiPokok"].ToString();
                Console.WriteLine($"Direct form GajiPokok: '{gajiFormValue}'");

                if (decimal.TryParse(gajiFormValue, out decimal parsedGaji))
                {
                    Console.WriteLine($"Successfully parsed to: {parsedGaji}");
                    if (model.GajiPokok == 0 && parsedGaji > 0)
                    {
                        Console.WriteLine("FORCING model.GajiPokok from form value");
                        model.GajiPokok = parsedGaji;
                    }
                }
                else
                {
                    Console.WriteLine($"FAILED to parse '{gajiFormValue}' as decimal");
                }
            }
            else
            {
                Console.WriteLine("GajiPokok key NOT FOUND in Form!");
            }

            Console.WriteLine($"FINAL model.GajiPokok before validation: {model.GajiPokok}");
            Console.WriteLine("=== END DEBUG ===");
            // ===== DEEP DEBUG END =====

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is INVALID:");
                foreach (var kvp in ModelState)
                {
                    if (kvp.Value.Errors.Any())
                    {
                        Console.WriteLine($"  {kvp.Key}: {string.Join(", ", kvp.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
                return View(model);
            }

            try
            {
                // Debug the JSON being sent to API
                var json = System.Text.Json.JsonSerializer.Serialize(model, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                Console.WriteLine($"JSON PAYLOAD TO API:\n{json}");

                var result = await _apiService.PostAsync<KaryawanResponse>("api/karyawan", model);

                Console.WriteLine($"API Response Success: {result?.Success}");
                Console.WriteLine($"API Response Message: {result?.Message}");
                if (result?.Data != null)
                {
                    Console.WriteLine($"API Response GajiPokok: {result.Data.GajiPokok}");
                }

                if (result?.Success == true)
                {
                    TempData["SuccessMessage"] = $"Karyawan {model.NamaLengkap} berhasil ditambahkan";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = result?.Message ?? "Gagal menambahkan karyawan";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
                Console.WriteLine($"STACK TRACE: {ex.StackTrace}");
                ViewBag.Error = $"Error: {ex.Message}";
                return View(model);
            }
        }


        // View employee details
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var result = await _apiService.GetAsync<KaryawanResponse>($"api/karyawan/{id}");
                if (result?.Success == true && result.Data != null)
                {
                    return View(result.Data);
                }
                else
                {
                    TempData["ErrorMessage"] = result?.Message ?? "Data karyawan tidak ditemukan";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Edit employee - GET
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var result = await _apiService.GetAsync<KaryawanResponse>($"api/karyawan/{id}");
                if (result?.Success == true && result.Data != null)
                {
                    // Convert to update request model
                    var updateModel = new UpdateKaryawanRequest
                    {
                        NIK = result.Data.NIK,
                        NamaLengkap = result.Data.NamaLengkap,
                        EmailKantor = result.Data.EmailKantor,
                        NoTelepon = result.Data.NoTelepon,
                        Posisi = result.Data.Posisi,
                        Divisi = result.Data.Divisi,
                        TglBergabung = result.Data.TglBergabung,
                        GajiPokok = result.Data.GajiPokok,
                        StatusKaryawan = result.Data.StatusKaryawan
                    };

                    ViewBag.KaryawanId = id;
                    ViewBag.CurrentUsername = result.Data.Username;
                    ViewBag.CurrentEmail = result.Data.Email;

                    return View(updateModel);
                }
                else
                {
                    TempData["ErrorMessage"] = result?.Message ?? "Data karyawan tidak ditemukan";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Edit employee - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateKaryawanRequest model)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate ViewBag data for form
                try
                {
                    var currentResult = await _apiService.GetAsync<KaryawanResponse>($"api/karyawan/{id}");
                    if (currentResult?.Success == true && currentResult.Data != null)
                    {
                        ViewBag.KaryawanId = id;
                        ViewBag.CurrentUsername = currentResult.Data.Username;
                        ViewBag.CurrentEmail = currentResult.Data.Email;
                    }
                }
                catch { }

                return View(model);
            }

            try
            {
                var result = await _apiService.PutAsync<KaryawanResponse>($"api/karyawan/{id}", model);
                if (result?.Success == true)
                {
                    TempData["SuccessMessage"] = $"Data karyawan {model.NamaLengkap} berhasil diperbarui";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = result?.Message ?? "Gagal memperbarui data karyawan";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // Activate employee
        [HttpGet]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                var result = await _apiService.PostAsync<string>($"api/karyawan/{id}/activate", new { });
                if (result?.Success == true)
                {
                    TempData["SuccessMessage"] = "Karyawan berhasil diaktifkan";
                }
                else
                {
                    TempData["ErrorMessage"] = result?.Message ?? "Gagal mengaktifkan karyawan";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // Deactivate employee
        [HttpGet]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var result = await _apiService.PostAsync<string>($"api/karyawan/{id}/deactivate", new { });
                if (result?.Success == true)
                {
                    TempData["SuccessMessage"] = "Karyawan berhasil dinonaktifkan";
                }
                else
                {
                    TempData["ErrorMessage"] = result?.Message ?? "Gagal menonaktifkan karyawan";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // Toggle employee status (AJAX)
        [HttpPost]
        public async Task<IActionResult> ToggleStatus([FromBody] ToggleStatusRequest request)
        {
            try
            {
                var endpoint = request.Activate ?
                    $"api/karyawan/{request.Id}/activate" :
                    $"api/karyawan/{request.Id}/deactivate";

                var result = await _apiService.PostAsync<string>(endpoint, new { });

                if (result?.Success == true)
                {
                    var message = request.Activate ?
                        "Karyawan berhasil diaktifkan" :
                        "Karyawan berhasil dinonaktifkan";
                    return Json(new { success = true, message = message });
                }
                else
                {
                    return Json(new { success = false, message = result?.Message ?? "Gagal mengubah status karyawan" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Delete employee (soft delete)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _apiService.DeleteAsync($"api/karyawan/{id}");
                if (result?.Success == true)
                {
                    return Json(new { success = true, message = "Karyawan berhasil dihapus" });
                }
                else
                {
                    return Json(new { success = false, message = result?.Message ?? "Gagal menghapus karyawan" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Get employee statistics (for dashboard)
        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var result = await _apiService.GetAsync<List<KaryawanResponse>>("api/karyawan");
                if (result?.Success == true && result.Data != null)
                {
                    var stats = new
                    {
                        Total = result.Data.Count,
                        Aktif = result.Data.Count(k => k.StatusKaryawan == "Aktif"),
                        NonAktif = result.Data.Count(k => k.StatusKaryawan == "Non-Aktif"),
                        Divisi = result.Data.GroupBy(k => k.Divisi).Count(),
                        ByDivisi = result.Data.GroupBy(k => k.Divisi)
                            .Select(g => new { Divisi = g.Key, Count = g.Count() })
                            .ToList(),
                        ByPosisi = result.Data.GroupBy(k => k.Posisi)
                            .Select(g => new { Posisi = g.Key, Count = g.Count() })
                            .OrderByDescending(x => x.Count)
                            .Take(5)
                            .ToList()
                    };

                    return Json(new { success = true, data = stats });
                }
                else
                {
                    return Json(new { success = false, message = "Gagal mengambil statistik karyawan" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }

    // Helper class for toggle status request
    public class ToggleStatusRequest
    {
        public int Id { get; set; }
        public bool Activate { get; set; }
    }
}