using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public decimal? Weight { get; set; }  // kg
        public decimal? Height { get; set; }  // meter
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<AiRecommendation> AiRecommendations { get; set; } = new List<AiRecommendation>();
    }
}
