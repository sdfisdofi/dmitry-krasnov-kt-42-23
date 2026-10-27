using dmitry_krasnov_kt_42_23.Database;
using dmitry_krasnov_kt_42_23.Filters.GroupFilters;
using dmitry_krasnov_kt_42_23.Models;
using dmitry_krasnov_kt_42_23.Requests.GroupRequests;
using Microsoft.EntityFrameworkCore;

namespace dmitry_krasnov_kt_42_23.Interfaces.GroupsInterfaces
{
    // Интерфейс (абстракция): описывает, ЧТО умеет сервис групп
    public interface IGroupService
    {
        public Task<Group[]> GetGroupsAsync(GroupFilter filter, CancellationToken cancellationToken);

        // Возвращают пару: группа (если всё успешно) или текст ошибки (если что-то не так)
        public Task<(Group? Group, string? Error)> AddGroupAsync(AddGroupRequest request, CancellationToken cancellationToken);

        public Task<(Group? Group, string? Error)> UpdateGroupAsync(UpdateGroupRequest request, CancellationToken cancellationToken);

        // Возвращает false, если группа с таким Id не найдена
        public Task<bool> DeleteGroupAsync(int groupId, CancellationToken cancellationToken);
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

            // Фильтр по Id специальности (если передан)
            if (filter.SpecialityId.HasValue)
            {
                var specialityId = filter.SpecialityId.Value;
                query = query.Where(g => g.SpecialityId == specialityId);
            }

            // Фильтр по названию специальности (через навигационное свойство Speciality)
            var specialityTitle = filter.SpecialityTitle;
            if (!string.IsNullOrWhiteSpace(specialityTitle))
            {
                query = query.Where(g => g.Speciality!.Title.Contains(specialityTitle));
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

        public async Task<(Group? Group, string? Error)> AddGroupAsync(AddGroupRequest request, CancellationToken cancellationToken = default)
        {
            // Нельзя создать группу с несуществующей или удаленной специальностью
            var specialityError = await CheckSpecialityAsync(request.SpecialityId, cancellationToken);
            if (specialityError != null)
            {
                return (null, specialityError);
            }

            var group = new Group
            {
                GroupName = request.GroupName,
                SpecialityId = request.SpecialityId,
                Year = request.Year,
                IsDeleted = false
            };

            await _dbContext.Set<Group>().AddAsync(group, cancellationToken);

            // После SaveChanges база сама присвоит группе GroupId
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Подгружаем специальность, чтобы она отобразилась в ответе
            await _dbContext.Entry(group).Reference(g => g.Speciality).LoadAsync(cancellationToken);

            return (group, null);
        }

        public async Task<(Group? Group, string? Error)> UpdateGroupAsync(UpdateGroupRequest request, CancellationToken cancellationToken = default)
        {
            var group = await _dbContext.Set<Group>()
                .FirstOrDefaultAsync(g => g.GroupId == request.GroupId, cancellationToken);

            if (group == null)
            {
                return (null, $"Группа с Id = {request.GroupId} не найдена");
            }

            var specialityError = await CheckSpecialityAsync(request.SpecialityId, cancellationToken);
            if (specialityError != null)
            {
                return (null, specialityError);
            }

            // EF Core отслеживает загруженный объект,
            // поэтому достаточно поменять свойства и вызвать SaveChanges
            group.GroupName = request.GroupName;
            group.SpecialityId = request.SpecialityId;
            group.Year = request.Year;

            await _dbContext.SaveChangesAsync(cancellationToken);

            // Если специальность поменялась - подгружаем новую для ответа
            await _dbContext.Entry(group).Reference(g => g.Speciality).LoadAsync(cancellationToken);

            return (group, null);
        }

        public async Task<bool> DeleteGroupAsync(int groupId, CancellationToken cancellationToken = default)
        {
            var group = await _dbContext.Set<Group>()
                .FirstOrDefaultAsync(g => g.GroupId == groupId, cancellationToken);

            if (group == null)
            {
                return false;
            }

            // Мягкое удаление: запись остается в БД, но помечается как удаленная
            group.IsDeleted = true;

            // По условию варианта при удалении группы удаляются и ее студенты
            var students = await _dbContext.Set<Student>()
                .Where(s => s.GroupId == groupId)
                .ToListAsync(cancellationToken);

            foreach (var student in students)
            {
                student.IsDeleted = true;
            }

            // Один SaveChanges = одна транзакция:
            // либо помечаются и группа, и все студенты, либо ничего
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        // Вспомогательный метод: проверяет, что специальность существует и не удалена.
        // Возвращает текст ошибки или null, если всё в порядке
        private async Task<string?> CheckSpecialityAsync(int specialityId, CancellationToken cancellationToken)
        {
            var speciality = await _dbContext.Set<Speciality>()
                .FirstOrDefaultAsync(s => s.SpecialityId == specialityId, cancellationToken);

            if (speciality == null)
            {
                return $"Специальность с Id = {specialityId} не найдена";
            }

            if (speciality.IsDeleted)
            {
                return $"Специальность с Id = {specialityId} удалена";
            }

            return null;
        }
    }
}
