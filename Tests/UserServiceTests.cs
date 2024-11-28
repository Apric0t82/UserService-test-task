using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;

namespace UserService_test_task.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task CreateUserAsync_ShouldCreateUser()
        {
            // Arrange
            var mockContext = new Mock<AppDbContext>();
            var userService = new UserService(mockContext.Object);
            var userDto = new CreateUserDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "Test1234",
                Role = "User"
            };

            // Act
            await userService.CreateUserAsync(userDto);

            // Assert
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldNotCreateUser_WhenInputIsInvalid()
        {
            // Arrange
            var mockContext = new Mock<AppDbContext>();
            var userService = new UserService(mockContext.Object);
            var invalidUserDto = new CreateUserDto
            {
                Name = "",
                Email = "invalid email",
                Password = "short",
                Role = "InvalidRole"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => userService.CreateUserAsync(invalidUserDto));
            Assert.Contains("Invalid user input", exception.Message);

            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test1234"),
                Email = "old@example.com",
                Role = "User"
            };

            var updatedUser = new User
            {
                Id = userId,
                Name = "New Name",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test12345"),
                Email = "new@example.com",
                Role = "Admin"
            };

            var mockSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<AppDbContext>();

            mockContext.Setup(m => m.Users.FindAsync(userId))
                       .ReturnsAsync(existingUser);

            var userService = new UserService(mockContext.Object);

            // Act
            await userService.UpdateUserAsync(updatedUser);

            // Assert
            Assert.Equal("Admin", existingUser.Role);

            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test1234"),
                Email = "test@example.com",
                Role = "User"
            };

            var mockSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<AppDbContext>();

            mockContext.Setup(m => m.Users.FindAsync(userId))
                       .ReturnsAsync(existingUser);

            mockSet.Setup(m => m.Remove(existingUser));
            mockContext.Setup(m => m.Users).Returns(mockSet.Object);

            var userService = new UserService(mockContext.Object);

            // Act
            await userService.DeleteUserAsync(userId);

            // Assert
            mockSet.Verify(m => m.Remove(existingUser), Times.Once);
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999; // Non-existent user
            var mockContext = new Mock<AppDbContext>();

            mockContext.Setup(m => m.Users.FindAsync(userId))
                       .ReturnsAsync((User)null); // User not found

            var userService = new UserService(mockContext.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => userService.DeleteUserAsync(userId));
            Assert.Contains($"User with ID {userId} not found", exception.Message);

            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never); // Ensure no save attempts
        }
    }
}
