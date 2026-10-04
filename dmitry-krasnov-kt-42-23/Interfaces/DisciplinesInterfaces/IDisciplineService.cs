using dmitry_krasnov_kt_42_23.Database;
using dmitry_krasnov_kt_42_23.Filters.DisciplineFilters;
using dmitry_krasnov_kt_42_23.Models;
using dmitry_krasnov_kt_42_23.Requests.DisciplineRequests;
using Microsoft.EntityFrameworkCore;

namespace dmitry_krasnov_kt_42_23.Interfaces.DisciplinesInterfaces
{
    // Интерфейс (абстракция): описывает, ЧТО умеет сервис дисциплин
    public interface IDisciplineService
    {
        public Task<Discipline[]> GetDisciplinesAsync(DisciplineFilter filter, CancellationToken cancellationToken);

        public Task<Discipline> AddDisciplineAsync(AddDisciplineRequest request, CancellationToken cancellationToken);

        // Возвращает null, если дисциплина с таким Id не найдена
        public Task<Discipline?> UpdateDisciplineAsync(UpdateDisciplineRequest request, CancellationToken cancellationToken);

        // Возвращает false, если дисциплина с таким Id не найдена
        public Task<bool> DeleteDisciplineAsync(int disciplineId, CancellationToken cancellationToken);
    }

    // Реализация интерфейса: описывает, КАК это делается
    public class DisciplineService : IDisciplineService
    {
        private readonly StudentDbContext _dbContext;

        public DisciplineService(StudentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Discipline[]> GetDisciplinesAsync(DisciplineFilter filter, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Set<Discipline>().AsQueryable();

            // Фильтр по названию
            var disciplineName = filter.DisciplineName;
            if (!string.IsNullOrWhiteSpace(disciplineName))
            {
                query = query.Where(d => d.DisciplineName.Contains(disciplineName));
            }

            // Фильтр по направлению (гуманитарное / техническое)
            if (filter.Direction.HasValue)
            {
                var direction = filter.Direction.Value;
                query = query.Where(d => d.Direction == direction);
            }

            // Фильтр по статусу удаления
            if (filter.IsDeleted.HasValue)
            {
                var isDeleted = filter.IsDeleted.Value;
                query = query.Where(d => d.IsDeleted == isDeleted);
            }

            return await query.ToArrayAsync(cancellationToken);
        }

        public async Task<Discipline> AddDisciplineAsync(AddDisciplineRequest request, CancellationToken cancellationToken = default)
        {
            var discipline = new Discipline
            {
                DisciplineName = request.DisciplineName,
                Direction = request.Direction,
                IsDeleted = false
            };

            await _dbContext.Set<Discipline>().AddAsync(discipline, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return discipline;
        }

        public async Task<Discipline?> UpdateDisciplineAsync(UpdateDisciplineRequest request, CancellationToken cancellationToken = default)
        {
            var discipline = await _dbContext.Set<Discipline>()
                .FirstOrDefaultAsync(d => d.DisciplineId == request.DisciplineId, cancellationToken);

            if (discipline == null)
            {
                return null;
            }

            discipline.DisciplineName = request.DisciplineName;
            discipline.Direction = request.Direction;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return discipline;
        }

        public async Task<bool> DeleteDisciplineAsync(int disciplineId, CancellationToken cancellationToken = default)
        {
            var discipline = await _dbContext.Set<Discipline>()
                .FirstOrDefaultAsync(d => d.DisciplineId == disciplineId, cancellationToken);

            if (discipline == null)
            {
                return false;
            }

            // Мягкое удаление. Оценки и зачеты по дисциплине остаются в БД
            discipline.IsDeleted = true;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
