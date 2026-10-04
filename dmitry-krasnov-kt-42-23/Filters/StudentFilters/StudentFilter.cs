namespace dmitry_krasnov_kt_42_23.Filters.StudentFilters
{
    // Фильтр для получения списка студентов.
    // Все поля необязательные: если поле не заполнено (null), фильтрация по нему не выполняется
    public class StudentFilter
    {
        // Фильтр по группе: можно указать Id группы или ее название
        public int? GroupId { get; set; }
        public string? GroupName { get; set; }

        // Фильтр по ФИО (поиск по части фамилии / имени / отчества)
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }

        // Статус удаления: true - только удаленные, false - только неудаленные, null - все
        public bool? IsDeleted { get; set; }
    }
}
