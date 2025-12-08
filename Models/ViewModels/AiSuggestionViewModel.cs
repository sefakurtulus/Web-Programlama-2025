using System.ComponentModel.DataAnnotations;
using GymManagementSystem.Models.Enums;

namespace GymManagementSystem.Models.ViewModels
{
    public class AiSuggestionViewModel
    {
        // Temel Bilgiler
        [Required(ErrorMessage = "Yaş gereklidir.")]
        [Range(15, 100, ErrorMessage = "Yaş 15-100 arasında olmalıdır.")]
        [Display(Name = "Yaş")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Cinsiyet seçimi zorunludur.")]
        [Display(Name = "Cinsiyet")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Kilo gereklidir.")]
        [Range(30, 300, ErrorMessage = "Kilo 30-300 kg arasında olmalıdır.")]
        [Display(Name = "Kilo (kg)")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Boy gereklidir.")]
        [Range(1.0, 2.5, ErrorMessage = "Boy 1.0-2.5 metre arasında olmalıdır.")]
        [Display(Name = "Boy (metre)")]
        public decimal Height { get; set; }

        // Vücut ve Aktivite
        [Required(ErrorMessage = "Vücut tipi seçimi zorunludur.")]
        [Display(Name = "Vücut Tipi")]
        public BodyType BodyType { get; set; }

        [Required(ErrorMessage = "Aktivite seviyesi seçimi zorunludur.")]
        [Display(Name = "Aktivite Seviyesi")]
        public ActivityLevel ActivityLevel { get; set; }

        // Hedef ve Program
        [Required(ErrorMessage = "Hedef seçimi zorunludur.")]
        [Display(Name = "Hedef")]
        public FitnessGoal Goal { get; set; }

        [Required(ErrorMessage = "Antrenman yeri seçimi zorunludur.")]
        [Display(Name = "Antrenman Yeri")]
        public WorkoutPlace WorkoutPlace { get; set; }

        [Required(ErrorMessage = "Haftada kaç gün antrenman yapacağınızı belirtin.")]
        [Range(1, 7, ErrorMessage = "Haftada 1-7 gün arası olmalıdır.")]
        [Display(Name = "Haftada Kaç Gün")]
        public int DaysPerWeek { get; set; } = 3;

        // Ekstra Bilgiler
        [MaxLength(1000)]
        [Display(Name = "Ekstra Belirtmek İstedikleriniz (Sakatlık, Allerji vb.)")]
        public string? UserNotes { get; set; }
    }
}
