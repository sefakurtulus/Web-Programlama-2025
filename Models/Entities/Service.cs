using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Models.Entities
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(15, 240)]
        public int DurationMinutes { get; set; } // 15 dakika - 4 saat arası

        [Required]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
