using System.ComponentModel.DataAnnotations;

namespace dmitry_krasnov_kt_42_23.Requests.PerformanceRequests
{
    // Данные для выставления оценки студенту
    public class AddGradeRequest
    {
        [Range(1, int.MaxValue)]
        public int StudentId { get; set; }

        [Range(1, int.MaxValue)]
        public int DisciplineId { get; set; }

        // Оценка только от 2 до 5
        [Range(2, 5)]
        public int Value { get; set; }

        // Дата необязательная: если не передать, будет текущая
        public DateTime? Date { get; set; }
    }
}
