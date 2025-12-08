using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models.Entities;
using GymManagementSystem.Models.ViewModels;
using GymManagementSystem.Services;

namespace GymManagementSystem.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAppointmentService appointmentService)
        {
            _context = context;
            _userManager = userManager;
            _appointmentService = appointmentService;
        }

        // GET: Appointments/MyAppointments
        public async Task<IActionResult> MyAppointments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var appointments = await _appointmentService.GetUserAppointments(user.Id);
            return View(appointments);
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Services = new SelectList(
                await _context.Services.Where(s => s.IsActive).ToListAsync(),
                "Id",
                "Name"
            );

            ViewBag.Trainers = new SelectList(
                await _context.Trainers.ToListAsync(),
                "Id",
                "FullName"
            );

            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return RedirectToAction("Login", "Account");

                var service = await _context.Services.FindAsync(model.ServiceId);
                if (service == null)
                {
                    ModelState.AddModelError("", "Geçersiz hizmet seçimi.");
                    await PopulateDropdowns();
                    return View(model);
                }

                // Geçmiş tarih kontrolü
                if (model.AppointmentDate.Date < DateTime.Today)
                {
                    ModelState.AddModelError("AppointmentDate", "Geçmiş bir tarih için randevu oluşturamazsınız.");
                    await PopulateDropdowns();
                    return View(model);
                }

                var appointment = new Appointment
                {
                    UserId = user.Id,
                    TrainerId = model.TrainerId,
                    ServiceId = model.ServiceId,
                    AppointmentDate = model.AppointmentDate,
                    StartTime = model.StartTime,
                    Notes = model.Notes
                };

                var success = await _appointmentService.CreateAppointment(appointment);

                if (success)
                {
                    TempData["SuccessMessage"] = "Randevunuz başarıyla oluşturuldu!";
                    return RedirectToAction(nameof(MyAppointments));
                }
                else
                {
                    ModelState.AddModelError("", "Seçilen tarih ve saatte antrenör müsait değil veya çakışan bir randevu var.");
                }
            }

            await PopulateDropdowns();
            return View(model);
        }

        // POST: Appointments/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var success = await _appointmentService.CancelAppointment(id, user.Id);

            if (success)
            {
                TempData["SuccessMessage"] = "Randevunuz iptal edildi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Randevu iptal edilemedi.";
            }

            return RedirectToAction(nameof(MyAppointments));
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentService.GetAppointmentById(id);

            if (appointment == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || appointment.UserId != user.Id)
                return Forbid();

            return View(appointment);
        }

        private async Task PopulateDropdowns()
        {
            ViewBag.Services = new SelectList(
                await _context.Services.Where(s => s.IsActive).ToListAsync(),
                "Id",
                "Name"
            );

            ViewBag.Trainers = new SelectList(
                await _context.Trainers.ToListAsync(),
                "Id",
                "FullName"
            );
        }
    }
}
