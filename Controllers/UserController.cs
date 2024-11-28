using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserService_test_task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
        {
            var userId = await _userService.CreateUserAsync(userDto);
            return CreatedAtAction(nameof(CreateUser), new { id = userId }, null);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNames()
        {
            var users = await _userService.GetUsersAsync();
            var userNames = users.Select(u => u.Name).ToList();
            return Ok(userNames);
        }

        [HttpPut("role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleDto roleDto)
        {
            await _userService.UpdateUserRoleAsync(roleDto);
            return NoContent();
        }
    }
}
