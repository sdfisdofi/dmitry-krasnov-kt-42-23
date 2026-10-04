using System.ComponentModel.DataAnnotations;

namespace dmitry_krasnov_kt_42_23.Requests.StudentRequests
{
    // Данные для добавления нового студента
    public class AddStudentRequest
    {
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        // Отчество необязательное
        [MaxLength(100)]
        public string? MiddleName { get; set; }

        [Range(1, int.MaxValue)]
        public int GroupId { get; set; }
    }
}
