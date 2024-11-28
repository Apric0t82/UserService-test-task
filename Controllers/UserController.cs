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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (user == null || user.Id != id) {  
                return BadRequest(); 
            }

            var existing = await _userService.GetUserByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }
            
            await _userService.UpdateUserAsync(user);

            return NoContent();
        }
    }
}
