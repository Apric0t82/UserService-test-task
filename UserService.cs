namespace UserService_test_task
{

    public interface IUserService
    {
        public void CreateUser(CreateUserDto userDto);
        public List<string> GetUsers();
        public void UpdateUserRole(UpdateUserRoleDto userRoleDto);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public void CreateUser(CreateUserDto userDto)
        {
            ValidateUserInput(userDto);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = passwordHash,
                Role = userDto.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public List<string> GetUsers()
        {
            return _context.Users.Select(u => u.Name).ToList();
        }

        public void UpdateUserRole(UpdateUserRoleDto userRoleDto)
        {
            if (!IsValidRole(userRoleDto.NewRole))
            {
                throw new ArgumentException($"Invalid role: {userRoleDto.NewRole}");
            }

            var user = _context.Users.Find(userRoleDto.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.Role = userRoleDto.NewRole;
            _context.SaveChanges();
        }

        private void ValidateUserInput(CreateUserDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Name) || string.IsNullOrWhiteSpace(userDto.Email) ||
                string.IsNullOrWhiteSpace(userDto.Password) || !IsValidRole(userDto.Role))
            {
                throw new ArgumentException("Invalid user input");
            }

            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(userDto.Email))
            {
                throw new Exception("Invalid email");
            }
        }

        private bool IsValidRole(string role)
        {
            var validRoles = new[] { "User", "Admin", "SuperAdmin" };
            return validRoles.Contains(role);
        }
    }
}