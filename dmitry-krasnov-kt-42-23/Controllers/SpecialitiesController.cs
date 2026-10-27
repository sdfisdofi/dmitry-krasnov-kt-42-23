using dmitry_krasnov_kt_42_23.Filters.SpecialityFilters;
using dmitry_krasnov_kt_42_23.Interfaces.SpecialitiesInterfaces;
using dmitry_krasnov_kt_42_23.Requests.SpecialityRequests;
using Microsoft.AspNetCore.Mvc;

namespace dmitry_krasnov_kt_42_23.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpecialitiesController : ControllerBase
    {
        private readonly ILogger<SpecialitiesController> _logger;
        private readonly ISpecialityService _specialityService;

        public SpecialitiesController(ILogger<SpecialitiesController> logger, ISpecialityService specialityService)
        {
            _logger = logger;
            _specialityService = specialityService;
        }

        // Получение списка специальностей с фильтрацией
        [HttpPost("GetSpecialitiesByFilter")]
        public async Task<IActionResult> GetSpecialitiesByFilterAsync(SpecialityFilter filter, CancellationToken cancellationToken = default)
        {
            var specialities = await _specialityService.GetSpecialitiesAsync(filter, cancellationToken);

            return Ok(specialities);
        }

        // Добавление специальности
        [HttpPost("AddSpeciality")]
        public async Task<IActionResult> AddSpecialityAsync(AddSpecialityRequest request, CancellationToken cancellationToken = default)
        {
            var speciality = await _specialityService.AddSpecialityAsync(request, cancellationToken);

            return Ok(speciality);
        }

        // Изменение специальности
        [HttpPut("UpdateSpeciality")]
        public async Task<IActionResult> UpdateSpecialityAsync(UpdateSpecialityRequest request, CancellationToken cancellationToken = default)
        {
            var speciality = await _specialityService.UpdateSpecialityAsync(request, cancellationToken);

            if (speciality == null)
            {
                return NotFound($"Специальность с Id = {request.SpecialityId} не найдена");
            }

            return Ok(speciality);
        }

        // Удаление специальности (мягкое)
        [HttpDelete("DeleteSpeciality/{specialityId}")]
        public async Task<IActionResult> DeleteSpecialityAsync(int specialityId, CancellationToken cancellationToken = default)
        {
            var isDeleted = await _specialityService.DeleteSpecialityAsync(specialityId, cancellationToken);

            if (!isDeleted)
            {
                return NotFound($"Специальность с Id = {specialityId} не найдена");
            }

            return Ok($"Специальность с Id = {specialityId} помечена как удаленная");
        }
    }
}