using System.ComponentModel.DataAnnotations;

namespace dmitry_krasnov_kt_42_23.Requests.SpecialityRequests
{
    // Данные для добавления новой специальности
    public class AddSpecialityRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        // Код необязательный
        [MaxLength(20)]
        public string? Code { get; set; }
    }
}