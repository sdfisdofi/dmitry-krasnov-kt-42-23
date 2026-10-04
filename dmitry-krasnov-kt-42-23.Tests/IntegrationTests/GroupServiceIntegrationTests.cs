using dmitry_krasnov_kt_42_23.Database;
using dmitry_krasnov_kt_42_23.Filters.GroupFilters;
using dmitry_krasnov_kt_42_23.Interfaces.GroupsInterfaces;
using dmitry_krasnov_kt_42_23.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace dmitry_krasnov_kt_42_23.Tests.IntegrationTests
{
    // Интеграционные тесты: проверяем работу сервиса вместе с БД.
    // Используется БД в памяти (InMemory) - она создается для теста и исчезает после него
    public class GroupServiceIntegrationTests
    {
        // Каждый тест получает СВОЮ пустую БД (уникальное имя через Guid),
        // чтобы данные одного теста не мешали другому
        private static StudentDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<StudentDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new StudentDbContext(options);
        }

        [Fact]
        public async Task GetGroupsAsync_Year2023_TwoObjects()
        {
            // Arrange
            await using var ctx = CreateContext();
            var groupService = new GroupService(ctx);

            var groups = new List<Group>
            {
                new Group { GroupName = "КТ-42-23", Specialty = "Информатика и вычислительная техника", Year = 2023 },
                new Group { GroupName = "КТ-41-23", Specialty = "Информатика и вычислительная техника", Year = 2023 },
                new Group { GroupName = "ЭК-31-24", Specialty = "Экономика", Year = 2024 }
            };
            await ctx.Set<Group>().AddRangeAsync(groups);
            await ctx.SaveChangesAsync();

            // Act
            var filter = new GroupFilter { Year = 2023 };
            var result = await groupService.GetGroupsAsync(filter, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Length);
            Assert.All(result, g => Assert.Equal(2023, g.Year));
        }

        [Fact]
        public async Task GetGroupsAsync_IsDeletedTrue_OneObject()
        {
            // Arrange
            await using var ctx = CreateContext();
            var groupService = new GroupService(ctx);

            var groups = new List<Group>
            {
                new Group { GroupName = "КТ-42-23", Specialty = "ИВТ", Year = 2023 },
                new Group { GroupName = "КТ-51-20", Specialty = "ИВТ", Year = 2020, IsDeleted = true }
            };
            await ctx.Set<Group>().AddRangeAsync(groups);
            await ctx.SaveChangesAsync();

            // Act
            var filter = new GroupFilter { IsDeleted = true };
            var result = await groupService.GetGroupsAsync(filter, CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal("КТ-51-20", result[0].GroupName);
        }

        [Fact]
        public async Task DeleteGroupAsync_GroupWithTwoStudents_GroupAndStudentsDeleted()
        {
            // Arrange
            await using var ctx = CreateContext();
            var groupService = new GroupService(ctx);

            var groupToDelete = new Group { GroupName = "КТ-41-23", Specialty = "ИВТ", Year = 2023 };
            var otherGroup = new Group { GroupName = "ЭК-31-24", Specialty = "Экономика", Year = 2024 };
            await ctx.Set<Group>().AddRangeAsync(groupToDelete, otherGroup);

            // Студентов привязываем через навигационное свойство Group,
            // тогда не нужно заранее знать Id групп
            var students = new List<Student>
            {
                new Student { LastName = "Иванов", FirstName = "Иван", Group = groupToDelete },
                new Student { LastName = "Петров", FirstName = "Пётр", Group = groupToDelete },
                new Student { LastName = "Сидорова", FirstName = "Анна", Group = otherGroup }
            };
            await ctx.Set<Student>().AddRangeAsync(students);
            await ctx.SaveChangesAsync();

            // Act
            var result = await groupService.DeleteGroupAsync(groupToDelete.GroupId, CancellationToken.None);

            // Assert
            Assert.True(result);

            var deletedGroup = await ctx.Set<Group>().FirstAsync(g => g.GroupId == groupToDelete.GroupId);
            Assert.True(deletedGroup.IsDeleted);

            // Оба студента удаленной группы помечены как удаленные
            var studentsOfDeletedGroup = await ctx.Set<Student>()
                .Where(s => s.GroupId == groupToDelete.GroupId)
                .ToListAsync();
            Assert.Equal(2, studentsOfDeletedGroup.Count);
            Assert.All(studentsOfDeletedGroup, s => Assert.True(s.IsDeleted));

            // Студент другой группы не затронут
            var otherStudent = await ctx.Set<Student>().FirstAsync(s => s.LastName == "Сидорова");
            Assert.False(otherStudent.IsDeleted);
        }

        [Fact]
        public async Task DeleteGroupAsync_NotExistingId_False()
        {
            // Arrange
            await using var ctx = CreateContext();
            var groupService = new GroupService(ctx);

            // Act
            var result = await groupService.DeleteGroupAsync(999, CancellationToken.None);

            // Assert
            Assert.False(result);
        }
    }
}
