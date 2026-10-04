using dmitry_krasnov_kt_42_23.Models;
using Xunit;

namespace dmitry_krasnov_kt_42_23.Tests.UnitTests
{
    // Unit-тесты: проверяем только сам класс Group, без БД и других модулей.
    // Шаблон названия теста: НазваниеМетода_ИспользуемоеЗначение_ОжидаемыйРезультат
    public class GroupTests
    {
        [Fact]
        public void IsValidGroupName_KT4223_True()
        {
            // Arrange - подготовка тестовых данных
            var group = new Group { GroupName = "КТ-42-23" };

            // Act - вызов проверяемого метода
            var result = group.IsValidGroupName();

            // Assert - сравнение результата с ожидаемым
            Assert.True(result);
        }

        [Fact]
        public void IsValidGroupName_LatinKT3120_True()
        {
            // Arrange
            var group = new Group { GroupName = "KT-31-20" };

            // Act
            var result = group.IsValidGroupName();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidGroupName_WithoutDashes_False()
        {
            // Arrange
            var group = new Group { GroupName = "КТ4223" };

            // Act
            var result = group.IsValidGroupName();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidGroupName_LowerCase_False()
        {
            // Arrange
            var group = new Group { GroupName = "кт-42-23" };

            // Act
            var result = group.IsValidGroupName();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidGroupName_EmptyString_False()
        {
            // Arrange
            var group = new Group { GroupName = "" };

            // Act
            var result = group.IsValidGroupName();

            // Assert
            Assert.False(result);
        }
    }
}
