using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.Models.ViewModels
{
    public class CreateAppointmentViewModel
    {
        [Required(ErrorMessage = "Hizmet seçimi zorunludur.")]
        [Display(Name = "Hizmet")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Antrenör seçimi zorunludur.")]
        [Display(Name = "Antrenör")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Tarih seçimi zorunludur.")]
        [Display(Name = "Randevu Tarihi")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Saat seçimi zorunludur.")]
        [Display(Name = "Başlangıç Saati")]
        public TimeSpan StartTime { get; set; } = new TimeSpan(9, 0, 0);

        [MaxLength(500)]
        [Display(Name = "Notlar (İsteğe Bağlı)")]
        public string? Notes { get; set; }
    }
}
