using System.ComponentModel.DataAnnotations;

namespace dmitry_krasnov_kt_42_23.Requests.PerformanceRequests
{
    // Данные для изменения оценки (например, студент пересдал экзамен)
    public class UpdateGradeRequest
    {
        [Range(1, int.MaxValue)]
        public int GradeId { get; set; }

        [Range(2, 5)]
        public int Value { get; set; }

        // Если не передать - дата останется прежней
        public DateTime? Date { get; set; }
    }
}
