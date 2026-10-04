using System.ComponentModel.DataAnnotations;

namespace dmitry_krasnov_kt_42_23.Filters.PerformanceFilters
{
    // Фильтр для получения среднего балла по предмету в группе
    public class GroupDisciplineAverageFilter
    {
        [Range(1, int.MaxValue)]
        public int GroupId { get; set; }

        [Range(1, int.MaxValue)]
        public int DisciplineId { get; set; }
    }
}
