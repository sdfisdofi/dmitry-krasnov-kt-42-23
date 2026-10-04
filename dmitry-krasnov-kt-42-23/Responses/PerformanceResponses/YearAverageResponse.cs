namespace dmitry_krasnov_kt_42_23.Responses.PerformanceResponses
{
    // Ответ: средний балл по году
    public class YearAverageResponse
    {
        public int Year { get; set; }

        // Заполнены, если в фильтре была указана группа / студент
        public int? GroupId { get; set; }
        public int? StudentId { get; set; }

        // null, если за этот год оценок нет
        public double? AverageGrade { get; set; }
        public int GradesCount { get; set; }
    }
}
