namespace dmitry_krasnov_kt_42_23.Models
{
    public class Discipline
    {
        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; } = string.Empty;
        public DisciplineDirection Direction { get; set; }

        // Мягкое удаление: true - дисциплина удалена
        public bool IsDeleted { get; set; }
    }
}
