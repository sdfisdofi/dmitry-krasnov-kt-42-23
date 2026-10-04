using dmitry_krasnov_kt_42_23.Database;
using dmitry_krasnov_kt_42_23.Filters.PerformanceFilters;
using dmitry_krasnov_kt_42_23.Models;
using dmitry_krasnov_kt_42_23.Requests.PerformanceRequests;
using dmitry_krasnov_kt_42_23.Responses.PerformanceResponses;
using Microsoft.EntityFrameworkCore;

namespace dmitry_krasnov_kt_42_23.Interfaces.PerformanceInterfaces
{
    // Интерфейс (абстракция): описывает, ЧТО умеет сервис успеваемости
    public interface IPerformanceService
    {
        // Все методы возвращают пару: результат (если всё успешно) или текст ошибки

        // --- Получение успеваемости ---
        public Task<(StudentPerformanceResponse? Result, string? Error)> GetStudentPerformanceAsync(StudentPerformanceFilter filter, CancellationToken cancellationToken);

        public Task<(GroupDisciplineAverageResponse? Result, string? Error)> GetGroupDisciplineAverageAsync(GroupDisciplineAverageFilter filter, CancellationToken cancellationToken);

        public Task<(YearAverageResponse? Result, string? Error)> GetYearAverageAsync(YearAverageFilter filter, CancellationToken cancellationToken);

        // --- Добавление и изменение оценок и зачетов ---
        public Task<(Grade? Grade, string? Error)> AddGradeAsync(AddGradeRequest request, CancellationToken cancellationToken);

        public Task<(Grade? Grade, string? Error)> UpdateGradeAsync(UpdateGradeRequest request, CancellationToken cancellationToken);

        public Task<(Credit? Credit, string? Error)> AddCreditAsync(AddCreditRequest request, CancellationToken cancellationToken);

        public Task<(Credit? Credit, string? Error)> UpdateCreditAsync(UpdateCreditRequest request, CancellationToken cancellationToken);
    }

    // Реализация интерфейса: описывает, КАК это делается
    public class PerformanceService : IPerformanceService
    {
        private readonly StudentDbContext _dbContext;

        public PerformanceService(StudentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ===================== Получение успеваемости =====================

        public async Task<(StudentPerformanceResponse? Result, string? Error)> GetStudentPerformanceAsync(StudentPerformanceFilter filter, CancellationToken cancellationToken = default)
        {
            // Группа подгрузится автоматически (AutoInclude)
            var student = await _dbContext.Set<Student>()
                .FirstOrDefaultAsync(s => s.StudentId == filter.StudentId, cancellationToken);

            if (student == null)
            {
                return (null, $"Студент с Id = {filter.StudentId} не найден");
            }

            var gradesQuery = _dbContext.Set<Grade>().Where(g => g.StudentId == filter.StudentId);
            var creditsQuery = _dbContext.Set<Credit>().Where(c => c.StudentId == filter.StudentId);

            // Если указана дисциплина - оставляем только ее
            if (filter.DisciplineId.HasValue)
            {
                var disciplineId = filter.DisciplineId.Value;
                gradesQuery = gradesQuery.Where(g => g.DisciplineId == disciplineId);
                creditsQuery = creditsQuery.Where(c => c.DisciplineId == disciplineId);
            }

            // Select превращает записи БД в объекты для ответа (это называется проекция).
            // В SQL уйдут только нужные колонки
            var grades = await gradesQuery
                .OrderBy(g => g.Date)
                .Select(g => new GradeItem
                {
                    GradeId = g.GradeId,
                    DisciplineId = g.DisciplineId,
                    DisciplineName = g.Discipline!.DisciplineName,
                    Value = g.Value,
                    Date = g.Date
                })
                .ToArrayAsync(cancellationToken);

            var credits = await creditsQuery
                .OrderBy(c => c.Date)
                .Select(c => new CreditItem
                {
                    CreditId = c.CreditId,
                    DisciplineId = c.DisciplineId,
                    DisciplineName = c.Discipline!.DisciplineName,
                    IsPassed = c.IsPassed,
                    Date = c.Date
                })
                .ToArrayAsync(cancellationToken);

            var result = new StudentPerformanceResponse
            {
                StudentId = student.StudentId,
                FullName = $"{student.LastName} {student.FirstName} {student.MiddleName}".Trim(),
                GroupName = student.Group?.GroupName ?? string.Empty,
                // Оценки уже загружены в память, поэтому средний считаем здесь же
                AverageGrade = grades.Length > 0
                    ? Math.Round(grades.Average(g => g.Value), 2)
                    : null,
                Grades = grades,
                Credits = credits
            };

            return (result, null);
        }

        public async Task<(GroupDisciplineAverageResponse? Result, string? Error)> GetGroupDisciplineAverageAsync(GroupDisciplineAverageFilter filter, CancellationToken cancellationToken = default)
        {
            var group = await _dbContext.Set<Group>()
                .FirstOrDefaultAsync(g => g.GroupId == filter.GroupId, cancellationToken);

            if (group == null)
            {
                return (null, $"Группа с Id = {filter.GroupId} не найдена");
            }

            var discipline = await _dbContext.Set<Discipline>()
                .FirstOrDefaultAsync(d => d.DisciplineId == filter.DisciplineId, cancellationToken);

            if (discipline == null)
            {
                return (null, $"Дисциплина с Id = {filter.DisciplineId} не найдена");
            }

            // Оценки по дисциплине у студентов этой группы.
            // Удаленных студентов в подсчет не берем
            var query = _dbContext.Set<Grade>()
                .Where(g => g.DisciplineId == filter.DisciplineId
                         && g.Student!.GroupId == filter.GroupId
                         && !g.Student.IsDeleted);

            var gradesCount = await query.CountAsync(cancellationToken);

            // (double?) нужен, чтобы при отсутствии оценок вернулся null, а не ошибка
            var average = await query.Select(g => (double?)g.Value).AverageAsync(cancellationToken);

            var result = new GroupDisciplineAverageResponse
            {
                GroupId = group.GroupId,
                GroupName = group.GroupName,
                DisciplineId = discipline.DisciplineId,
                DisciplineName = discipline.DisciplineName,
                AverageGrade = average.HasValue ? Math.Round(average.Value, 2) : null,
                GradesCount = gradesCount
            };

            return (result, null);
        }

        public async Task<(YearAverageResponse? Result, string? Error)> GetYearAverageAsync(YearAverageFilter filter, CancellationToken cancellationToken = default)
        {
            // Оценки за указанный год. Удаленных студентов в подсчет не берем
            var query = _dbContext.Set<Grade>()
                .Where(g => g.Date.Year == filter.Year && !g.Student!.IsDeleted);

            if (filter.GroupId.HasValue)
            {
                var groupId = filter.GroupId.Value;

                var groupExists = await _dbContext.Set<Group>().AnyAsync(g => g.GroupId == groupId, cancellationToken);
                if (!groupExists)
                {
                    return (null, $"Группа с Id = {groupId} не найдена");
                }

                query = query.Where(g => g.Student!.GroupId == groupId);
            }

            if (filter.StudentId.HasValue)
            {
                var studentId = filter.StudentId.Value;

                var studentExists = await _dbContext.Set<Student>().AnyAsync(s => s.StudentId == studentId, cancellationToken);
                if (!studentExists)
                {
                    return (null, $"Студент с Id = {studentId} не найден");
                }

                query = query.Where(g => g.StudentId == studentId);
            }

            var gradesCount = await query.CountAsync(cancellationToken);
            var average = await query.Select(g => (double?)g.Value).AverageAsync(cancellationToken);

            var result = new YearAverageResponse
            {
                Year = filter.Year,
                GroupId = filter.GroupId,
                StudentId = filter.StudentId,
                AverageGrade = average.HasValue ? Math.Round(average.Value, 2) : null,
                GradesCount = gradesCount
            };

            return (result, null);
        }

        // ============ Добавление и изменение оценок и зачетов ============

        public async Task<(Grade? Grade, string? Error)> AddGradeAsync(AddGradeRequest request, CancellationToken cancellationToken = default)
        {
            // Оценку можно поставить только существующему студенту по существующей дисциплине
            var error = await CheckStudentAndDisciplineAsync(request.StudentId, request.DisciplineId, cancellationToken);
            if (error != null)
            {
                return (null, error);
            }

            var grade = new Grade
            {
                StudentId = request.StudentId,
                DisciplineId = request.DisciplineId,
                Value = request.Value,
                Date = request.Date ?? DateTime.Now
            };

            await _dbContext.Set<Grade>().AddAsync(grade, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Студент и дисциплина уже загружены в контекст при проверке,
            // поэтому EF Core сам подставит их в grade.Student и grade.Discipline
            return (grade, null);
        }

        public async Task<(Grade? Grade, string? Error)> UpdateGradeAsync(UpdateGradeRequest request, CancellationToken cancellationToken = default)
        {
            // Студент и дисциплина подгрузятся автоматически (AutoInclude в GradeConfiguration)
            var grade = await _dbContext.Set<Grade>()
                .FirstOrDefaultAsync(g => g.GradeId == request.GradeId, cancellationToken);

            if (grade == null)
            {
                return (null, $"Оценка с Id = {request.GradeId} не найдена");
            }

            grade.Value = request.Value;

            if (request.Date.HasValue)
            {
                grade.Date = request.Date.Value;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return (grade, null);
        }

        public async Task<(Credit? Credit, string? Error)> AddCreditAsync(AddCreditRequest request, CancellationToken cancellationToken = default)
        {
            var error = await CheckStudentAndDisciplineAsync(request.StudentId, request.DisciplineId, cancellationToken);
            if (error != null)
            {
                return (null, error);
            }

            var credit = new Credit
            {
                StudentId = request.StudentId,
                DisciplineId = request.DisciplineId,
                IsPassed = request.IsPassed,
                Date = request.Date ?? DateTime.Now
            };

            await _dbContext.Set<Credit>().AddAsync(credit, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return (credit, null);
        }

        public async Task<(Credit? Credit, string? Error)> UpdateCreditAsync(UpdateCreditRequest request, CancellationToken cancellationToken = default)
        {
            var credit = await _dbContext.Set<Credit>()
                .FirstOrDefaultAsync(c => c.CreditId == request.CreditId, cancellationToken);

            if (credit == null)
            {
                return (null, $"Зачет с Id = {request.CreditId} не найден");
            }

            credit.IsPassed = request.IsPassed;

            if (request.Date.HasValue)
            {
                credit.Date = request.Date.Value;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return (credit, null);
        }

        // Вспомогательный метод: проверяет, что студент и дисциплина существуют и не удалены.
        // Возвращает текст ошибки или null, если всё в порядке
        private async Task<string?> CheckStudentAndDisciplineAsync(int studentId, int disciplineId, CancellationToken cancellationToken)
        {
            var student = await _dbContext.Set<Student>()
                .FirstOrDefaultAsync(s => s.StudentId == studentId, cancellationToken);

            if (student == null)
            {
                return $"Студент с Id = {studentId} не найден";
            }

            if (student.IsDeleted)
            {
                return $"Студент с Id = {studentId} удален";
            }

            var discipline = await _dbContext.Set<Discipline>()
                .FirstOrDefaultAsync(d => d.DisciplineId == disciplineId, cancellationToken);

            if (discipline == null)
            {
                return $"Дисциплина с Id = {disciplineId} не найдена";
            }

            if (discipline.IsDeleted)
            {
                return $"Дисциплина с Id = {disciplineId} удалена";
            }

            return null;
        }
    }
}
