using dmitry_krasnov_kt_42_23.Models;
using Xunit;

namespace dmitry_krasnov_kt_42_23.Tests.UnitTests
{
    public class GradeTests
    {
        [Fact]
        public void IsPositive_Value5_True()
        {
            // Arrange
            var grade = new Grade { Value = 5 };

            // Act
            var result = grade.IsPositive();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPositive_Value2_False()
        {
            // Arrange
            var grade = new Grade { Value = 2 };

            // Act
            var result = grade.IsPositive();

            // Assert
            Assert.False(result);
        }

        // [Theory] - один тест, который запускается несколько раз с разными данными.
        // Каждая строка [InlineData] - отдельный тест-кейс: (оценка, ожидаемый результат)
        [Theory]
        [InlineData(2, false)]
        [InlineData(3, true)]
        [InlineData(4, true)]
        [InlineData(5, true)]
        public void IsPositive_AllValues_ExpectedResult(int value, bool expected)
        {
            // Arrange
            var grade = new Grade { Value = value };

            // Act
            var result = grade.IsPositive();

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
