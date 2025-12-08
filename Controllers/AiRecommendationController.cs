using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models.Entities;
using GymManagementSystem.Models.ViewModels;
using GymManagementSystem.Services;

namespace GymManagementSystem.Controllers
{
    [Authorize]
    public class AiRecommendationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAiService _aiService;

        public AiRecommendationController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAiService aiService)
        {
            _context = context;
            _userManager = userManager;
            _aiService = aiService;
        }

        // GET: AiRecommendation/Index
        public IActionResult Index()
        {
            var model = new AiSuggestionViewModel
            {
                DaysPerWeek = 3 // Default value
            };
            return View(model);
        }

        // POST: AiRecommendation/Generate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(AiSuggestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            try
            {
                // AI'dan öneri al
                var recommendation = await _aiService.GetPersonalizedRecommendation(model);

                // Veritabanına kaydet
                var aiRecommendation = new AiRecommendation
                {
                    UserId = user.Id,
                    Weight = model.Weight,
                    Height = model.Height,
                    DietPlan = recommendation,
                    ExercisePlan = recommendation, // Aynı öneride hem diyet hem egzersiz var
                    CreatedAt = DateTime.Now
                };

                _context.AiRecommendations.Add(aiRecommendation);
                await _context.SaveChangesAsync();

                // Kullanıcının profilini güncelle
                user.Weight = model.Weight;
                user.Height = model.Height;
                await _userManager.UpdateAsync(user);

                ViewBag.Recommendation = recommendation;
                ViewBag.Weight = model.Weight;
                ViewBag.Height = model.Height;
                ViewBag.BMI = (model.Weight / (model.Height * model.Height)).ToString("F2");
                ViewBag.Model = model;

                return View("Result");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Öneri oluşturulurken bir hata oluştu: " + ex.Message);
                return View("Index", model);
            }
        }

        // GET: AiRecommendation/MyRecommendations
        public async Task<IActionResult> MyRecommendations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var recommendations = await _context.AiRecommendations
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(recommendations);
        }
    }
}
