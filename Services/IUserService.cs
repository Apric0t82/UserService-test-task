using UserService_test_task.Model;

namespace UserService_test_task.Services
{
    public interface IUserService
    {
        Task<int> CreateUserAsync(CreateUserDto userDto);
        Task<List<UserDTO>> GetUsersAsync();
        Task<UserDTO> GetUserByIdAsync(int id);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
}