using dmitry_krasnov_kt_42_23.Filters.StudentFilters;
using dmitry_krasnov_kt_42_23.Interfaces.StudentsInterfaces;
using dmitry_krasnov_kt_42_23.Requests.StudentRequests;
using Microsoft.AspNetCore.Mvc;

namespace dmitry_krasnov_kt_42_23.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly ILogger<StudentsController> _logger;
        private readonly IStudentService _studentService;

        public StudentsController(ILogger<StudentsController> logger, IStudentService studentService)
        {
            _logger = logger;
            _studentService = studentService;
        }

        // Получение списка студентов с фильтрацией
        [HttpPost("GetStudentsByFilter")]
        public async Task<IActionResult> GetStudentsByFilterAsync(StudentFilter filter, CancellationToken cancellationToken = default)
        {
            var students = await _studentService.GetStudentsAsync(filter, cancellationToken);

            return Ok(students);
        }

        // Добавление студента
        [HttpPost("AddStudent")]
        public async Task<IActionResult> AddStudentAsync(AddStudentRequest request, CancellationToken cancellationToken = default)
        {
            var (student, error) = await _studentService.AddStudentAsync(request, cancellationToken);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok(student);
        }

        // Изменение студента
        [HttpPut("UpdateStudent")]
        public async Task<IActionResult> UpdateStudentAsync(UpdateStudentRequest request, CancellationToken cancellationToken = default)
        {
            var (student, error) = await _studentService.UpdateStudentAsync(request, cancellationToken);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok(student);
        }

        // Удаление студента (мягкое)
        [HttpDelete("DeleteStudent/{studentId}")]
        public async Task<IActionResult> DeleteStudentAsync(int studentId, CancellationToken cancellationToken = default)
        {
            var isDeleted = await _studentService.DeleteStudentAsync(studentId, cancellationToken);

            if (!isDeleted)
            {
                return NotFound($"Студент с Id = {studentId} не найден");
            }

            return Ok($"Студент с Id = {studentId} помечен как удаленный");
        }
    }
}
