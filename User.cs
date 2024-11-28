namespace UserService_test_task
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Email? Email { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Email? Email { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    public class CreateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class UpdateUserRoleDto
    {
        public int UserId { get; set; }
        public string NewRole { get; set; } = string.Empty;
    }

    public class Email 
    {
        private readonly System.Text.RegularExpressions.Regex _emailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public Email(string email) 
        {
            Value = !_emailRegex.IsMatch(email) ? throw new Exception("Invalid email") : email;
        }

        public string Value { get; set; }
    }
}
