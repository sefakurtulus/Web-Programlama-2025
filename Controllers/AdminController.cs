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
        public async Task<IActionResult> CreateTrainer(Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Trainers.Add(trainer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Antrenör başarıyla eklendi.";
                return RedirectToAction(nameof(ManageTrainers));
            }
            return View(trainer);
        }

        // GET: Admin/EditTrainer/5
        public async Task<IActionResult> EditTrainer(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
                return NotFound();

            return View(trainer);
        }

        // POST: Admin/EditTrainer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainer(int id, Trainer trainer)
        {
            if (id != trainer.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
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
