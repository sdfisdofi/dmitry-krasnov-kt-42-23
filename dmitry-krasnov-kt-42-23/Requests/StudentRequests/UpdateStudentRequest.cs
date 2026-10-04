using System.ComponentModel.DataAnnotations;

namespace dmitry_krasnov_kt_42_23.Requests.StudentRequests
{
    // Данные для изменения студента: Id студента, которого меняем, и новые значения полей
    public class UpdateStudentRequest
    {
        [Range(1, int.MaxValue)]
        public int StudentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? MiddleName { get; set; }

        // Можно перевести студента в другую группу
        [Range(1, int.MaxValue)]
        public int GroupId { get; set; }
    }
}
