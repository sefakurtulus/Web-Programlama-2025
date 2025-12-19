using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models.Entities;

namespace GymManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var stats = new
            {
                TotalMembers = await _context.Users.CountAsync(),
                TotalTrainers = await _context.Trainers.CountAsync(),
                TotalServices = await _context.Services.CountAsync(),
                TotalAppointments = await _context.Appointments.CountAsync(),
                PendingAppointments = await _context.Appointments.CountAsync(a => a.Status == "Pending"),
                TodayAppointments = await _context.Appointments.CountAsync(a => a.AppointmentDate.Date == DateTime.Today)
            };

            ViewBag.Stats = stats;

            // Son randevular
            var recentAppointments = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToListAsync();

            return View(recentAppointments);
        }

        #region Services Management

        // GET: Admin/Services
        public async Task<IActionResult> ManageServices()
        {
            var services = await _context.Services.ToListAsync();
            return View(services);
        }

        // GET: Admin/CreateService
        public IActionResult CreateService()
        {
            return View();
        }

        // POST: Admin/CreateService
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateService(Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Services.Add(service);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hizmet başarıyla eklendi.";
                return RedirectToAction(nameof(ManageServices));
            }
            return View(service);
        }

        // GET: Admin/EditService/5
        public async Task<IActionResult> EditService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            return View(service);
        }

        // POST: Admin/EditService/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(int id, Service service)
        {
            if (id != service.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Hizmet başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ServiceExists(service.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(ManageServices));
            }
            return View(service);
        }

        // POST: Admin/DeleteService/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                // Soft delete - IsActive = false
                service.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hizmet pasif hale getirildi.";
            }
            return RedirectToAction(nameof(ManageServices));
        }

        // GET: Admin/ToggleService/5
        public async Task<IActionResult> ToggleService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                // Toggle IsActive status
                service.IsActive = !service.IsActive;
                await _context.SaveChangesAsync();
                
                var statusText = service.IsActive ? "aktif" : "pasif";
                TempData["SuccessMessage"] = $"Hizmet {statusText} hale getirildi.";
            }
            return RedirectToAction(nameof(ManageServices));
        }

        #endregion

        #region Trainers Management

        // GET: Admin/Trainers
        public async Task<IActionResult> ManageTrainers()
        {
            var trainers = await _context.Trainers
                .Include(t => t.Specialties)
                .Include(t => t.Availabilities)
                .ToListAsync();
            return View(trainers);
        }

        // GET: Admin/CreateTrainer
        public IActionResult CreateTrainer()
        {
            return View();
        }

        // POST: Admin/CreateTrainer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrainer(Trainer trainer, string[] Specialties, string[] AvailableDays)
        {
            if (ModelState.IsValid)
            {
                // 1. Antrenörü kaydet
                _context.Trainers.Add(trainer);
                await _context.SaveChangesAsync();
                
                // 2. Uzmanlık alanlarını kaydet
                if (Specialties != null && Specialties.Length > 0)
                {
                    foreach (var specialty in Specialties)
                    {
                        var trainerSpecialty = new TrainerSpecialty
                        {
                            TrainerId = trainer.Id,
                            SpecialtyName = specialty
                        };
                        _context.TrainerSpecialties.Add(trainerSpecialty);
                    }
                }
                
                // 3. Müsaitlik saatlerini kaydet
                if (AvailableDays != null && AvailableDays.Length > 0)
                {
                    foreach (var dayString in AvailableDays)
                    {
                        // Form'dan gelen saat bilgilerini al
                        var startTimeKey = $"StartTimes_{dayString}";
                        var endTimeKey = $"EndTimes_{dayString}";
                        
                        var startTimeStr = Request.Form[startTimeKey].ToString();
                        var endTimeStr = Request.Form[endTimeKey].ToString();
                        
                        if (!string.IsNullOrEmpty(startTimeStr) && !string.IsNullOrEmpty(endTimeStr))
                        {
                            if (TimeSpan.TryParse(startTimeStr, out var startTime) && 
                                TimeSpan.TryParse(endTimeStr, out var endTime))
                            {
                                var availability = new TrainerAvailability
                                {
                                    TrainerId = trainer.Id,
                                    DayOfWeek = Enum.Parse<DayOfWeek>(dayString),
                                    StartTime = startTime,
                                    EndTime = endTime,
                                    IsAvailable = true
                                };
                                _context.TrainerAvailabilities.Add(availability);
                            }
                        }
                    }
                }
                
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Antrenör başarıyla eklendi.";
                return RedirectToAction(nameof(ManageTrainers));
            }
            return View(trainer);
        }

        // GET: Admin/EditTrainer/5
        public async Task<IActionResult> EditTrainer(int id)
        {
            var trainer = await _context.Trainers
                .Include(t => t.Specialties)
                .Include(t => t.Availabilities)
                .FirstOrDefaultAsync(t => t.Id == id);
                
            if (trainer == null)
                return NotFound();

            return View(trainer);
        }

        // POST: Admin/EditTrainer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainer(int id, Trainer trainer, string[] Specialties, string[] AvailableDays)
        {
            if (id != trainer.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Antrenör bilgilerini güncelle
                    _context.Update(trainer);
                    
                    // 2. Mevcut uzmanlıkları sil
                    var oldSpecialties = await _context.TrainerSpecialties
                        .Where(ts => ts.TrainerId == id)
                        .ToListAsync();
                    _context.TrainerSpecialties.RemoveRange(oldSpecialties);
                    
                    // 3. Yeni uzmanlıkları ekle
                    if (Specialties != null && Specialties.Length > 0)
                    {
                        foreach (var specialty in Specialties)
                        {
                            var trainerSpecialty = new TrainerSpecialty
                            {
                                TrainerId = trainer.Id,
                                SpecialtyName = specialty
                            };
                            _context.TrainerSpecialties.Add(trainerSpecialty);
                        }
                    }
                    
                    // 4. Mevcut müsaitlikleri sil
                    var oldAvailabilities = await _context.TrainerAvailabilities
                        .Where(ta => ta.TrainerId == id)
                        .ToListAsync();
                    _context.TrainerAvailabilities.RemoveRange(oldAvailabilities);
                    
                    // 5. Yeni müsaitlikleri ekle
                    if (AvailableDays != null && AvailableDays.Length > 0)
                    {
                        foreach (var dayString in AvailableDays)
                        {
                            var startTimeKey = $"StartTimes_{dayString}";
                            var endTimeKey = $"EndTimes_{dayString}";
                            
                            var startTimeStr = Request.Form[startTimeKey].ToString();
                            var endTimeStr = Request.Form[endTimeKey].ToString();
                            
                            if (!string.IsNullOrEmpty(startTimeStr) && !string.IsNullOrEmpty(endTimeStr))
                            {
                                if (TimeSpan.TryParse(startTimeStr, out var startTime) && 
                                    TimeSpan.TryParse(endTimeStr, out var endTime))
                                {
                                    var availability = new TrainerAvailability
                                    {
                                        TrainerId = trainer.Id,
                                        DayOfWeek = Enum.Parse<DayOfWeek>(dayString),
                                        StartTime = startTime,
                                        EndTime = endTime,
                                        IsAvailable = true
                                    };
                                    _context.TrainerAvailabilities.Add(availability);
                                }
                            }
                        }
                    }
                    
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Antrenör başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TrainerExists(trainer.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(ManageTrainers));
            }
            return View(trainer);
        }

        #endregion

        #region Appointments Management

        // GET: Admin/ManageAppointments
        public async Task<IActionResult> ManageAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.StartTime)
                .ToListAsync();

            return View(appointments);
        }

        // POST: Admin/ApproveAppointment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null && appointment.Status == "Pending")
            {
                appointment.Status = "Approved";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Randevu onaylandı.";
            }
            return RedirectToAction(nameof(ManageAppointments));
        }

        // POST: Admin/CompleteAppointment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null && appointment.Status == "Approved")
            {
                appointment.Status = "Completed";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Randevu tamamlandı olarak işaretlendi.";
            }
            return RedirectToAction(nameof(ManageAppointments));
        }

        #endregion

        private async Task<bool> ServiceExists(int id)
        {
            return await _context.Services.AnyAsync(e => e.Id == id);
        }

        private async Task<bool> TrainerExists(int id)
        {
            return await _context.Trainers.AnyAsync(e => e.Id == id);
        }
    }
}
