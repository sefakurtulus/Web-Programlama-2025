using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Models.Entities;

namespace GymManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
        public DbSet<TrainerSpecialty> TrainerSpecialties { get; set; }
        public DbSet<AiRecommendation> AiRecommendations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Trainer configuration
            modelBuilder.Entity<Trainer>()
                .HasIndex(t => t.Email)
                .IsUnique();

            // Service configuration
            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

            // Appointment configuration
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Trainer)
                .WithMany(t => t.Appointments)
                .HasForeignKey(a => a.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // TrainerAvailability configuration
            modelBuilder.Entity<TrainerAvailability>()
                .HasOne(ta => ta.Trainer)
                .WithMany(t => t.Availabilities)
                .HasForeignKey(ta => ta.TrainerId)
                .OnDelete(DeleteBehavior.Cascade);

            // TrainerSpecialty configuration
            modelBuilder.Entity<TrainerSpecialty>()
                .HasOne(ts => ts.Trainer)
                .WithMany(t => t.Specialties)
                .HasForeignKey(ts => ts.TrainerId)
                .OnDelete(DeleteBehavior.Cascade);

            // AiRecommendation configuration
            modelBuilder.Entity<AiRecommendation>()
                .HasOne(ar => ar.User)
                .WithMany(u => u.AiRecommendations)
                .HasForeignKey(ar => ar.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AiRecommendation>()
                .Property(ar => ar.DietPlan)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<AiRecommendation>()
                .Property(ar => ar.ExercisePlan)
                .HasColumnType("nvarchar(max)");

            // ApplicationUser configuration
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Weight)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Height)
                .HasPrecision(3, 2);

            // Trainer HourlyRate precision
            modelBuilder.Entity<Trainer>()
                .Property(t => t.HourlyRate)
                .HasPrecision(18, 2);
        }
    }
}
