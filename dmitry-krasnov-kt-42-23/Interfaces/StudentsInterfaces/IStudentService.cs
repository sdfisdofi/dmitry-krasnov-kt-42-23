using dmitry_krasnov_kt_42_23.Database;
using dmitry_krasnov_kt_42_23.Filters.StudentFilters;
using dmitry_krasnov_kt_42_23.Models;
using dmitry_krasnov_kt_42_23.Requests.StudentRequests;
using Microsoft.EntityFrameworkCore;

namespace dmitry_krasnov_kt_42_23.Interfaces.StudentsInterfaces
{
    // Интерфейс (абстракция): описывает, ЧТО умеет сервис студентов
    public interface IStudentService
    {
        public Task<Student[]> GetStudentsAsync(StudentFilter filter, CancellationToken cancellationToken);

        // Возвращают пару: студент (если всё успешно) или текст ошибки (если что-то не так)
        public Task<(Student? Student, string? Error)> AddStudentAsync(AddStudentRequest request, CancellationToken cancellationToken);

        public Task<(Student? Student, string? Error)> UpdateStudentAsync(UpdateStudentRequest request, CancellationToken cancellationToken);

        // Возвращает false, если студент с таким Id не найден
        public Task<bool> DeleteStudentAsync(int studentId, CancellationToken cancellationToken);
    }

    // Реализация интерфейса: описывает, КАК это делается
    public class StudentService : IStudentService
    {
        private readonly StudentDbContext _dbContext;

        public StudentService(StudentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Student[]> GetStudentsAsync(StudentFilter filter, CancellationToken cancellationToken = default)
        {
            // Группа подгрузится автоматически (AutoInclude в StudentConfiguration)
            var query = _dbContext.Set<Student>().AsQueryable();

            // Фильтр по Id группы
            if (filter.GroupId.HasValue)
            {
                var groupId = filter.GroupId.Value;
                query = query.Where(s => s.GroupId == groupId);
            }

            // Фильтр по названию группы (через навигационное свойство Group)
            var groupName = filter.GroupName;
            if (!string.IsNullOrWhiteSpace(groupName))
            {
                query = query.Where(s => s.Group!.GroupName == groupName);
            }

            // Фильтры по ФИО (поиск по части строки)
            var lastName = filter.LastName;
            if (!string.IsNullOrWhiteSpace(lastName))
            {
                query = query.Where(s => s.LastName.Contains(lastName));
            }

            var firstName = filter.FirstName;
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                query = query.Where(s => s.FirstName.Contains(firstName));
            }

            var middleName = filter.MiddleName;
            if (!string.IsNullOrWhiteSpace(middleName))
            {
                query = query.Where(s => s.MiddleName != null && s.MiddleName.Contains(middleName));
            }

            // Фильтр по статусу удаления
            if (filter.IsDeleted.HasValue)
            {
                var isDeleted = filter.IsDeleted.Value;
                query = query.Where(s => s.IsDeleted == isDeleted);
            }

            return await query.ToArrayAsync(cancellationToken);
        }

        public async Task<(Student? Student, string? Error)> AddStudentAsync(AddStudentRequest request, CancellationToken cancellationToken = default)
        {
            // Нельзя добавить студента в несуществующую или удаленную группу
            var groupError = await CheckGroupAsync(request.GroupId, cancellationToken);
            if (groupError != null)
            {
                return (null, groupError);
            }

            var student = new Student
            {
                LastName = request.LastName,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                GroupId = request.GroupId,
                IsDeleted = false
            };

            await _dbContext.Set<Student>().AddAsync(student, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Подгружаем группу, чтобы она отобразилась в ответе
            await _dbContext.Entry(student).Reference(s => s.Group).LoadAsync(cancellationToken);

            return (student, null);
        }

        public async Task<(Student? Student, string? Error)> UpdateStudentAsync(UpdateStudentRequest request, CancellationToken cancellationToken = default)
        {
            var student = await _dbContext.Set<Student>()
                .FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);

            if (student == null)
            {
                return (null, $"Студент с Id = {request.StudentId} не найден");
            }

            var groupError = await CheckGroupAsync(request.GroupId, cancellationToken);
            if (groupError != null)
            {
                return (null, groupError);
            }

            student.LastName = request.LastName;
            student.FirstName = request.FirstName;
            student.MiddleName = request.MiddleName;
            student.GroupId = request.GroupId;

            await _dbContext.SaveChangesAsync(cancellationToken);

            // Если группа поменялась - подгружаем новую для ответа
            await _dbContext.Entry(student).Reference(s => s.Group).LoadAsync(cancellationToken);

            return (student, null);
        }

        public async Task<bool> DeleteStudentAsync(int studentId, CancellationToken cancellationToken = default)
        {
            var student = await _dbContext.Set<Student>()
                .FirstOrDefaultAsync(s => s.StudentId == studentId, cancellationToken);

            if (student == null)
            {
                return false;
            }

            // Мягкое удаление
            student.IsDeleted = true;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        // Вспомогательный метод: проверяет, что группа существует и не удалена.
        // Возвращает текст ошибки или null, если всё в порядке
        private async Task<string?> CheckGroupAsync(int groupId, CancellationToken cancellationToken)
        {
            var group = await _dbContext.Set<Group>()
                .FirstOrDefaultAsync(g => g.GroupId == groupId, cancellationToken);

            if (group == null)
            {
                return $"Группа с Id = {groupId} не найдена";
            }

            if (group.IsDeleted)
            {
                return $"Группа с Id = {groupId} удалена, добавить в нее студента нельзя";
            }

            return null;
        }
    }
}
