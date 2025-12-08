using GymManagementSystem.Models.ViewModels;
using GymManagementSystem.Models.Enums;

namespace GymManagementSystem.Services
{
    public interface IAiService
    {
        Task<string> GetPersonalizedRecommendation(AiSuggestionViewModel model);
    }

    public class GeminiAiService : IAiService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeminiAiService> _logger;

        public GeminiAiService(IConfiguration configuration, HttpClient httpClient, ILogger<GeminiAiService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetPersonalizedRecommendation(AiSuggestionViewModel model)
        {
            try
            {
                var apiKey = _configuration["AiSettings:GeminiApiKey"];

                // API anahtarı kontrolü
                if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
                {
                    return GenerateMockRecommendation(model);
                }

                // Prompt oluştur
                var prompt = BuildPrompt(model);
                var endpoint = _configuration["AiSettings:ApiEndpoint"];

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var response = await _httpClient.PostAsJsonAsync($"{endpoint}?key={apiKey}", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    
                    try
                    {
                        // Gemini API response parsing
                        var jsonDoc = System.Text.Json.JsonDocument.Parse(result);
                        var candidates = jsonDoc.RootElement.GetProperty("candidates");
                        if (candidates.GetArrayLength() > 0)
                        {
                            var content = candidates[0].GetProperty("content");
                            var parts = content.GetProperty("parts");
                            if (parts.GetArrayLength() > 0)
                            {
                                var text = parts[0].GetProperty("text").GetString();
                                if (!string.IsNullOrEmpty(text))
                                {
                                    // AI response'u HTML formatına çevir
                                    return FormatAiResponse(text, model);
                                }
                            }
                        }
                    }
                    catch (Exception parseEx)
                    {
                        _logger.LogError(parseEx, "Gemini API response parse hatası");
                    }
                    
                    // Parse edilemezse mock response dön
                    return GenerateMockRecommendation(model);
                }
                else
                {
                    _logger.LogError($"Gemini API error: {response.StatusCode}");
                    return GenerateMockRecommendation(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI recommendation hatası");
                return GenerateMockRecommendation(model);
            }
        }

        private string BuildPrompt(AiSuggestionViewModel model)
        {
            var bmi = model.Weight / (model.Height * model.Height);

            var genderText = model.Gender == Gender.Erkek ? "Erkek" : "Kadın";
            var bodyTypeText = GetBodyTypeDescription(model.BodyType);
            var activityText = GetActivityLevelDescription(model.ActivityLevel);
            var goalText = GetGoalDescription(model.Goal);
            var workoutPlaceText = GetWorkoutPlaceDescription(model.WorkoutPlace);

            return $@"Sen profesyonel bir fitness koçusun. Aşağıdaki özelliklere sahip kullanıcı için 1 haftalık örnek antrenman ve beslenme programı hazırla:

**Kullanıcı Profili:**
- Cinsiyet/Yaş: {genderText}, {model.Age} yaşında
- Boy/Kilo: {model.Height:F2}m, {model.Weight}kg (BMI: {bmi:F2})
- Vücut Tipi: {bodyTypeText}
- Günlük Aktivite: {activityText}
- Hedef: {goalText}
- Antrenman Yeri: {workoutPlaceText}
- Müsaitlik: Haftada {model.DaysPerWeek} gün
- Ekstra Notlar: {model.UserNotes ?? "Yok"}

Lütfen aşağıdaki formatta yanıt ver:

1. **Durum Analizi:** BMI değerlendirmesi ve genel durum
2. **Haftalık Antrenman Programı:** {model.DaysPerWeek} günlük detaylı program (hangi gün ne yapacak, setler, tekrarlar)
3. **Beslenme Planı:** Günlük kalori hedefi, makro dağılımı, örnek öğünler
4. **Önemli Notlar:** Su tüketimi, dinlenme, uyku vb.
5. **Motivasyon Mesajı**

Türkçe, profesyonel ama samimi bir dille yaz.";
        }

        private string FormatAiResponse(string aiText, AiSuggestionViewModel model)
        {
            // AI'dan gelen metni HTML'e çevir
            var bmi = model.Weight / (model.Height * model.Height);
            
            // Basit markdown → HTML dönüşümü
            var html = aiText.Replace("\n", "<br/>");

            return $@"<div class='ai-recommendation'>
<h2>🤖 Gemini AI Tarafından Oluşturuldu</h2>

<div class='alert alert-info'>
    <strong>📊 Profiliniz:</strong> {model.Gender}, {model.Age} yaş, {model.Weight}kg, {model.Height:F2}m (BMI: {bmi:F2})
</div>

<div class='ai-content' style='white-space: pre-wrap; font-family: Arial;'>
{html}
</div>

<hr/>
<p class='text-muted'><em>✨ Bu öneri Google Gemini AI tarafından oluşturulmuştur.</em></p>
</div>";
        }

        private string GenerateMockRecommendation(AiSuggestionViewModel model)
        {
            var bmi = model.Weight / (model.Height * model.Height);
            var bmiCategory = bmi switch
            {
                < 18.5m => "zayıf",
                >= 18.5m and < 25m => "normal kiloda",
                >= 25m and < 30m => "hafif kilolu",
                _ => "obez"
            };

            var genderText = model.Gender == Gender.Erkek ? "Erkek" : "Kadın";
            var dailyCalories = CalculateDailyCalories(model, bmi);
            var workoutPlan = GenerateWorkoutPlan(model);

            return $@"<div class='ai-recommendation'>
<h2>🏋️ Kişiselleştirilmiş Fitness Programınız</h2>

<div class='alert alert-primary'>
    <h4>📊 Durum Analizi</h4>
    <ul>
        <li><strong>Profil:</strong> {genderText}, {model.Age} yaşında</li>
        <li><strong>Boy/Kilo:</strong> {model.Height:F2}m / {model.Weight}kg</li>
        <li><strong>BMI:</strong> {bmi:F2} ({bmiCategory})</li>
        <li><strong>Vücut Tipi:</strong> {GetBodyTypeDescription(model.BodyType)}</li>
        <li><strong>Hedef:</strong> {GetGoalDescription(model.Goal)}</li>
    </ul>
</div>

<h4>💪 {model.DaysPerWeek} Günlük Haftalık Antrenman Programı</h4>
{workoutPlan}

<h4>🥗 Beslenme Planı</h4>
<div class='alert alert-success'>
    <p><strong>Günlük Kalori Hedefi:</strong> ~{dailyCalories} kalori</p>
    <p><strong>Makro Dağılımı:</strong></p>
    <ul>
        <li>Protein: {GetProteinAmount(model)}g (Kas gelişimi için)</li>
        <li>Karbonhidrat: {GetCarbsAmount(model, dailyCalories)}g (Enerji için)</li>
        <li>Yağ: {GetFatAmount(dailyCalories)}g (Hormon dengesi için)</li>
    </ul>
</div>

<h5>📅 Örnek Günlük Öğün Planı:</h5>
{GenerateMealPlan(model)}

<div class='alert alert-info'>
    <h5>💡 Önemli Notlar</h5>
    <ul>
        <li>💧 <strong>Su:</strong> Günde en az 2.5-3 litre su için</li>
        <li>😴 <strong>Uyku:</strong> 7-8 saat kaliteli uyku</li>
        <li>🔥 <strong>Isınma:</strong> Her antrenmana 5-10 dk dinamik ısınma ile başlayın</li>
        <li>🧘 <strong>Dinlenme:</strong> Kaslar dinlenme sırasında gelişir, ara günlerde aktif dinlenme yapın</li>
        {(string.IsNullOrEmpty(model.UserNotes) ? "" : $"<li>⚠️ <strong>Notunuz:</strong> {model.UserNotes}</li>")}
    </ul>
</div>

<div class='alert alert-warning'>
    <h4>🎯 Motivasyon & İpuçları</h4>
    <p>{GetMotivationalMessage(model.Goal)}</p>
    <p><strong>Unutmayın:</strong> Tutarlılık başarının anahtarıdır. Mükemmel program değil, sürekli yapılan program sonuç verir! 💪</p>
</div>

<hr/>
<p class='text-muted'><em>⚠️ Bu öneri AI tarafından oluşturulmuş genel bir rehberdir. Kişiselleştirilmiş program için antrenörlerimizle görüşebilirsiniz.</em></p>
</div>";
        }

        private string GenerateWorkoutPlan(AiSuggestionViewModel model)
        {
            var days = new[] { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi", "Pazar" };
            var workoutDays = days.Take(model.DaysPerWeek).ToArray();
            
            var plan = "<ul>";
            
            for (int i = 0; i < model.DaysPerWeek; i++)
            {
                var dayName = workoutDays[i];
                var workout = GetWorkoutForDay(i, model);
                plan += $"<li><strong>{dayName}:</strong> {workout}</li>";
            }
            
            plan += "</ul>";
            return plan;
        }

        private string GetWorkoutForDay(int dayIndex, AiSuggestionViewModel model)
        {
            var place = model.WorkoutPlace;
            var goal = model.Goal;

            return (dayIndex % 3) switch
            {
                0 => place == WorkoutPlace.SporSalonu 
                    ? "Üst Vücut (Bench press 4x8, Pull-up 3x10, Shoulder press 3x10, Triceps 3x12)" 
                    : "Üst Vücut (Şınav 4x max, Reverse row 3x12, Pike push-up 3x10)",
                1 => "Alt Vücut (Squat 4x10, Lunges 3x12, Leg curl 3x10, Calf raise 3x15) + 20dk kardiyo",
                _ => goal == FitnessGoal.KondisyonArtirmak 
                    ? "Kardiyo & Core (30dk HIIT, Plank 3x60sn, Russian twist 3x20)" 
                    : "Full Body (Compound hareketler, 45-60dk)"
            };
        }

        private string GenerateMealPlan(AiSuggestionViewModel model)
        {
            return @"<ul>
    <li><strong>Sabah (07:00):</strong> Yumurta (3 adet) + Kepekli ekmek (2 dilim) + Avokado + Süt</li>
    <li><strong>Ara Öğün (10:30):</strong> Meyveli yoğurt + Kuruyemiş (1 avuç)</li>
    <li><strong>Öğle (13:00):</strong> Izgara tavuk/balık (200g) + Bulgur pilavı + Salata</li>
    <li><strong>Ara Öğün (16:00):</strong> Protein shake + Muz</li>
    <li><strong>Akşam (19:00):</strong> Sebze yemeği + Tavuk/Et + Yoğurt</li>
    <li><strong>Gece (Opsiyonel):</strong> Az yağlı süt + Badem</li>
</ul>";
        }

        private int CalculateDailyCalories(AiSuggestionViewModel model, decimal bmi)
        {
            // BMR hesaplama (Harris-Benedict)
            decimal bmr = model.Gender == Gender.Erkek
                ? 88.362m + (13.397m * model.Weight) + (4.799m * model.Height * 100) - (5.677m * model.Age)
                : 447.593m + (9.247m * model.Weight) + (3.098m * model.Height * 100) - (4.330m * model.Age);

            // Aktivite faktörü
            decimal activityFactor = model.ActivityLevel switch
            {
                ActivityLevel.Hareketsiz => 1.2m,
                ActivityLevel.AzHareketli => 1.375m,
                ActivityLevel.OrtaSeviye => 1.55m,
                ActivityLevel.CokHareketli => 1.725m,
                _ => 1.4m
            };

            var tdee = bmr * activityFactor;

            // Hedefe göre ayarlama
            return model.Goal switch
            {
                FitnessGoal.KiloVermek => (int)(tdee - 500),
                FitnessGoal.KiloAlmak => (int)(tdee + 300),
                FitnessGoal.KondisyonArtirmak => (int)tdee,
                _ => (int)tdee
            };
        }

        private int GetProteinAmount(AiSuggestionViewModel model)
        {
            return (int)(model.Weight * 2);
        }

        private int GetCarbsAmount(AiSuggestionViewModel model, int calories)
        {
            return model.Goal == FitnessGoal.KiloVermek 
                ? (int)(calories * 0.35 / 4)
                : (int)(calories * 0.45 / 4);
        }

        private int GetFatAmount(int calories)
        {
            return (int)(calories * 0.25 / 9);
        }

        private string GetMotivationalMessage(FitnessGoal goal)
        {
            return goal switch
            {
                FitnessGoal.KiloVermek => "Kilo vermek bir maraton, sprint değil! Günde 500 kalori açık vermek ayda ~2kg sağlıklı kilo kaybı sağlar. Sabırlı olun! 🔥",
                FitnessGoal.KiloAlmak => "Kas yapmak için ağır kaldırın, bol protein tüketin ve dinlenin. Süreç zaman alır ama sonuçlar kalıcıdır! 💪",
                FitnessGoal.FormKorumak => "Formda kalmak yaşam tarzıdır. Dengeli beslenme ve düzenli antrenmanla hedeflerinizi koruyun! ⚖️",
                FitnessGoal.KondisyonArtirmak => "Kondisyon her şeyin temelidir! Küçük adımlarla başlayın, her hafta biraz daha zorlayın. 🏃",
                _ => "Her gün bir adım ileri! Başarı sabır ve disiplin ister."
            };
        }

        private string GetBodyTypeDescription(BodyType type)
        {
            return type switch
            {
                BodyType.Ektomorf => "Ektomorf (Zayıf yapılı, hızlı metabolizma)",
                BodyType.Mezomorf => "Mezomorf (Atletik yapılı, kolay kas yapar)",
                BodyType.Endomorf => "Endomorf (Geniş yapılı, çabuk kilo alır)",
                _ => type.ToString()
            };
        }

        private string GetActivityLevelDescription(ActivityLevel level)
        {
            return level switch
            {
                ActivityLevel.Hareketsiz => "Hareketsiz (Masa başı işi)",
                ActivityLevel.AzHareketli => "Az Hareketli (Haftada 1-2 gün spor)",
                ActivityLevel.OrtaSeviye => "Orta Seviye (Haftada 3-4 gün spor)",
                ActivityLevel.CokHareketli => "Çok Hareketli (Haftada 5+ gün spor)",
                _ => level.ToString()
            };
        }

        private string GetGoalDescription(FitnessGoal goal)
        {
            return goal switch
            {
                FitnessGoal.KiloVermek => "Kilo Vermek (Yağ yakma)",
                FitnessGoal.KiloAlmak => "Kilo Almak (Kas yapma/Bulk)",
                FitnessGoal.FormKorumak => "Form Korumak (Maintenance)",
                FitnessGoal.KondisyonArtirmak => "Kondisyon Artırmak",
                _ => goal.ToString()
            };
        }

        private string GetWorkoutPlaceDescription(WorkoutPlace place)
        {
            return place switch
            {
                WorkoutPlace.SporSalonu => "Spor Salonu (Ekipman mevcut)",
                WorkoutPlace.EvVucutAgirligi => "Ev (Vücut ağırlığı ile)",
                WorkoutPlace.EvDambilSeti => "Ev (Dambıl seti ile)",
                _ => place.ToString()
            };
        }
    }
}
