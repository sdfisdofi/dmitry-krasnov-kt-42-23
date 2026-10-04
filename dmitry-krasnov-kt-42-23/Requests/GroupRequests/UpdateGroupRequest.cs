using System.ComponentModel.DataAnnotations;

namespace dmitry_krasnov_kt_42_23.Requests.GroupRequests
{
    // Данные для изменения группы: Id группы, которую меняем, и новые значения полей
    public class UpdateGroupRequest
    {
        [Range(1, int.MaxValue)]
        public int GroupId { get; set; }

        [Required]
        [MaxLength(100)]
        public string GroupName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Specialty { get; set; } = string.Empty;

        [Range(1990, 2100)]
        public int Year { get; set; }
    }
}
