namespace GymManagementSystem.Models.Enums
{
    public enum Gender
    {
        Erkek = 0,
        Kadın = 1
    }

    public enum BodyType
    {
        Ektomorf = 0,      // Zayıf yapılı
        Mezomorf = 1,      // Atletik yapılı
        Endomorf = 2       // Geniş yapılı
    }

    public enum ActivityLevel
    {
        Hareketsiz = 0,       // Masa başı işi
        AzHareketli = 1,      // Haftada 1-2 gün
        OrtaSeviye = 2,       // Haftada 3-4 gün
        CokHareketli = 3      // Haftada 5+ gün
    }

    public enum FitnessGoal
    {
        KiloVermek = 0,
        KiloAlmak = 1,        // Bulk
        FormKorumak = 2,
        KondisyonArtirmak = 3
    }

    public enum WorkoutPlace
    {
        SporSalonu = 0,
        EvVucutAgirligi = 1,
        EvDambilSeti = 2
    }
}
