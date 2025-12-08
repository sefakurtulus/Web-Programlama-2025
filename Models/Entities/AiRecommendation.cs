using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagementSystem.Models.Entities
{
    public class AiRecommendation
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        
        [MaxLength(500)]
        public string? PhotoPath { get; set; }

        public string? DietPlan { get; set; }
        public string? ExercisePlan { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Property
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;
    }
}
