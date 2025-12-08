using Microsoft.AspNetCore.Identity;
using GymManagementSystem.Models.Entities;

namespace GymManagementSystem.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Member"))
            {
                await roleManager.CreateAsync(new IdentityRole("Member"));
            }

            // Seed Admin User
            var adminEmail = "g211210004@sakarya.edu.tr";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "sau");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed Services
            if (!context.Services.Any())
            {
                var services = new List<Service>
                {
                    new Service
                    {
                        Name = "Yoga",
                        Description = "Rahatlatıcı yoga seansları",
                        DurationMinutes = 60,
                        Price = 200,
                        IsActive = true
                    },
                    new Service
                    {
                        Name = "Pilates",
                        Description = "Kas tonlaması ve esneklik için pilates",
                        DurationMinutes = 60,
                        Price = 250,
                        IsActive = true
                    },
                    new Service
                    {
                        Name = "Fitness",
                        Description = "Genel fitness ve kondisyon antrenmanı",
                        DurationMinutes = 90,
                        Price = 300,
                        IsActive = true
                    },
                    new Service
                    {
                        Name = "Kişisel Antrenman",
                        Description = "Birebir kişisel antrenman",
                        DurationMinutes = 60,
                        Price = 400,
                        IsActive = true
                    }
                };

                context.Services.AddRange(services);
                await context.SaveChangesAsync();
            }

            // Seed Trainers
            if (!context.Trainers.Any())
            {
                var trainers = new List<Trainer>
                {
                    new Trainer
                    {
                        FullName = "Ahmet Yılmaz",
                        Email = "ahmet@gym.com",
                        PhoneNumber = "555-0001",
                        Bio = "10 yıllık deneyime sahip profesyonel fitness antrenörü",
                        HourlyRate = 300
                    },
                    new Trainer
                    {
                        FullName = "Ayşe Kara",
                        Email = "ayse@gym.com",
                        PhoneNumber = "555-0002",
                        Bio = "Yoga ve pilates uzmanı",
                        HourlyRate = 250
                    },
                    new Trainer
                    {
                        FullName = "Mehmet Demir",
                        Email = "mehmet@gym.com",
                        PhoneNumber = "555-0003",
                        Bio = "Kilo verme ve kas yapma konusunda uzman",
                        HourlyRate = 350
                    }
                };

                context.Trainers.AddRange(trainers);
                await context.SaveChangesAsync();

                // Seed Trainer Specialties
                var specialties = new List<TrainerSpecialty>
                {
                    new TrainerSpecialty { TrainerId = 1, SpecialtyName = "Kilo Verme" },
                    new TrainerSpecialty { TrainerId = 1, SpecialtyName = "Kas Yapma" },
                    new TrainerSpecialty { TrainerId = 2, SpecialtyName = "Yoga" },
                    new TrainerSpecialty { TrainerId = 2, SpecialtyName = "Pilates" },
                    new TrainerSpecialty { TrainerId = 3, SpecialtyName = "Kilo Verme" },
                    new TrainerSpecialty { TrainerId = 3, SpecialtyName = "Vücut Geliştirme" }
                };

                context.TrainerSpecialties.AddRange(specialties);
                await context.SaveChangesAsync();

                // Seed Trainer Availabilities
                var availabilities = new List<TrainerAvailability>();
                
                // Ahmet - Pazartesi, Çarşamba, Cuma 09:00-17:00
                availabilities.AddRange(new[]
                {
                    new TrainerAvailability { TrainerId = 1, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) },
                    new TrainerAvailability { TrainerId = 1, DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) },
                    new TrainerAvailability { TrainerId = 1, DayOfWeek = DayOfWeek.Friday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) }
                });

                // Ayşe - Salı, Perşembe 10:00-18:00
                availabilities.AddRange(new[]
                {
                    new TrainerAvailability { TrainerId = 2, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(18, 0, 0) },
                    new TrainerAvailability { TrainerId = 2, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(18, 0, 0) }
                });

                // Mehmet - Her gün 08:00-16:00
                for (int i = 0; i < 7; i++)
                {
                    availabilities.Add(new TrainerAvailability
                    {
                        TrainerId = 3,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(8, 0, 0),
                        EndTime = new TimeSpan(16, 0, 0)
                    });
                }

                context.TrainerAvailabilities.AddRange(availabilities);
                await context.SaveChangesAsync();
            }
        }
    }
}
