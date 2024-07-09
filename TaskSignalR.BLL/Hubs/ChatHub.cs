using Microsoft.AspNetCore.SignalR;
using TaskSignalR.BLL.DTO;
using TaskSignalR.BLL.IService;

namespace TaskSignalR.BLL.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task SendMessage(string chatId, string userId, string message)
        {
            await Clients.Group(chatId).SendAsync("ReceiveMessage", userId, message);

            var messageDto = new MessageDto
            {
                ChatId = int.Parse(chatId),
                UserId = int.Parse(userId),
                Content = message
            };
            await _chatService.AddMessage(messageDto);
        }

        public async Task JoinChat(string chatId, string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);

            await Clients.Group(chatId).SendAsync("UserJoined", userId);

            await _chatService.AddUserToChat(int.Parse(chatId), int.Parse(userId));
        }

        public async Task LeaveChat(string chatId, string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);

            await Clients.Group(chatId).SendAsync("UserLeft", userId);

            await _chatService.RemoveUserFromChat(int.Parse(chatId), int.Parse(userId));
        }

        public async Task DeleteChat(string chatId, string userId)
        {
            var chatIdInt = int.Parse(chatId);
            var userIdInt = int.Parse(userId);

            var chatDto = new ChatDto
            {
                Id = chatIdInt,
                CreatorId = userIdInt
            };

            try
            {
                var deleteSuccessfully = await _chatService.DeleteChat(chatDto);
                if (deleteSuccessfully)
                {
                    await Clients.Group(chatId).SendAsync("ChatDeleted", chatId);

                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
                }
            }
            catch (UnauthorizedAccessException)
            {
                await Clients.Caller.SendAsync("Error", "You do not have permissions to delete this chat");
            }
            catch (KeyNotFoundException)
            {
                await Clients.Caller.SendAsync("Error", "Chat not found");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }
    }
}
