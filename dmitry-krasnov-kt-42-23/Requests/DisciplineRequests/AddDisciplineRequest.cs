using dmitry_krasnov_kt_42_23.Models;
using System.ComponentModel.DataAnnotations;

namespace dmitry_krasnov_kt_42_23.Requests.DisciplineRequests
{
    // Данные для добавления новой дисциплины
    public class AddDisciplineRequest
    {
        [Required]
        [MaxLength(200)]
        public string DisciplineName { get; set; } = string.Empty;

        // [EnumDataType] не пропустит значение, которого нет в enum (например 7)
        [EnumDataType(typeof(DisciplineDirection))]
        public DisciplineDirection Direction { get; set; }
    }
}
