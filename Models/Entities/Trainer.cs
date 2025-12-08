using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Models.Entities
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        public decimal HourlyRate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public ICollection<TrainerSpecialty> Specialties { get; set; } = new List<TrainerSpecialty>();
        public ICollection<TrainerAvailability> Availabilities { get; set; } = new List<TrainerAvailability>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
