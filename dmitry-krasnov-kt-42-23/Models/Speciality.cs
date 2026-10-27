namespace dmitry_krasnov_kt_42_23.Models
{
    // Специальность (например "09.03.01 Информатика и вычислительная техника")
    public class Speciality
    {
        public int SpecialityId { get; set; }

        // Название специальности
        public string Title { get; set; } = string.Empty;

        // Код специальности (например "09.03.01"), может быть не заполнен
        public string? Code { get; set; }

        // Мягкое удаление: true - специальность удалена
        public bool IsDeleted { get; set; }
    }
}