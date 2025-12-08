using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Services;

namespace GymManagementSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppointmentService _appointmentService;

        public ReportsController(ApplicationDbContext context, IAppointmentService appointmentService)
        {
            _context = context;
            _appointmentService = appointmentService;
        }

        /// <summary>
        /// Belirli tarih ve saatte müsait antrenörleri getirir
        /// </summary>
        /// <param name="date">Randevu tarihi (yyyy-MM-dd)</param>
        /// <param name="startTime">Başlangıç saati (HH:mm)</param>
        /// <param name="durationMinutes">Randevu süresi (dakika)</param>
        /// <returns>Müsait antrenörler listesi</returns>
        [HttpGet("available-trainers")]
        public async Task<IActionResult> GetAvailableTrainers(
            [FromQuery] DateTime date,
            [FromQuery] string startTime,
            [FromQuery] int durationMinutes = 60)
        {
            try
            {
                if (!TimeSpan.TryParse(startTime, out var parsedTime))
                {
                    return BadRequest(new { error = "Geçersiz saat formatı. Örnek: 09:00" });
                }

                var availableTrainers = await _appointmentService.GetAvailableTrainers(date, parsedTime, durationMinutes);

                var result = availableTrainers.Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Email,
                    t.PhoneNumber,
                    t.HourlyRate,
                    Specialties = t.Specialties.Select(s => s.SpecialtyName).ToList()
                });

                return Ok(new
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    StartTime = startTime,
                    DurationMinutes = durationMinutes,
                    AvailableTrainersCount = result.Count(),
                    Trainers = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Bir hata oluştu.", details = ex.Message });
            }
        }

        /// <summary>
        /// Aylık randevu istatistiklerini getirir
        /// </summary>
        /// <param name="year">Yıl</param>
        /// <param name="month">Ay (1-12)</param>
        /// <returns>Aylık randevu istatistikleri</returns>
        [HttpGet("monthly-stats")]
        public async Task<IActionResult> GetMonthlyStats([FromQuery] int year, [FromQuery] int month)
        {
            try
            {
                if (month < 1 || month > 12)
                {
                    return BadRequest(new { error = "Ay değeri 1-12 arasında olmalıdır." });
                }

                var appointments = await _context.Appointments
                    .Where(a => a.AppointmentDate.Year == year && a.AppointmentDate.Month == month)
                    .ToListAsync();

                var stats = new
                {
                    Year = year,
                    Month = month,
                    TotalAppointments = appointments.Count,
                    StatusBreakdown = appointments
                        .GroupBy(a => a.Status)
                        .Select(g => new { Status = g.Key, Count = g.Count() })
                        .ToList(),
                    ServiceBreakdown = await _context.Appointments
                        .Where(a => a.AppointmentDate.Year == year && a.AppointmentDate.Month == month)
                        .Include(a => a.Service)
                        .GroupBy(a => a.Service.Name)
                        .Select(g => new { ServiceName = g.Key, Count = g.Count() })
                        .ToListAsync(),
                    TotalRevenue = await _context.Appointments
                        .Where(a => a.AppointmentDate.Year == year
                            && a.AppointmentDate.Month == month
                            && (a.Status == "Completed" || a.Status == "Approved"))
                        .Include(a => a.Service)
                        .SumAsync(a => a.Service.Price)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Bir hata oluştu.", details = ex.Message });
            }
        }

        /// <summary>
        /// Tüm hizmetleri listeler
        /// </summary>
        /// <param name="activeOnly">Sadece aktif hizmetler</param>
        /// <returns>Hizmetler listesi</returns>
        [HttpGet("services")]
        public async Task<IActionResult> GetServices([FromQuery] bool activeOnly = true)
        {
            try
            {
                var query = _context.Services.AsQueryable();

                if (activeOnly)
                {
                    query = query.Where(s => s.IsActive);
                }

                var services = await query
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.Description,
                        s.DurationMinutes,
                        s.Price,
                        s.IsActive
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Count = services.Count,
                    Services = services
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Bir hata oluştu.", details = ex.Message });
            }
        }

        /// <summary>
        /// Antrenör bazında randevu sayısını getirir
        /// </summary>
        /// <returns>Antrenör istatistikleri</returns>
        [HttpGet("trainer-stats")]
        public async Task<IActionResult> GetTrainerStats()
        {
            try
            {
                var trainerStats = await _context.Trainers
                    .Select(t => new
                    {
                        TrainerId = t.Id,
                        TrainerName = t.FullName,
                        TotalAppointments = t.Appointments.Count(),
                        PendingAppointments = t.Appointments.Count(a => a.Status == "Pending"),
                        CompletedAppointments = t.Appointments.Count(a => a.Status == "Completed"),
                        Specialties = t.Specialties.Select(s => s.SpecialtyName).ToList()
                    })
                    .ToListAsync();

                return Ok(new
                {
                    TotalTrainers = trainerStats.Count,
                    Stats = trainerStats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Bir hata oluştu.", details = ex.Message });
            }
        }
    }
}
