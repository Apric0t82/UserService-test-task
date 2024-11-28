using System.ComponentModel.DataAnnotations;

namespace UserService_test_task
{
    public class User
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public required string PasswordHash { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression("^(User|Admin|SuperAdmin)$", ErrorMessage = "Role must be User, Admin, or SuperAdmin.")]
        public required string Role { get; set; }
    }

    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class CreateUserDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression("^(User|Admin|SuperAdmin)$", ErrorMessage = "Role must be User, Admin, or SuperAdmin.")]
        public required string Role { get; set; }
    }
}
