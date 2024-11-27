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
            if (string.IsNullOrEmpty(userDto.Name) || string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.Password))
            {
                throw new Exception("Invalid input");
            }

            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(userDto.Email))
            {
                throw new Exception("Invalid email");
            }

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
            if (userRoleDto.NewRole != "Admin" && userRoleDto.NewRole != "User")
            {
                throw new Exception("Invalid role");
            }

            using (var db = new SqlConnection("connectionString"))
            {
                db.Open();
                var command = new SqlCommand($"UPDATE Users SET Role = '{userRoleDto.NewRole}' WHERE Id = {userRoleDto.UserId}", db);
                command.ExecuteNonQuery();
            }
        }
    }
}