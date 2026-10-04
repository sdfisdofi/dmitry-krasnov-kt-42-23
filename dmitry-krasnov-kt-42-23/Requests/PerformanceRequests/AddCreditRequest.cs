using System.ComponentModel.DataAnnotations;

namespace dmitry_krasnov_kt_42_23.Requests.PerformanceRequests
{
    // Данные для выставления зачета студенту
    public class AddCreditRequest
    {
        [Range(1, int.MaxValue)]
        public int StudentId { get; set; }

        [Range(1, int.MaxValue)]
        public int DisciplineId { get; set; }

        // true - зачтено, false - не зачтено
        public bool IsPassed { get; set; }

        // Дата необязательная: если не передать, будет текущая
        public DateTime? Date { get; set; }
    }
}
