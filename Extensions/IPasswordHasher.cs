namespace UserService_test_task.Extensions
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
    }

    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty", nameof(password));
            }

            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }

}