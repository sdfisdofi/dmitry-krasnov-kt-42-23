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

        // Выставление зачета студенту
        [HttpPost("AddCredit")]
        public async Task<IActionResult> AddCreditAsync(AddCreditRequest request, CancellationToken cancellationToken = default)
        {
            var (credit, error) = await _performanceService.AddCreditAsync(request, cancellationToken);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok(credit);
        }

        // Изменение зачета
        [HttpPut("UpdateCredit")]
        public async Task<IActionResult> UpdateCreditAsync(UpdateCreditRequest request, CancellationToken cancellationToken = default)
        {
            var (credit, error) = await _performanceService.UpdateCreditAsync(request, cancellationToken);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok(credit);
        }
    }
}
