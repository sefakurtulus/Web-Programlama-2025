using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagementSystem.Models.Entities
{
    public class TrainerSpecialty
    {
        public int Id { get; set; }

        [Required]
        public int TrainerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string SpecialtyName { get; set; } = string.Empty; // Kilo verme, Kas yapma, vb.

        // Navigation Property
        [ForeignKey("TrainerId")]
        public Trainer Trainer { get; set; } = null!;
    }
}
