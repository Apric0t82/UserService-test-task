using Microsoft.EntityFrameworkCore;

namespace UserService_test_task
{

    public interface IUserService
    {
        Task<int> CreateUserAsync(CreateUserDto userDto);
        Task<List<UserDTO>> GetUsersAsync();
        Task<UserDTO> GetUserByIdAsync(int id);
        Task UpdateUserAsync(User user);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateUserAsync(CreateUserDto userDto)
        {
            ValidateUserInput(userDto);

            var validatedEmail = new Email(userDto.Email);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var user = new User
            {
                Name = userDto.Name,
                Email = validatedEmail,
                PasswordHash = passwordHash,
                Role = userDto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user.Id;
        }

        public async Task<List<UserDTO>> GetUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            }).ToList();
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        private void ValidateUserInput(CreateUserDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Name) || string.IsNullOrWhiteSpace(userDto.Email) ||
                string.IsNullOrWhiteSpace(userDto.Password) || !IsValidRole(userDto.Role))
            {
                throw new ArgumentException("Invalid user input");
            }
        }

        private bool IsValidRole(string role)
        {
            var validRoles = new[] { "User", "Admin", "SuperAdmin" };
            return validRoles.Contains(role);
        }
    }
}