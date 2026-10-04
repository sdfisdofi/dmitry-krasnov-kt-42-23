using dmitry_krasnov_kt_42_23.Filters.DisciplineFilters;
using dmitry_krasnov_kt_42_23.Interfaces.DisciplinesInterfaces;
using dmitry_krasnov_kt_42_23.Requests.DisciplineRequests;
using Microsoft.AspNetCore.Mvc;

namespace dmitry_krasnov_kt_42_23.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisciplinesController : ControllerBase
    {
        private readonly ILogger<DisciplinesController> _logger;
        private readonly IDisciplineService _disciplineService;

        public DisciplinesController(ILogger<DisciplinesController> logger, IDisciplineService disciplineService)
        {
            _logger = logger;
            _disciplineService = disciplineService;
        }

        // Получение списка дисциплин с фильтрацией
        [HttpPost("GetDisciplinesByFilter")]
        public async Task<IActionResult> GetDisciplinesByFilterAsync(DisciplineFilter filter, CancellationToken cancellationToken = default)
        {
            var disciplines = await _disciplineService.GetDisciplinesAsync(filter, cancellationToken);

            return Ok(disciplines);
        }

        // Добавление дисциплины
        [HttpPost("AddDiscipline")]
        public async Task<IActionResult> AddDisciplineAsync(AddDisciplineRequest request, CancellationToken cancellationToken = default)
        {
            var discipline = await _disciplineService.AddDisciplineAsync(request, cancellationToken);

            return Ok(discipline);
        }

        // Изменение дисциплины
        [HttpPut("UpdateDiscipline")]
        public async Task<IActionResult> UpdateDisciplineAsync(UpdateDisciplineRequest request, CancellationToken cancellationToken = default)
        {
            var discipline = await _disciplineService.UpdateDisciplineAsync(request, cancellationToken);

            if (discipline == null)
            {
                return NotFound($"Дисциплина с Id = {request.DisciplineId} не найдена");
            }

            return Ok(discipline);
        }

        // Удаление дисциплины (мягкое)
        [HttpDelete("DeleteDiscipline/{disciplineId}")]
        public async Task<IActionResult> DeleteDisciplineAsync(int disciplineId, CancellationToken cancellationToken = default)
        {
            var isDeleted = await _disciplineService.DeleteDisciplineAsync(disciplineId, cancellationToken);

            if (!isDeleted)
            {
                return NotFound($"Дисциплина с Id = {disciplineId} не найдена");
            }

            return Ok($"Дисциплина с Id = {disciplineId} помечена как удаленная");
        }
    }
}
