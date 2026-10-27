using dmitry_krasnov_kt_42_23.Filters.PerformanceFilters;
using dmitry_krasnov_kt_42_23.Interfaces.PerformanceInterfaces;
using dmitry_krasnov_kt_42_23.Requests.PerformanceRequests;
using Microsoft.AspNetCore.Mvc;

namespace dmitry_krasnov_kt_42_23.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly ILogger<PerformanceController> _logger;
        private readonly IPerformanceService _performanceService;

        public PerformanceController(ILogger<PerformanceController> logger, IPerformanceService performanceService)
        {
            _logger = logger;
            _performanceService = performanceService;
        }

        // ===================== Получение успеваемости =====================

        // Оценки конкретного студента
        [HttpPost("GetStudentPerformance")]
        public async Task<IActionResult> GetStudentPerformanceAsync(StudentPerformanceFilter filter, CancellationToken cancellationToken = default)
        {
            var (result, error) = await _performanceService.GetStudentPerformanceAsync(filter, cancellationToken);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok(result);
        }

        // Средний балл по предмету в группе
        [HttpPost("GetGroupDisciplineAverage")]
        public async Task<IActionResult> GetGroupDisciplineAverageAsync(GroupDisciplineAverageFilter filter, CancellationToken cancellationToken = default)
        {
            var (result, error) = await _performanceService.GetGroupDisciplineAverageAsync(filter, cancellationToken);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok(result);
        }

        // Средний балл по году
        [HttpPost("GetYearAverage")]
        public async Task<IActionResult> GetYearAverageAsync(YearAverageFilter filter, CancellationToken cancellationToken = default)
        {
            var (result, error) = await _performanceService.GetYearAverageAsync(filter, cancellationToken);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok(result);
        }

        // ================= Добавление и изменение оценок ==================

        // Выставление оценки студенту
        [HttpPost("AddGrade")]
        public async Task<IActionResult> AddGradeAsync(AddGradeRequest request, CancellationToken cancellationToken = default)
        {
            var (grade, error) = await _performanceService.AddGradeAsync(request, cancellationToken);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok(grade);
        }

        // Изменение оценки
        [HttpPut("UpdateGrade")]
        public async Task<IActionResult> UpdateGradeAsync(UpdateGradeRequest request, CancellationToken cancellationToken = default)
        {
            var (grade, error) = await _performanceService.UpdateGradeAsync(request, cancellationToken);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok(grade);
        }
    }
}
