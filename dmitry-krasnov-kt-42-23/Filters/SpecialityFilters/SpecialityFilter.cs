namespace dmitry_krasnov_kt_42_23.Filters.SpecialityFilters
{
    // Фильтр для получения списка специальностей.
    // Все поля необязательные: если поле не заполнено (null), фильтрация по нему не выполняется
    public class SpecialityFilter
    {
        // Поиск по части названия
        public string? Title { get; set; }

        // Поиск по части кода
        public string? Code { get; set; }

        // Статус удаления: true - только удаленные, false - только неудаленные, null - все
        public bool? IsDeleted { get; set; }
    }
}