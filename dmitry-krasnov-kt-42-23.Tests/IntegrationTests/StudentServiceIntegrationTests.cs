using dmitry_krasnov_kt_42_23.Database;
using dmitry_krasnov_kt_42_23.Filters.StudentFilters;
using dmitry_krasnov_kt_42_23.Interfaces.StudentsInterfaces;
using dmitry_krasnov_kt_42_23.Models;
using dmitry_krasnov_kt_42_23.Requests.StudentRequests;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace dmitry_krasnov_kt_42_23.Tests.IntegrationTests
{
    public class StudentServiceIntegrationTests
    {
        private static StudentDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<StudentDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new StudentDbContext(options);
        }

        [Fact]
        public async Task GetStudentsAsync_GroupNameKT4223_TwoObjects()
        {
            // Arrange
            await using var ctx = CreateContext();
            var studentService = new StudentService(ctx);

            var kt = new Group { GroupName = "КТ-42-23", Specialty = "ИВТ", Year = 2023 };
            var ek = new Group { GroupName = "ЭК-31-24", Specialty = "Экономика", Year = 2024 };
            await ctx.Set<Group>().AddRangeAsync(kt, ek);

            var students = new List<Student>
            {
                new Student { LastName = "Кузнецов", FirstName = "Алексей", Group = kt },
                new Student { LastName = "Морозова", FirstName = "Мария", Group = kt },
                new Student { LastName = "Сидорова", FirstName = "Анна", Group = ek }
            };
            await ctx.Set<Student>().AddRangeAsync(students);
            await ctx.SaveChangesAsync();

            // Act
            var filter = new StudentFilter { GroupName = "КТ-42-23" };
            var result = await studentService.GetStudentsAsync(filter, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Length);
            Assert.All(result, s => Assert.Equal(kt.GroupId, s.GroupId));
        }

        [Fact]
        public async Task GetStudentsAsync_LastNameKuz_OneObject()
        {
            // Arrange
            await using var ctx = CreateContext();
            var studentService = new StudentService(ctx);

            var kt = new Group { GroupName = "КТ-42-23", Specialty = "ИВТ", Year = 2023 };
            await ctx.Set<Group>().AddAsync(kt);

            var students = new List<Student>
            {
                new Student { LastName = "Кузнецов", FirstName = "Алексей", Group = kt },
                new Student { LastName = "Морозова", FirstName = "Мария", Group = kt }
            };
            await ctx.Set<Student>().AddRangeAsync(students);
            await ctx.SaveChangesAsync();

            // Act
            var filter = new StudentFilter { LastName = "Кузн" };
            var result = await studentService.GetStudentsAsync(filter, CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal("Кузнецов", result[0].LastName);
        }

        [Fact]
        public async Task AddStudentAsync_DeletedGroup_Error()
        {
            // Arrange
            await using var ctx = CreateContext();
            var studentService = new StudentService(ctx);

            var deletedGroup = new Group { GroupName = "КТ-51-20", Specialty = "ИВТ", Year = 2020, IsDeleted = true };
            await ctx.Set<Group>().AddAsync(deletedGroup);
            await ctx.SaveChangesAsync();

            // Act
            var request = new AddStudentRequest
            {
                LastName = "Новиков",
                FirstName = "Илья",
                GroupId = deletedGroup.GroupId
            };
            var (student, error) = await studentService.AddStudentAsync(request, CancellationToken.None);

            // Assert
            Assert.Null(student);
            Assert.NotNull(error);
            Assert.Contains("удалена", error);

            // Студент не должен был попасть в БД
            Assert.Equal(0, await ctx.Set<Student>().CountAsync());
        }

        [Fact]
        public async Task AddStudentAsync_ValidData_StudentAdded()
        {
            // Arrange
            await using var ctx = CreateContext();
            var studentService = new StudentService(ctx);

            var kt = new Group { GroupName = "КТ-42-23", Specialty = "ИВТ", Year = 2023 };
            await ctx.Set<Group>().AddAsync(kt);
            await ctx.SaveChangesAsync();

            // Act
            var request = new AddStudentRequest
            {
                LastName = "Новиков",
                FirstName = "Илья",
                GroupId = kt.GroupId
            };
            var (student, error) = await studentService.AddStudentAsync(request, CancellationToken.None);

            // Assert
            Assert.Null(error);
            Assert.NotNull(student);
            Assert.Equal("КТ-42-23", student.Group?.GroupName);
            Assert.Equal(1, await ctx.Set<Student>().CountAsync());
        }
    }
}
