using dmitry_krasnov_kt_42_23.Database;
using dmitry_krasnov_kt_42_23.Filters.PerformanceFilters;
using dmitry_krasnov_kt_42_23.Interfaces.PerformanceInterfaces;
using dmitry_krasnov_kt_42_23.Models;
using dmitry_krasnov_kt_42_23.Requests.PerformanceRequests;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace dmitry_krasnov_kt_42_23.Tests.IntegrationTests
{
    public class PerformanceServiceIntegrationTests
    {
        private static StudentDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<StudentDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new StudentDbContext(options);
        }

        // Общие тестовые данные для тестов успеваемости.
        //
        // Группа КТ-42-23: Кузнецов, Морозова, Удалов (удален)
        // Группа ЭК-31-24: Сидорова
        //
        // Оценки по "Базы данных":
        //   Кузнецов  5 (2025), 4 (2026)
        //   Морозова  3 (2025)
        //   Удалов    2 (2025)  <- удаленный студент, в средний балл не входит
        //   Сидорова  2 (2025)  <- другая группа
        private static async Task<SeedData> SeedAsync(StudentDbContext ctx)
        {
            var kt = new Group { GroupName = "КТ-42-23", Specialty = "ИВТ", Year = 2023 };
            var ek = new Group { GroupName = "ЭК-31-24", Specialty = "Экономика", Year = 2024 };
            await ctx.Set<Group>().AddRangeAsync(kt, ek);

            var kuznetsov = new Student { LastName = "Кузнецов", FirstName = "Алексей", Group = kt };
            var morozova = new Student { LastName = "Морозова", FirstName = "Мария", Group = kt };
            var udalov = new Student { LastName = "Удалов", FirstName = "Олег", Group = kt, IsDeleted = true };
            var sidorova = new Student { LastName = "Сидорова", FirstName = "Анна", Group = ek };
            await ctx.Set<Student>().AddRangeAsync(kuznetsov, morozova, udalov, sidorova);

            var databases = new Discipline { DisciplineName = "Базы данных", Direction = DisciplineDirection.Technical };
            await ctx.Set<Discipline>().AddAsync(databases);

            var grades = new List<Grade>
            {
                new Grade { Student = kuznetsov, Discipline = databases, Value = 5, Date = new DateTime(2025, 12, 25) },
                new Grade { Student = kuznetsov, Discipline = databases, Value = 4, Date = new DateTime(2026, 6, 20) },
                new Grade { Student = morozova,  Discipline = databases, Value = 3, Date = new DateTime(2025, 12, 25) },
                new Grade { Student = udalov,    Discipline = databases, Value = 2, Date = new DateTime(2025, 12, 25) },
                new Grade { Student = sidorova,  Discipline = databases, Value = 2, Date = new DateTime(2025, 12, 26) }
            };
            await ctx.Set<Grade>().AddRangeAsync(grades);

            await ctx.SaveChangesAsync();

            return new SeedData(kt, ek, kuznetsov, udalov, databases);
        }

        // Удобная "коробка" для созданных объектов, чтобы тесты могли взять их Id
        private record SeedData(Group Kt, Group Ek, Student Kuznetsov, Student DeletedStudent, Discipline Databases);

        [Fact]
        public async Task GetGroupDisciplineAverageAsync_KT4223Databases_AverageFour()
        {
            // Arrange
            await using var ctx = CreateContext();
            var service = new PerformanceService(ctx);
            var data = await SeedAsync(ctx);

            // Act
            var filter = new GroupDisciplineAverageFilter
            {
                GroupId = data.Kt.GroupId,
                DisciplineId = data.Databases.DisciplineId
            };
            var (result, error) = await service.GetGroupDisciplineAverageAsync(filter, CancellationToken.None);

            // Assert
            // (5 + 4 + 3) / 3 = 4. Оценка удаленного студента и студента другой группы не учтены
            Assert.Null(error);
            Assert.NotNull(result);
            Assert.Equal(4.0, result.AverageGrade);
            Assert.Equal(3, result.GradesCount);
        }

        [Fact]
        public async Task GetYearAverageAsync_Year2025_Average333()
        {
            // Arrange
            await using var ctx = CreateContext();
            var service = new PerformanceService(ctx);
            await SeedAsync(ctx);

            // Act
            var filter = new YearAverageFilter { Year = 2025 };
            var (result, error) = await service.GetYearAverageAsync(filter, CancellationToken.None);

            // Assert
            // 2025 год: 5, 3, 2 (Сидорова) = 10 / 3 = 3.33. Удаленный студент не учтен
            Assert.Null(error);
            Assert.NotNull(result);
            Assert.Equal(3.33, result.AverageGrade);
            Assert.Equal(3, result.GradesCount);
        }

        [Fact]
        public async Task GetYearAverageAsync_Year2024_Null()
        {
            // Arrange
            await using var ctx = CreateContext();
            var service = new PerformanceService(ctx);
            await SeedAsync(ctx);

            // Act
            var filter = new YearAverageFilter { Year = 2024 };
            var (result, error) = await service.GetYearAverageAsync(filter, CancellationToken.None);

            // Assert
            // За 2024 год оценок нет - средний балл null, а не ошибка
            Assert.Null(error);
            Assert.NotNull(result);
            Assert.Null(result.AverageGrade);
            Assert.Equal(0, result.GradesCount);
        }

        [Fact]
        public async Task GetStudentPerformanceAsync_Kuznetsov_TwoGradesAverage45()
        {
            // Arrange
            await using var ctx = CreateContext();
            var service = new PerformanceService(ctx);
            var data = await SeedAsync(ctx);

            // Act
            var filter = new StudentPerformanceFilter { StudentId = data.Kuznetsov.StudentId };
            var (result, error) = await service.GetStudentPerformanceAsync(filter, CancellationToken.None);

            // Assert
            Assert.Null(error);
            Assert.NotNull(result);
            Assert.Equal("КТ-42-23", result.GroupName);
            Assert.Equal(2, result.Grades.Length);
            Assert.Equal(4.5, result.AverageGrade);
        }

        [Fact]
        public async Task AddGradeAsync_DeletedStudent_Error()
        {
            // Arrange
            await using var ctx = CreateContext();
            var service = new PerformanceService(ctx);
            var data = await SeedAsync(ctx);
            var gradesBefore = await ctx.Set<Grade>().CountAsync();

            // Act
            var request = new AddGradeRequest
            {
                StudentId = data.DeletedStudent.StudentId,
                DisciplineId = data.Databases.DisciplineId,
                Value = 5
            };
            var (grade, error) = await service.AddGradeAsync(request, CancellationToken.None);

            // Assert
            Assert.Null(grade);
            Assert.NotNull(error);
            Assert.Contains("удален", error);

            // Новая оценка не появилась
            Assert.Equal(gradesBefore, await ctx.Set<Grade>().CountAsync());
        }
    }
}
