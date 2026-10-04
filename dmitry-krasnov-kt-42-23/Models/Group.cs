using System.Text.RegularExpressions;

namespace dmitry_krasnov_kt_42_23.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;

        // Специальность (например "Информатика и вычислительная техника")
        public string Specialty { get; set; } = string.Empty;

        // Год набора группы (например 2023)
        public int Year { get; set; }

        // Мягкое удаление: true - группа удалена
        public bool IsDeleted { get; set; }

        // Проверка названия группы на соответствие шаблону:
        // 2-4 заглавные буквы, дефис, 2 цифры, дефис, 2 цифры. Пример: КТ-42-23
        // Метод не обращается к БД и другим классам, поэтому его удобно проверять unit-тестами
        public bool IsValidGroupName()
        {
            return Regex.IsMatch(GroupName, @"^[А-ЯЁA-Z]{2,4}-\d{2}-\d{2}$");
        }
    }
}
