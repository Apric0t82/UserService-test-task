using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using UserService_test_task.Extensions;
using UserService_test_task.Context;
using UserService_test_task.Model;
using UserService_test_task.Services;

namespace UserService_test_task.Tests
{
    public class UserServiceTests
    {
        private Mock<AppDbContext> _mockContext;
        private Mock<IPasswordHasher> _mockPasswordHasher;
        private UserService _userService;

        public UserServiceTests()
        {
            _mockContext = new Mock<AppDbContext>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _userService = new UserService(_mockContext.Object, _mockPasswordHasher.Object);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreateUser()
        {
            // Arrange
            var userDto = new CreateUserDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "Test1234",
                Role = "User"
            };

            _mockPasswordHasher.Setup(p => p.HashPassword(userDto.Password))
                      .Returns("hashed_password");

            // Act
            await _userService.CreateUserAsync(userDto);

            // Assert
            _mockContext.Verify(m => m.Users.Add(It.Is<User>(u =>
                u.Name == userDto.Name &&
                u.Email == userDto.Email &&
                u.PasswordHash == "hashed_password" &&
                u.Role == userDto.Role
            )), Times.Once);

            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldNotCreateUser_WhenInputIsInvalid()
        {
            // Arrange
            var invalidUserDto = new CreateUserDto
            {
                Name = "",
                Email = "invalid email",
                Password = "short",
                Role = "InvalidRole"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateUserAsync(invalidUserDto));
            Assert.Contains("Invalid user input", exception.Message);

            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUser_WhenUserIsValid()
        {
            // Arrange
            var userId = 1;

            var existingUser = new User
            {
                Id = userId,
                Name = "Old Name",
                PasswordHash = _mockPasswordHasher.Object.HashPassword("Test1234"),
                Email = "old@example.com",
                Role = "User"
            };

            var updatedUser = new User
            {
                Id = userId,
                Name = "New Name",
                PasswordHash = _mockPasswordHasher.Object.HashPassword("Test12345"),
                Email = "new@example.com",
                Role = "Admin"
            };

            var mockSet = new Mock<DbSet<User>>();

            _mockContext.Setup(m => m.Users.FindAsync(userId))
                       .ReturnsAsync(existingUser);

            // Act
            await _userService.UpdateUserAsync(updatedUser);

            // Assert
            Assert.Equal("Admin", existingUser.Role);

            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;

            var existingUser = new User
            {
                Id = userId,
                Name = "Test User",
                PasswordHash = _mockPasswordHasher.Object.HashPassword("Test1234"),
                Email = "test@example.com",
                Role = "User"
            };

            var mockSet = new Mock<DbSet<User>>();

            _mockContext.Setup(m => m.Users.FindAsync(userId))
                       .ReturnsAsync(existingUser);

            mockSet.Setup(m => m.Remove(existingUser));
            _mockContext.Setup(m => m.Users).Returns(mockSet.Object);


            // Act
            await _userService.DeleteUserAsync(userId);

            // Assert
            mockSet.Verify(m => m.Remove(existingUser), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999; // Non-existent user

            _mockContext.Setup(m => m.Users.FindAsync(userId))
                       .ReturnsAsync((User)null); // User not found

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.DeleteUserAsync(userId));
            Assert.Contains($"User with ID {userId} not found", exception.Message);

            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never); // Ensure no save attempts
        }
    }
}
