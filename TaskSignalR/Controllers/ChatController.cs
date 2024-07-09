using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using TaskSignalR.BLL.DTO;
using TaskSignalR.BLL.IService;
using TaskSignalR.Contracts;
using TaskSignalR.DAL.Models;
using TaskSignalR.Requests;

namespace TaskSignalR.Controllers
{
    /// <summary>
    /// Controller for handling chat-related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatController"/> class.
        /// </summary>
        /// <param name="chatService">The chat service dependency.</param>
        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        /// <summary>
        /// Get all chats.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/Chat/GetAll
        /// 
        /// </remarks>
        /// <returns>A list of all chats.</returns>
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ChatResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ChatResponse>>> GetAllChats()
        {
            try
            {
                var chats = await _chatService.GetAllChats();
                return Ok(chats.Select(c => new ChatResponse
                {
                    Id = c.Id,
                    Title = c.Title,
                    CreatorId = c.CreatorId,
                }).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all chats");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get a chat by its ID.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/Chat/GetById?id=1
        /// 
        /// </remarks>
        /// <param name="id">The ID of the chat.</param>
        /// <returns>The chat with the specified ID.</returns>
        [HttpGet("GetById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChatResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ChatResponse>> GetChatById(int id)
        {
            try
            {
                var chat = await _chatService.GetChatById(id);

                return Ok(new ChatResponse
                {
                    Id = chat.Id,
                    Title = chat.Title,
                    CreatorId = chat.CreatorId,
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Chat not found");
                return NotFound("Chat not found");
            }
        }

        /// <summary>
        /// Search chats by title.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/Chat/GetByTitle?title=General
        /// 
        /// </remarks>
        /// <param name="title">The title to search for.</param>
        /// <returns>A list of chats matching the search criteria.</returns>
        [HttpGet("GetByTitle")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ChatResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ChatResponse>>> SearchChats(string title)
        {
            try
            {
                var chats = await _chatService.SearchChats(title);

                return Ok(chats.Select(chat => new ChatResponse
                {
                    Id = chat.Id,
                    Title = chat.Title,
                    CreatorId = chat.CreatorId,
                }));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Chat not found by Title");
                return NotFound("Chat not found by Title");
            }
        }

        /// <summary>
        /// Create a new chat.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Chat/Create
        ///     {
        ///         "title": "New Chat",
        ///         "creatorId": 1
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">The request containing chat details.</param>
        /// <returns>A boolean indicating if the chat was created successfully.</returns>
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChatDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> CreateChat([FromBody] ChatRequest request)
        {
            try
            {
                var chatDto = new ChatDto
                {
                    Title = request.Title,
                    CreatorId = request.CreatorId,
                };

                var createSuccessfully = await _chatService.CreateChat(chatDto);
                if (createSuccessfully)
                {
                    return Ok("Chat was created");
                }
                else
                {
                    return BadRequest("Failed to create chat");
                }
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Chat was not created");
                return NotFound("Chat was not created");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Chat with the same title and creator already exists");
                return BadRequest("Chat with the same title and creator already exists");
            }
        }

        /// <summary>
        /// Add a message to a chat.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Chat/AddMessage
        ///     {
        ///         "content": "Hello, world!",
        ///         "chatId": 1,
        ///         "userId": 1
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">The request containing message details.</param>
        /// <returns>A boolean indicating if the message was added successfully.</returns>
        [HttpPost("AddMessage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> AddMessage([FromBody] MessageRequest request)
        {
            try
            {
                var messageDto = new MessageDto
                {
                    Content = request.Content,
                    ChatId = request.ChatId,
                    UserId = request.UserId,
                };

                var addSuccessfully = await _chatService.AddMessage(messageDto);
                if (addSuccessfully)
                {
                    return Ok("Message send");
                }
                else
                {
                    return BadRequest("Failed to send message");
                }
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Chat or User not found");
                return NotFound("Chat or User not found");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "User is not a member of the chat");
                return NotFound("User is not a member of the chat");
            }
        }

        /// <summary>
        /// Add a user to a chat.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Chat/AddUser
        ///     {
        ///         "chatId": 1,
        ///         "userId": 2
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">The request containing user and chat details.</param>
        /// <returns>A boolean indicating if the user was added successfully.</returns>
        [HttpPost("AddUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> AddUserToChat([FromBody] UserRequest request)
        {
            try
            {
                var addSuccessfully = await _chatService.AddUserToChat(request.ChatId, request.UserId);
                if (addSuccessfully)
                {
                    return Ok("User add");
                }
                else
                {
                    return BadRequest("Failed to add user to chat");
                }
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Chat or User not found");
                return NotFound("Chat or User not found");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "User is already in the chat");
                return NotFound("User is already in the chat");
            }
        }

        /// <summary>
        /// Delete a chat.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE api/Chat/Delete
        ///     {
        ///         "title": "New Chat",
        ///         "creatorId": 1
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">The request containing chat details.</param>
        /// <returns>A boolean indicating if the chat was deleted successfully.</returns>
        [HttpDelete("Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteChat([FromBody] ChatRequest request)
        {
            try
            {
                var chatDto = new ChatDto
                {
                    Title = request.Title,
                    CreatorId = request.CreatorId
                };

                var deleteSuccessfully = await _chatService.DeleteChat(chatDto);
                if (deleteSuccessfully)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound("Chat not found or unauthorized to delete");
                }
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Chat not found");
                return NotFound("Chat not found");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "You do not have permissions to delete this chat");
                return Forbid("You do not have permissions to delete this chat");
            }
        }

        /// <summary>
        /// Remove a user from a chat.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE api/Chat/RemoveUser
        ///     {
        ///         "chatId": 1,
        ///         "userId": 2
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">The request containing user and chat details.</param>
        /// <returns>A boolean indicating if the user was removed successfully.</returns>
        [HttpDelete("RemoveUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> RemoveUserFromChat([FromBody] UserRequest request)
        {
            try
            {
                var removeSuccessfully = await _chatService.RemoveUserFromChat(request.ChatId, request.UserId);
                if (removeSuccessfully)
                {
                    return Ok("User remove from chat");
                }
                else
                {
                    return BadRequest("Failed to remove user from chat");
                }
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "User is not in the chat");
                return NotFound("User is not in the chat");
            }
        }
    }
}
