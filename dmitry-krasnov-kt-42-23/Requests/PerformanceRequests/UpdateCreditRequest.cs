using System.ComponentModel.DataAnnotations;

namespace dmitry_krasnov_kt_42_23.Requests.PerformanceRequests
{
    // Данные для изменения зачета
    public class UpdateCreditRequest
    {
        [Range(1, int.MaxValue)]
        public int CreditId { get; set; }

        public bool IsPassed { get; set; }

        // Если не передать - дата останется прежней
        public DateTime? Date { get; set; }
    }
}
