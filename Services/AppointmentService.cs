using GymManagementSystem.Data;
using GymManagementSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Services
{
    public interface IAppointmentService
    {
        Task<bool> IsTrainerAvailable(int trainerId, DateTime date, TimeSpan startTime, int durationMinutes);
        Task<List<Trainer>> GetAvailableTrainers(DateTime date, TimeSpan startTime, int durationMinutes);
        Task<bool> CreateAppointment(Appointment appointment);
        Task<List<Appointment>> GetUserAppointments(string userId);
        Task<Appointment?> GetAppointmentById(int id);
        Task<bool> CancelAppointment(int id, string userId);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsTrainerAvailable(int trainerId, DateTime date, TimeSpan startTime, int durationMinutes)
        {
            var endTime = startTime.Add(TimeSpan.FromMinutes(durationMinutes));
            var dayOfWeek = date.DayOfWeek;

            // 1. Antrenör o gün müsait mi?
            var availability = await _context.TrainerAvailabilities
                .FirstOrDefaultAsync(a => a.TrainerId == trainerId
                    && a.DayOfWeek == dayOfWeek
                    && a.IsAvailable
                    && a.StartTime <= startTime
                    && a.EndTime >= endTime);

            if (availability == null)
                return false;

            // 2. Çakışan randevu var mı?
            var hasConflict = await _context.Appointments
                .AnyAsync(a => a.TrainerId == trainerId
                    && a.AppointmentDate.Date == date.Date
                    && a.Status != "Cancelled"
                    && ((a.StartTime < endTime && a.EndTime > startTime)));

            return !hasConflict;
        }

        public async Task<List<Trainer>> GetAvailableTrainers(DateTime date, TimeSpan startTime, int durationMinutes)
        {
            var allTrainers = await _context.Trainers
                .Include(t => t.Specialties)
                .Include(t => t.Availabilities)
                .ToListAsync();

            var availableTrainers = new List<Trainer>();

            foreach (var trainer in allTrainers)
            {
                if (await IsTrainerAvailable(trainer.Id, date, startTime, durationMinutes))
                {
                    availableTrainers.Add(trainer);
                }
            }

            return availableTrainers;
        }

        public async Task<bool> CreateAppointment(Appointment appointment)
        {
            try
            {
                // Müsaitlik kontrolü
                var service = await _context.Services.FindAsync(appointment.ServiceId);
                if (service == null || !service.IsActive)
                    return false;

                var isAvailable = await IsTrainerAvailable(
                    appointment.TrainerId,
                    appointment.AppointmentDate,
                    appointment.StartTime,
                    service.DurationMinutes
                );

                if (!isAvailable)
                    return false;

                // EndTime hesapla
                appointment.EndTime = appointment.StartTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));
                appointment.Status = "Pending";
                appointment.CreatedAt = DateTime.Now;

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Appointment>> GetUserAppointments(string userId)
        {
            return await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentById(int id)
        {
            return await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> CancelAppointment(int id, string userId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (appointment == null || appointment.Status == "Cancelled" || appointment.Status == "Completed")
                return false;

            appointment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
