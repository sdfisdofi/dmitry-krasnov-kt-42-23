namespace dmitry_krasnov_kt_42_23.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;

        // Специальность группы (внешний ключ на таблицу специальностей)
        public int SpecialityId { get; set; }
        public Speciality? Speciality { get; set; }

        // Год набора группы (например 2023)
        public int Year { get; set; }

        // Мягкое удаление: true - группа удалена
        public bool IsDeleted { get; set; }
    }
}
