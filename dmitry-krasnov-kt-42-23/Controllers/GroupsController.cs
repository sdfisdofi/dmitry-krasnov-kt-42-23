using dmitry_krasnov_kt_42_23.Filters.GroupFilters;
using dmitry_krasnov_kt_42_23.Interfaces.GroupsInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace dmitry_krasnov_kt_42_23.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly ILogger<GroupsController> _logger;
        private readonly IGroupService _groupService;

        // Контроллер получает интерфейс через Dependency Injection,
        // про конкретную реализацию (GroupService) он ничего не знает
        public GroupsController(ILogger<GroupsController> logger, IGroupService groupService)
        {
            _logger = logger;
            _groupService = groupService;
        }

        // Получение списка групп с фильтрацией.
        // Фильтр передается в теле запроса (Request body) в формате JSON
        [HttpPost("GetGroupsByFilter")]
        public async Task<IActionResult> GetGroupsByFilterAsync(GroupFilter filter, CancellationToken cancellationToken = default)
        {
            var groups = await _groupService.GetGroupsAsync(filter, cancellationToken);

            return Ok(groups);
        }
    }
}
