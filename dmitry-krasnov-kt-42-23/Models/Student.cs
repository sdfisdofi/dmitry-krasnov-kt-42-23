namespace dmitry_krasnov_kt_42_23.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Отчества может не быть, поэтому string? (необязательное поле)
        public string? MiddleName { get; set; }

        public int GroupId { get; set; }
        public Group? Group { get; set; }

        // Мягкое удаление: true - студент удален
        public bool IsDeleted { get; set; }
    }
}
