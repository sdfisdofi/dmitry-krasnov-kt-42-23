namespace dmitry_krasnov_kt_42_23.Models
{
    // Оценка (экзамен): значение от 2 до 5
    public class Grade
    {
        public int GradeId { get; set; }
        public int Value { get; set; }

        // Дата выставления - нужна для "среднего балла по году"
        public DateTime Date { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int DisciplineId { get; set; }
        public Discipline? Discipline { get; set; }

        // Положительная ли оценка (3, 4, 5 - да; 2 - нет)
        public bool IsPositive()
        {
            return Value >= 3;
        }
    }
}
