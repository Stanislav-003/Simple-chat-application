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
using TaskSignalR.Requests;

namespace TaskSignalR.Tests
{
    public class ChatControllerTests
    {
        private readonly Mock<IChatService> _mockChatService;
        private readonly Mock<ILogger<ChatController>> _mockLogger;
        private readonly ChatController _controller;

        public ChatControllerTests()
        {
            _mockChatService = new Mock<IChatService>();
            _mockLogger = new Mock<ILogger<ChatController>>();
            _controller = new ChatController(_mockChatService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllChats_ReturnsOkResultWithChats()
        {
            // Arrange
            var chats = new List<ChatDto>
            {
                new ChatDto { Id = 1, Title = "Chat 1", CreatorId = 1 },
                new ChatDto { Id = 2, Title = "Chat 2", CreatorId = 2 }
            };
            _mockChatService.Setup(service => service.GetAllChats()).ReturnsAsync(chats);

            // Act
            var result = await _controller.GetAllChats();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnChats = Assert.IsType<List<ChatResponse>>(okResult.Value);
            Assert.Equal(chats.Count, returnChats.Count);
        }

        [Fact]
        public async Task GetChatById_ReturnsOkResultWithChat()
        {
            // Arrange
            var chat = new ChatDto { Id = 1, Title = "Chat 1", CreatorId = 1 };
            _mockChatService.Setup(service => service.GetChatById(1)).ReturnsAsync(chat);

            // Act
            var result = await _controller.GetChatById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnChat = Assert.IsType<ChatResponse>(okResult.Value);
            Assert.Equal(chat.Id, returnChat.Id);
        }

        [Fact]
        public async Task GetChatById_ReturnsNotFoundResult_WhenChatNotFound()
        {
            // Arrange
            _mockChatService.Setup(service => service.GetChatById(1)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetChatById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Chat not found", notFoundResult.Value);
        }

        [Fact]
        public async Task CreateChat_ReturnsOkResult()
        {
            // Arrange
            var chatRequest = new ChatRequest { Title = "New Chat", CreatorId = 1 };
            _mockChatService.Setup(service => service.CreateChat(It.IsAny<ChatDto>())).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateChat(chatRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Chat was created", okResult.Value);
        }

        [Fact]
        public async Task CreateChat_ReturnsBadRequestResult_WhenChatAlreadyExists()
        {
            // Arrange
            var chatRequest = new ChatRequest { Title = "Existing Chat", CreatorId = 1 };
            _mockChatService.Setup(service => service.CreateChat(It.IsAny<ChatDto>())).ThrowsAsync(new InvalidOperationException("Chat with the same title and creator already exists"));

            // Act
            var result = await _controller.CreateChat(chatRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Chat with the same title and creator already exists", badRequestResult.Value);
        }

        [Fact]
        public async Task AddMessage_ReturnsOkResult()
        {
            // Arrange
            var messageRequest = new MessageRequest { Content = "Hello", ChatId = 1, UserId = 1 };
            _mockChatService.Setup(service => service.AddMessage(It.IsAny<MessageDto>())).ReturnsAsync(true);

            // Act
            var result = await _controller.AddMessage(messageRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Message send", okResult.Value);
        }

        [Fact]
        public async Task AddMessage_ReturnsNotFoundResult_WhenChatOrUserNotFound()
        {
            // Arrange
            var messageRequest = new MessageRequest { Content = "Hello", ChatId = 1, UserId = 1 };
            _mockChatService.Setup(service => service.AddMessage(It.IsAny<MessageDto>())).ThrowsAsync(new KeyNotFoundException("Chat or User not found"));

            // Act
            var result = await _controller.AddMessage(messageRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Chat or User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task AddUserToChat_ReturnsOkResult()
        {
            // Arrange
            var userRequest = new UserRequest { ChatId = 1, UserId = 2 };
            _mockChatService.Setup(service => service.AddUserToChat(1, 2)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddUserToChat(userRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("User add", okResult.Value);
        }

        [Fact]
        public async Task AddUserToChat_ReturnsNotFoundResult_WhenChatOrUserNotFound()
        {
            // Arrange
            var userRequest = new UserRequest { ChatId = 1, UserId = 2 };
            _mockChatService.Setup(service => service.AddUserToChat(1, 2)).ThrowsAsync(new KeyNotFoundException("Chat or User not found"));

            // Act
            var result = await _controller.AddUserToChat(userRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Chat or User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteChat_ReturnsNoContentResult()
        {
            // Arrange
            var chatRequest = new ChatRequest { Title = "Chat 1", CreatorId = 1 };
            _mockChatService.Setup(service => service.DeleteChat(It.IsAny<ChatDto>())).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteChat(chatRequest);

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task DeleteChat_ReturnsNotFoundResult_WhenChatNotFound()
        {
            // Arrange
            var chatRequest = new ChatRequest { Title = "Non-existing Chat", CreatorId = 1 };
            _mockChatService.Setup(service => service.DeleteChat(It.IsAny<ChatDto>())).ThrowsAsync(new KeyNotFoundException("Chat not found"));

            // Act
            var result = await _controller.DeleteChat(chatRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Chat not found", notFoundResult.Value);
        }

        [Fact]
        public async Task RemoveUserFromChat_ReturnsOkResult()
        {
            // Arrange
            var userRequest = new UserRequest { ChatId = 1, UserId = 2 };
            _mockChatService.Setup(service => service.RemoveUserFromChat(1, 2)).ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveUserFromChat(userRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("User remove from chat", okResult.Value);
        }

        [Fact]
        public async Task RemoveUserFromChat_ReturnsNotFoundResult_WhenUserNotInChat()
        {
            // Arrange
            var userRequest = new UserRequest { ChatId = 1, UserId = 2 };
            _mockChatService.Setup(service => service.RemoveUserFromChat(1, 2)).ThrowsAsync(new KeyNotFoundException("User is not in the chat"));

            // Act
            var result = await _controller.RemoveUserFromChat(userRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("User is not in the chat", notFoundResult.Value);
        }
    }
}
