using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [RegularExpression(@"^\+90 \d{3}-\d{3}-\d{2}-\d{2}$", ErrorMessage = "Telefon numarası formatı: +90 xxx-xxx-xx-xx")]
        public new string? PhoneNumber { get; set; }
        public decimal? Weight { get; set; }  // kg
        public decimal? Height { get; set; }  // meter
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<AiRecommendation> AiRecommendations { get; set; } = new List<AiRecommendation>();
    }
}
