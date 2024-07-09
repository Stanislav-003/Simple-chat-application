using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSignalR.BLL.DTO;
using TaskSignalR.BLL.IService;
using TaskSignalR.Contracts;
using TaskSignalR.Controllers;

namespace TaskSignalR.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _controller = new UserController(_mockUserService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOkResultWithUsers()
        {
            // Arrange
            var users = new List<UserDto>
            {
                new UserDto { Id = 1, Name = "User 1" },
                new UserDto { Id = 2, Name = "User 2" }
            };
            _mockUserService.Setup(service => service.GetAllUsers()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnUsers = Assert.IsType<List<UserResponse>>(okResult.Value);
            Assert.Equal(users.Count, returnUsers.Count);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockUserService.Setup(service => service.GetAllUsers()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);
        }

        [Fact]
        public async Task GetUserById_ReturnsOkResultWithUser()
        {
            // Arrange
            var user = new UserDto { Id = 1, Name = "User 1" };
            _mockUserService.Setup(service => service.GetUserById(1)).ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnUser = Assert.IsType<UserResponse>(okResult.Value);
            Assert.Equal(user.Id, returnUser.Id);
        }

        [Fact]
        public async Task GetUserById_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockUserService.Setup(service => service.GetUserById(1)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetUserById(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFoundResult_WhenUserNotFound()
        {
            // Arrange
            _mockUserService.Setup(service => service.GetUserById(1)).ReturnsAsync((UserDto)null);

            // Act
            var result = await _controller.GetUserById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("User with ID 1 not found", notFoundResult.Value);
        }

        [Fact]
        public async Task CreateUser_ReturnsCreatedResult()
        {
            // Arrange
            _mockUserService.Setup(service => service.CreateUser("New User")).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateUser("New User");

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(true, createdResult.Value);
        }

        [Fact]
        public async Task CreateUser_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockUserService.Setup(service => service.CreateUser("New User")).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.CreateUser("New User");

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);
        }

        [Fact]
        public async Task CreateUser_ReturnsBadRequestResult_WhenFailed()
        {
            // Arrange
            _mockUserService.Setup(service => service.CreateUser("New User")).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateUser("New User");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Failed to create user", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOkResult()
        {
            // Arrange
            _mockUserService.Setup(service => service.UpdateUser(It.IsAny<UserDto>())).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateUser(1, "Updated User");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(true, okResult.Value);
        }

        [Fact]
        public async Task UpdateUser_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockUserService.Setup(service => service.UpdateUser(It.IsAny<UserDto>())).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.UpdateUser(1, "Updated User");

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);
        }

        [Fact]
        public async Task UpdateUser_ReturnsBadRequestResult_WhenFailed()
        {
            // Arrange
            _mockUserService.Setup(service => service.UpdateUser(It.IsAny<UserDto>())).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateUser(1, "Updated User");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("User not updated", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContentResult()
        {
            // Arrange
            _mockUserService.Setup(service => service.DeleteUser(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUser(1);

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockUserService.Setup(service => service.DeleteUser(1)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.DeleteUser(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);
        }

        [Fact]
        public async Task DeleteUser_ReturnsBadRequestResult_WhenFailed()
        {
            // Arrange
            _mockUserService.Setup(service => service.DeleteUser(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteUser(1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("User not deleted", badRequestResult.Value);
        }
    }
}
