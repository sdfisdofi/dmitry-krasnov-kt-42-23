namespace dmitry_krasnov_kt_42_23.Responses.PerformanceResponses
{
    // Ответ: успеваемость конкретного студента.
    // Это не таблица БД, а объект, который мы собираем специально для ответа
    public class StudentPerformanceResponse
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;

        // Средний балл по выбранным оценкам (null, если оценок нет)
        public double? AverageGrade { get; set; }

        public GradeItem[] Grades { get; set; } = [];
        public CreditItem[] Credits { get; set; } = [];
    }

    // Одна оценка в ответе
    public class GradeItem
    {
        public int GradeId { get; set; }
        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; } = string.Empty;
        public int Value { get; set; }
        public DateTime Date { get; set; }
    }

    // Один зачет в ответе
    public class CreditItem
    {
        public int CreditId { get; set; }
        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; } = string.Empty;
        public bool IsPassed { get; set; }
        public DateTime Date { get; set; }
    }
}
