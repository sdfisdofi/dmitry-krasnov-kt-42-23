using System.ComponentModel.DataAnnotations;

namespace dmitry_krasnov_kt_42_23.Filters.PerformanceFilters
{
    // Фильтр для получения успеваемости конкретного студента
    public class StudentPerformanceFilter
    {
        // Обязательно: чью успеваемость смотрим
        [Range(1, int.MaxValue)]
        public int StudentId { get; set; }

        // Необязательно: только по одной дисциплине
        public int? DisciplineId { get; set; }
    }
}
