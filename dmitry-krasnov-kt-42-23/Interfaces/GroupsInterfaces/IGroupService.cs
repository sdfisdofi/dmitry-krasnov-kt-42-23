using dmitry_krasnov_kt_42_23.Database;
using dmitry_krasnov_kt_42_23.Filters.GroupFilters;
using dmitry_krasnov_kt_42_23.Models;
using Microsoft.EntityFrameworkCore;

namespace dmitry_krasnov_kt_42_23.Interfaces.GroupsInterfaces
{
    // Интерфейс (абстракция): описывает, ЧТО умеет сервис групп
    public interface IGroupService
    {
        public Task<Group[]> GetGroupsAsync(GroupFilter filter, CancellationToken cancellationToken);
    }

    // Реализация интерфейса: описывает, КАК это делается
    public class GroupService : IGroupService
    {
        private readonly StudentDbContext _dbContext;

        public GroupService(StudentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Group[]> GetGroupsAsync(GroupFilter filter, CancellationToken cancellationToken = default)
        {
            // Начинаем с запроса ко всей таблице групп.
            // Запрос еще НЕ выполняется - мы только постепенно добавляем к нему условия
            var query = _dbContext.Set<Group>().AsQueryable();

            // Фильтр по специальности (если передан)
            var specialty = filter.Specialty;
            if (!string.IsNullOrWhiteSpace(specialty))
            {
                query = query.Where(g => g.Specialty.Contains(specialty));
            }

            // Фильтр по году (если передан)
            if (filter.Year.HasValue)
            {
                var year = filter.Year.Value;
                query = query.Where(g => g.Year == year);
            }

            // Фильтр по статусу удаления (если передан)
            if (filter.IsDeleted.HasValue)
            {
                var isDeleted = filter.IsDeleted.Value;
                query = query.Where(g => g.IsDeleted == isDeleted);
            }

            // Только здесь запрос уходит в БД (ToArrayAsync)
            return await query.ToArrayAsync(cancellationToken);
        }
    }
}
