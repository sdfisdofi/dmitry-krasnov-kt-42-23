using dmitry_krasnov_kt_42_23.Models;

namespace dmitry_krasnov_kt_42_23.Filters.DisciplineFilters
{
    // Фильтр для получения списка дисциплин.
    // Все поля необязательные: если поле не заполнено (null), фильтрация по нему не выполняется
    public class DisciplineFilter
    {
        // Поиск по части названия
        public string? DisciplineName { get; set; }

        // Направление: Humanitarian (гуманитарное) или Technical (техническое)
        public DisciplineDirection? Direction { get; set; }

        // Статус удаления: true - только удаленные, false - только неудаленные, null - все
        public bool? IsDeleted { get; set; }
    }
}
