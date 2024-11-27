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
        public void CreateUser(CreateUserDto userDto)
        {
            ValidateUserInput(userDto);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            using (var db = new SqlConnection("connectionString"))
            {
                db.Open();
                var command = new SqlCommand($"INSERT INTO Users (Name, Email, PasswordHash, Role) VALUES ('{userDto.Name}', '{userDto.Email}', '{passwordHash}', '{userDto.Role}')", db);
                command.ExecuteNonQuery();
            }
        }

        public List<string> GetUsers()
        {
            var users = new List<string>();

            using (var db = new SqlConnection("connectionString"))
            {
                db.Open();
                var command = new SqlCommand("SELECT Name FROM Users", db);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(reader.GetString(0));
                    }
                }
            }

            return users;
        }

        public void UpdateUserRole(UpdateUserRoleDto userRoleDto)
        {
            if (!IsValidRole(userRoleDto.NewRole))
            {
                throw new ArgumentException($"Invalid role: {userRoleDto.NewRole}");
            }

            using (var db = new SqlConnection("connectionString"))
            {
                db.Open();
                var command = new SqlCommand($"UPDATE Users SET Role = '{userRoleDto.NewRole}' WHERE Id = {userRoleDto.UserId}", db);
                command.ExecuteNonQuery();
            }
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