namespace dmitry_krasnov_kt_42_23.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;

        // Специальность (например "Информатика и вычислительная техника")
        public string Specialty { get; set; } = string.Empty;

        // Год набора группы (например 2023)
        public int Year { get; set; }

        // Мягкое удаление: true - группа удалена
        public bool IsDeleted { get; set; }
    }
}
