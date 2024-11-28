using Microsoft.EntityFrameworkCore;

namespace UserService_test_task
{

    public interface IUserService
    {
        Task<int> CreateUserAsync(CreateUserDto userDto);
        Task<List<User>> GetUsersAsync();
        Task UpdateUserRoleAsync(UpdateUserRoleDto roleDto);
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

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task UpdateUserRoleAsync(UpdateUserRoleDto userRoleDto)
        {
            if (!IsValidRole(userRoleDto.NewRole))
            {
                throw new ArgumentException($"Invalid role: {userRoleDto.NewRole}");
            }

            var user = await _context.Users.FindAsync(userRoleDto.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.Role = userRoleDto.NewRole;
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