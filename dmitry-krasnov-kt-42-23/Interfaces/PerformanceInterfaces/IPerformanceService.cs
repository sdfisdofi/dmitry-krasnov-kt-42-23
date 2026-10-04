using dmitry_krasnov_kt_42_23.Database;
using dmitry_krasnov_kt_42_23.Models;
using dmitry_krasnov_kt_42_23.Requests.PerformanceRequests;
using Microsoft.EntityFrameworkCore;

namespace dmitry_krasnov_kt_42_23.Interfaces.PerformanceInterfaces
{
    // Интерфейс (абстракция): описывает, ЧТО умеет сервис успеваемости
    public interface IPerformanceService
    {
        // Все методы возвращают пару: результат (если всё успешно) или текст ошибки
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
