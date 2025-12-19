# 🏋️ Gym Management System

Modern spor salonları için kapsamlı web tabanlı yönetim sistemi. ASP.NET Core 8.0 MVC, Entity Framework Core ve AI destekli kişiselleştirilmiş fitness önerileri ile geliştirilmiştir.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat-square&logo=bootstrap&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat-square&logo=microsoft-sql-server&logoColor=white)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)

---

## 📋 İçindekiler

- [Özellikler](#-özellikler)
- [Teknolojiler](#-teknolojiler)
- [Kurulum](#-kurulum)
- [Kullanım](#-kullanım)
- [API Dokümantasyonu](#-api-dokümantasyonu)
- [Veritabanı Yapısı](#-veritabanı-yapısı)
- [Ekran Görüntüleri](#-ekran-görüntüleri)
- [Katkıda Bulunma](#-katkıda-bulunma)
- [Lisans](#-lisans)

---

## ✨ Özellikler

### 👥 Kullanıcı Özellikleri
- ✅ **Kullanıcı Yönetimi**: Kayıt, giriş, profil yönetimi
- ✅ **Randevu Sistemi**: Antrenör ve hizmet seçimi ile dinamik randevu oluşturma
- ✅ **AI Fitness Önerileri**: Gemini AI ile kişiselleştirilmiş egzersiz programları
- ✅ **Randevu Takibi**: Geçmiş ve aktif randevuları görüntüleme

### 🔧 Admin Özellikleri
- ✅ **Dashboard**: Gerçek zamanlı istatistikler ve raporlar
- ✅ **Hizmet Yönetimi**: CRUD işlemleri, aktif/pasif durumu
- ✅ **Antrenör Yönetimi**: 
  - Uzmanlık alanları (Yoga, Pilates, Kilo Verme, vs.)
  - Haftalık müsaitlik takvimi (7 gün, saat aralıkları)
- ✅ **Randevu Yönetimi**: Onaylama, tamamlama, iptal işlemleri

### 🚀 REST API
- ✅ **4 Endpoint**: Müsait antrenörler, aylık istatistikler, hizmetler, antrenör istatistikleri
- ✅ **LINQ Sorguları**: Filtreleme, gruplama, aggregation
- ✅ **Swagger UI**: Otomatik API dokümantasyonu ve test arayüzü

### 🤖 AI Entegrasyonu
- ✅ **Google Gemini API**: Fitness önerileri için AI desteği
- ✅ **Kişiselleştirilmiş Programlar**: Yaş, kilo, boy, hedef ve deneyim seviyesine göre öneriler
- ✅ **Öneri Geçmişi**: Kullanıcının geçmiş AI önerilerini saklama

---

## 🛠️ Teknolojiler

### Backend
- **Framework**: ASP.NET Core 8.0 MVC
- **ORM**: Entity Framework Core 8.0
- **Veritabanı**: SQL Server 2019+
- **Authentication**: ASP.NET Identity (Role-based)
- **AI**: Google Gemini API 1.5 Pro

### Frontend
- **UI Framework**: Bootstrap 5.3
- **JavaScript**: Vanilla JS, jQuery Validation
- **View Engine**: Razor Pages
- **Responsive**: Mobile-first design

### Diğer
- **API Dok**: Swagger/OpenAPI 3.0
- **Validation**: Data Annotations
- **Dependency Injection**: Built-in ASP.NET Core DI

---

## 📦 Kurulum

### Gereksinimler
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server) veya SQL Server Express
- [Visual Studio 2022](https://visualstudio.microsoft.com/) veya [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### Adımlar

#### 1. Repository'yi Klonlayın
```bash
git clone https://github.com/[username]/GymManagementSystem.git
cd GymManagementSystem
```

#### 2. NuGet Paketlerini Yükleyin
```bash
dotnet restore
```

#### 3. Veritabanı Bağlantısını Ayarlayın
`appsettings.json` dosyasını düzenleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GymManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "GeminiApiKey": "YOUR_GEMINI_API_KEY_HERE"
}
```

> **Not**: Gemini API key almak için [Google AI Studio](https://makersuite.google.com/app/apikey) adresini ziyaret edin.

#### 4. Veritabanı Migration'ı Ç alıştırın
```bash
dotnet ef database update
```

Bu adım:
- Veritabanını oluşturur
- Tabloları oluşturur
- Örnek verileri ekler (admin hesabı, antrenörler, hizmetler)

#### 5. Uygulamayı Çalıştırın
```bash
dotnet run
```

Tarayıcınızda şu adresi açın: **http://localhost:5233**

---

## 🔑 İlk Giriş

### Admin Hesabı
- **Email**: `admin@gym.com`
- **Şifre**: `Admin123`

### Test Kullanıcısı Oluşturun
1. Ana sayfadan "Kayıt Ol" tıklayın
2. Bilgilerinizi doldurun
3. Otomatik giriş yapılacak

---

## 💻 Kullanım

### Kullanıcı (Üye) İşlemleri

#### Randevu Oluşturma
1. Navbar'dan **"Randevu Al"** tıklayın
2. **Hizmet** seçin (Yoga, Pilates, Fitness, Cardio)
3. **Antrenör** seçin
4. **Tarih** ve **Saat** seçin (müsait saatler otomatik yüklenir)
5. **"Randevu Oluştur"** butonuna tıklayın
6. Randevu "Beklemede" durumunda oluşturulur

#### AI Fitness Önerisi Alma
1. Navbar'dan **"AI Önerisi"** tıklayın
2. Formu doldurun:
   - Yaş, Cinsiyet
   - Kilo (kg), Boy (cm)
   - Fitness Hedefi (Kilo Verme / Kas Yapma / Dayanıklılık)
   - Deneyim Seviyesi (Başlangıç / Orta / İleri)
3. **"Öneri Al"** butonuna tıklayın
4. AI önerileri sayfa da gösterilir ve veritabanına kaydedilir

### Admin İşlemleri

#### Dashboard Görüntüleme
- Navbar'dan **"📊 Admin Panel"** → **"Dashboard"**
- İstatistikler: Toplam üye, antrenör, hizmet, randevu
- Bugünkü randevular
- Son 5 randevu

#### Hizmet Yönetimi
- **Hizmetleri Görüntüle**: Admin Panel → Hizmetler
- **Yeni Ekle**: "Yeni Hizmet Ekle" butonu
- **Düzenle**: "Düzenle" butonu
- **Aktif/Pasif**: Toggle butonu

#### Antrenör Yönetimi
- **Antrenörleri Görüntüle**: Admin Panel → Antrenörler
- **Yeni Ekle**: 
  1. Temel bilgiler (isim, email, telefon, ücret)
  2. Uzmanlık alanları (en az 1 checkbox seçilmeli)
  3. Müsaitlik takvimi (7 gün, checkbox + saat aralıkları)
- **Düzenle**: Mevcut veriler dolu gelir, güncelleyip kaydedin

#### Randevu Yönetimi
- **Tüm Randevular**: Admin Panel → Randevuları Yönet
- **Onayla**: "Pending" → "Approved" (yeşil buton)
- **Tamamla**: "Approved" → "Completed" (mavi buton)

---

## 📡 API Dokümantasyonu

### Swagger UI
API'yi test etmek için: **http://localhost:5233/swagger**

### Endpoint'ler

#### 1. Müsait Antrenörleri Getir
```http
GET /api/reports/available-trainers?date=2024-12-20&startTime=10:00&durationMinutes=60
```

**Response**:
```json
{
  "date": "2024-12-20",
  "startTime": "10:00",
  "durationMinutes": 60,
  "availableTrainersCount": 2,
  "trainers": [
    {
      "id": 1,
      "fullName": "Ahmet Yılmaz",
      "email": "ahmet@gym.com",
      "phoneNumber": "+90 555-000-10-01",
      "hourlyRate": 300.00,
      "specialties": ["Kilo Verme", "Kas Yapma"]
    }
  ]
}
```

#### 2. Aylık İstatistikler
```http
GET /api/reports/monthly-stats?year=2024&month=12
```

**Response**:
```json
{
  "year": 2024,
  "month": 12,
  "totalAppointments": 15,
  "statusBreakdown": [
    {"status": "Pending", "count": 5},
    {"status": "Approved", "count": 7},
    {"status": "Completed", "count": 3}
  ],
  "serviceBreakdown": [...],
  "totalRevenue": 4500
}
```

#### 3. Hizmet Listesi
```http
GET /api/reports/services?activeOnly=true
```

#### 4. Antrenör İstatistikleri
```http
GET /api/reports/trainer-stats
```

---

## 🗄️ Veritabanı Yapısı

### Tablolar (7 Tablo)

#### ApplicationUser (Kullanıcılar)
- **FullName**, Email, PhoneNumber (+90 xxx-xxx-xx-xx)
- Weight (kg), Height (cm) - AI için
- ASP.NET Identity genişletmesi

####  (Antrenörler)
- FullName, Email, PhoneNumber, Bio
- **HourlyRate** (Saatlik ücret)
- **Specialties** (One-to-Many)
- **Availabilities** (One-to-Many)

#### Service (Hizmetler)
- Name, Description
- **DurationMinutes** (15-240)
- Price, **IsActive** (Soft delete)

#### Appointment (Randevular)
- UserId, TrainerId, ServiceId (Foreign Keys)
- AppointmentDate, StartTime, EndTime
- **Status**: Pending / Approved / Completed / Cancelled
- Notes

#### TrainerSpecialty (Antrenör Uzmanlıkları)
- TrainerId, **SpecialtyName**
- Seçenekler: Kilo Verme, Kas Yapma, Yoga, Pilates, Cardio, Crossfit, Vücut Geliştirme, Zumba

#### TrainerAvailability (Antrenör Müsaitlik)
- TrainerId, **DayOfWeek** (Monday-Sunday)
- StartTime, EndTime, IsAvailable
- Haftalık çalışma takvimi

#### AiRecommendation (AI Önerileri)
- UserId, Age, Gender, FitnessGoal, ExperienceLevel
- Weight, Height, **Recommendations** (AI yanıtı)
- CreatedAt

### İlişkiler
```
ApplicationUser (1) ──────< (∞) Appointments
Trainer (1) ──────< (∞) Appointments  
Service (1) ──────< (∞) Appointments
Trainer (1) ──────< (∞) TrainerSpecialties
Trainer (1) ──────< (∞) TrainerAvailabilities
ApplicationUser (1) ──────< (∞) AiRecommendations
```

---

## 📸 Ekran Görüntüleri

### Ana Sayfa
![Ana Sayfa Placeholder]
Modern ve kullanıcı dostu arayüz, Bootstrap 5 ile tasarlanmıştır.

### Admin Dashboard
![Dashboard Placeholder]
Gerçek zamanlı istatistikler, bugünkü randevular ve son aktiviteler.

### Randevu Oluşturma
![Randevu Formu Placeholder]
Dinamik saat seçimi, müsaitlik kontrolü ile kullanıcı dostu form.

### AI Fitness Önerileri
![AI Önerileri Placeholder]
Google Gemini API ile kişiselleştirilmiş egzersiz programları.

### Antrenör Yönetimi
![Antrenör Formu Placeholder]
Uzmanlık seçimi ve haftalık müsaitlik takvimi.

### Swagger API
![Swagger UI Placeholder]
Otomatik API dokümantasyonu ve test arayüzü.

---

## 🚀 Özellikler ve Gereksinim Karşılama

| Gereksinim | Durum | Uygulama |
|------------|:-----:|----------|
| Kullanıcı Yönetimi | ✅ | ASP.NET Identity, kayıt/giriş/çıkış |
| Veritabanı (min 5 tablo) | ✅ | 7 tablo (ApplicationUser, Trainer, Service, Appointment, TrainerSpecialty, TrainerAvailability, AiRecommendation) |
| CRUD İşlemleri | ✅ | Hizmet, Antrenör, Randevu yönetimi |
| REST API | ✅ | 4 endpoint, LINQ filtreleme/gruplama |
| AI Entegrasyonu | ✅ | Google Gemini API, kişiselleştirilmiş fitness önerileri |
| UI/UX | ✅ | Bootstrap 5, responsive tasarım |
| Rol Bazlı Yetkilendirme | ✅ | Admin/User rolleri, [Authorize] attribute |

### Bonus Özellikler
- ✅ Swagger API dokümantasyonu
- ✅ Antrenör müsaitlik takvim sistemi
- ✅ Telefon otomatik format (+ 90 xxx-xxx-xx-xx)
- ✅ Admin dashboard istatistikleri
- ✅ Soft delete (IsActive)
- ✅ Async/await programlama

---

## 🔒 Güvenlik

- **Authentication**: ASP.NET Identity, cookie-based
- **Authorization**: Role-based access control (Admin/User)
- **SQL Injection**: Entity Framework parametreli sorgular
- **XSS**: Razor otomatik HTML encoding
- **CSRF**: AntiForgeryToken validation
- **Password**: PBKDF2 hashing
- **API Key**: appsettings.json (gitignore'da)

---

## 🧪 Test

### Manuel Test Senaryoları

#### Senaryo 1: Kullanıcı Kaydı
1. /Account/Register → Form doldur
2. ✅ "Kayıt başarılı" mesajı, otomatik giriş

#### Senaryo 2: Randevu Oluşturma
1. Giriş yap → "Randevu Al"
2. Service: Yoga, Trainer: Ahmet, Tarih: Yarın, Saat: 10:00
3. ✅ Randevu "Pending" statüsünde oluşturulur

#### Senaryo 3: Admin Randevu Onaylama
1. Admin girişi → Randevuları Yönet
2. "Onayla" butonu → ✅ Status "Approved"

#### Senaryo 4: AI Önerisi
1. AI Önerisi → Form doldur (Age: 25, Weight: 80, Goal: Kilo Verme)
2. ✅ AI önerileri gösterilir, veritabanına kaydedilir

#### Senaryo 5: API Testi
1. /swagger → GET /api/reports/available-trainers
2. "Try it out" → Parametreleri doldur → Execute
3. ✅ JSON response, HTTP 200

---

## 📁 Proje Yapısı

```
GymManagementSystem/
├── Controllers/
│   ├── AccountController.cs       # Kullanıcı kayıt/giriş
│   ├── AdminController.cs         # Admin panel
│   ├── AppointmentsController.cs  # Randevu yönetimi
│   ├── AiRecommendationController.cs  # AI önerileri
│   ├── HomeController.cs
│   └── Api/
│       └── ReportsController.cs   # REST API
├── Models/
│   ├── Entities/                  # 7 entity
│   ├── ViewModels/                # Form modelleri
│   └── Enums/                     # FitnessEnums
├── Views/
│   ├── Account/                   # Giriş/Kayıt
│   ├── Admin/                     # Admin panel
│   ├── Appointments/              # Randevular
│   ├── AiRecommendation/          # AI önerileri
│   └── Shared/                    # Layout
├── Services/
│   ├── GeminiAiService.cs         # AI servisi
│   └── AppointmentService.cs      # İş mantığı
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core context
│   └── DbInitializer.cs           # Seed data
├── Migrations/                    # EF migrations
├── wwwroot/                       # Static files
│   ├── css/
│   ├── js/
│   │   └── phone-mask.js          # Telefon formatı
│   └── lib/                       # Bootstrap, jQuery
├── Properties/
│   └── launchSettings.json
├── appsettings.json               # Yapılandırma
├── Program.cs                     # Uygulama başlangıcı
└── README.md
```

---

## 🤝 Katkıda Bulunma

Katkılarınızı bekliyoruz! Lütfen şu adımları izleyin:

1. **Fork** edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Push edin (`git push origin feature/AmazingFeature`)
5. **Pull Request** açın

---

## 📝 Geliştirme Planları

### Kısa Vadeli
- [ ] Email bildirimleri (randevu onayı)
- [ ] SMS bildirimleri (Twilio)
- [ ] Antrenör dashboard
- [ ] Çoklu gym lokasyonu

### Orta Vadeli
- [ ] Ödeme entegrasyonu (Stripe/Iyzico)
- [ ] Raporlama (Excel/PDF export)
- [ ] Üyelik paketleri
- [ ] Mobil uygulama (React Native)

### Uzun Vadeli
- [ ] AI ile otomatik antrenör eşleştirme
- [ ] Video konferans (online PT)
- [ ] Fitness tracker entegrasyonu
- [ ] Gamification (rozet, puan sistemi)

---

## 📄 Lisans

Bu proje [MIT Lisansı](LICENSE) altında lisanslanmıştır.

---

## 👨‍💻 Geliştirici

**[Ramazan Sefa Kurtuluş]**
- GitHub: [@sefakurtulus](https://github.com/sefakurtulus)
- Email: ramazansefakurtulus2001@gmail.com

---

## 🙏 Teşekkürler

- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
- [Bootstrap](https://getbootstrap.com/)
- [Google Gemini AI](https://ai.google.dev/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)

---

<div align="center">

**⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!**

Made with ❤️ and ☕

</div>
