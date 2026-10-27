namespace dmitry_krasnov_kt_42_23.Filters.GroupFilters
{
    // Фильтр для получения списка групп.
    // Все поля необязательные: если поле не заполнено (null), фильтрация по нему не выполняется
    public class GroupFilter
    {
        // Фильтр по специальности: можно указать Id специальности или часть ее названия
        public int? SpecialityId { get; set; }
        public string? SpecialityTitle { get; set; }

        // Год набора группы
        public int? Year { get; set; }

        // Статус удаления: true - только удаленные, false - только неудаленные, null - все
        public bool? IsDeleted { get; set; }
    }
}
