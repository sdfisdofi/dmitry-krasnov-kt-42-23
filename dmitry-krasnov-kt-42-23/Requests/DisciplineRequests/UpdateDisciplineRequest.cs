using dmitry_krasnov_kt_42_23.Models;
using System.ComponentModel.DataAnnotations;

namespace dmitry_krasnov_kt_42_23.Requests.DisciplineRequests
{
    // Данные для изменения дисциплины
    public class UpdateDisciplineRequest
    {
        [Range(1, int.MaxValue)]
        public int DisciplineId { get; set; }

        [Required]
        [MaxLength(200)]
        public string DisciplineName { get; set; } = string.Empty;

        [EnumDataType(typeof(DisciplineDirection))]
        public DisciplineDirection Direction { get; set; }
    }
}
