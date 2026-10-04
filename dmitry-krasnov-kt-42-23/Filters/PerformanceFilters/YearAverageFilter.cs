using System.ComponentModel.DataAnnotations;

namespace dmitry_krasnov_kt_42_23.Filters.PerformanceFilters
{
    // Фильтр для получения среднего балла по году
    public class YearAverageFilter
    {
        // Обязательно: за какой год считаем (по дате выставления оценки)
        [Range(1990, 2100)]
        public int Year { get; set; }

        // Необязательно: только по одной группе
        public int? GroupId { get; set; }

        // Необязательно: только по одному студенту
        public int? StudentId { get; set; }
    }
}
