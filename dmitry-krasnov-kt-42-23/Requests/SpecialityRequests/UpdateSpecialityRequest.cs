using System.ComponentModel.DataAnnotations;

namespace dmitry_krasnov_kt_42_23.Requests.SpecialityRequests
{
    // Данные для изменения специальности: Id специальности, которую меняем, и новые значения полей
    public class UpdateSpecialityRequest
    {
        [Range(1, int.MaxValue)]
        public int SpecialityId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Code { get; set; }
    }
}