namespace dmitry_krasnov_kt_42_23.Responses.PerformanceResponses
{
    // Ответ: средний балл по предмету в группе
    public class GroupDisciplineAverageResponse
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; } = string.Empty;

        // null, если в группе по этому предмету еще нет оценок
        public double? AverageGrade { get; set; }

        // Сколько оценок участвовало в подсчете
        public int GradesCount { get; set; }
    }
}
