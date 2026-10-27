using dmitry_krasnov_kt_42_23.Filters.GroupFilters;
using dmitry_krasnov_kt_42_23.Interfaces.GroupsInterfaces;
using dmitry_krasnov_kt_42_23.Requests.GroupRequests;
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

        // Добавление группы
        [HttpPost("AddGroup")]
        public async Task<IActionResult> AddGroupAsync(AddGroupRequest request, CancellationToken cancellationToken = default)
        {
            var (group, error) = await _groupService.AddGroupAsync(request, cancellationToken);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok(group);
        }

        // Изменение группы
        [HttpPut("UpdateGroup")]
        public async Task<IActionResult> UpdateGroupAsync(UpdateGroupRequest request, CancellationToken cancellationToken = default)
        {
            var (group, error) = await _groupService.UpdateGroupAsync(request, cancellationToken);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok(group);
        }

        // Удаление группы (мягкое) вместе с ее студентами
        [HttpDelete("DeleteGroup/{groupId}")]
        public async Task<IActionResult> DeleteGroupAsync(int groupId, CancellationToken cancellationToken = default)
        {
            var isDeleted = await _groupService.DeleteGroupAsync(groupId, cancellationToken);

            if (!isDeleted)
            {
                return NotFound($"Группа с Id = {groupId} не найдена");
            }

            return Ok($"Группа с Id = {groupId} и ее студенты помечены как удаленные");
        }
    }
}
