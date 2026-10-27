using dmitry_krasnov_kt_42_23.Database;
using dmitry_krasnov_kt_42_23.Filters.SpecialityFilters;
using dmitry_krasnov_kt_42_23.Models;
using dmitry_krasnov_kt_42_23.Requests.SpecialityRequests;
using Microsoft.EntityFrameworkCore;

namespace dmitry_krasnov_kt_42_23.Interfaces.SpecialitiesInterfaces
{
    // Интерфейс (абстракция): описывает, ЧТО умеет сервис специальностей
    public interface ISpecialityService
    {
        public Task<Speciality[]> GetSpecialitiesAsync(SpecialityFilter filter, CancellationToken cancellationToken);

        public Task<Speciality> AddSpecialityAsync(AddSpecialityRequest request, CancellationToken cancellationToken);

        // Возвращает null, если специальность с таким Id не найдена
        public Task<Speciality?> UpdateSpecialityAsync(UpdateSpecialityRequest request, CancellationToken cancellationToken);

        // Возвращает false, если специальность с таким Id не найдена
        public Task<bool> DeleteSpecialityAsync(int specialityId, CancellationToken cancellationToken);
    }

    // Реализация интерфейса: описывает, КАК это делается
    public class SpecialityService : ISpecialityService
    {
        private readonly StudentDbContext _dbContext;

        public SpecialityService(StudentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Speciality[]> GetSpecialitiesAsync(SpecialityFilter filter, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Set<Speciality>().AsQueryable();

            // Фильтр по названию
            var title = filter.Title;
            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(s => s.Title.Contains(title));
            }

            // Фильтр по коду
            var code = filter.Code;
            if (!string.IsNullOrWhiteSpace(code))
            {
                query = query.Where(s => s.Code != null && s.Code.Contains(code));
            }

            // Фильтр по статусу удаления
            if (filter.IsDeleted.HasValue)
            {
                var isDeleted = filter.IsDeleted.Value;
                query = query.Where(s => s.IsDeleted == isDeleted);
            }

            return await query.ToArrayAsync(cancellationToken);
        }

        public async Task<Speciality> AddSpecialityAsync(AddSpecialityRequest request, CancellationToken cancellationToken = default)
        {
            var speciality = new Speciality
            {
                Title = request.Title,
                Code = request.Code,
                IsDeleted = false
            };

            await _dbContext.Set<Speciality>().AddAsync(speciality, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return speciality;
        }

        public async Task<Speciality?> UpdateSpecialityAsync(UpdateSpecialityRequest request, CancellationToken cancellationToken = default)
        {
            var speciality = await _dbContext.Set<Speciality>()
                .FirstOrDefaultAsync(s => s.SpecialityId == request.SpecialityId, cancellationToken);

            if (speciality == null)
            {
                return null;
            }

            speciality.Title = request.Title;
            speciality.Code = request.Code;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return speciality;
        }

        public async Task<bool> DeleteSpecialityAsync(int specialityId, CancellationToken cancellationToken = default)
        {
            var speciality = await _dbContext.Set<Speciality>()
                .FirstOrDefaultAsync(s => s.SpecialityId == specialityId, cancellationToken);

            if (speciality == null)
            {
                return false;
            }

            // Мягкое удаление. Группы этой специальности остаются в БД
            speciality.IsDeleted = true;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}