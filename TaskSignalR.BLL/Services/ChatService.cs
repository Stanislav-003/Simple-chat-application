using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSignalR.BLL.DTO;
using TaskSignalR.BLL.Hubs;
using TaskSignalR.BLL.IService;
using TaskSignalR.DAL.IRepository;
using TaskSignalR.DAL.Models;

namespace TaskSignalR.BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IHubContext<ChatHub> _chatHubContext;

        public ChatService(IChatRepository chatRepository, IHubContext<ChatHub> chatHubContext)
        {
            _chatRepository = chatRepository;
            _chatHubContext = chatHubContext;
        }

        public async Task<bool> AddMessage(MessageDto messageDto)
        {
            var message = new Message
            {
                Content = messageDto.Content,
                ChatId = messageDto.ChatId,
                UserId = messageDto.UserId,
            };

            return await _chatRepository.AddMessageAsync(message);
        }

        public async Task<bool> AddUserToChat(int chatId, int userId)
        {
            return await _chatRepository.AddUserToChatAsync(chatId, userId);
        }

        public async Task<bool> CreateChat(ChatDto chatDto)
        {
            var chat = new Chat 
            {
                //Id = chatDto.Id,
                Title = chatDto.Title,
                CreatorId = chatDto.CreatorId,
            };

            return await _chatRepository.CreateChatAsync(chat);
        }

        public async Task<bool> DeleteChat(ChatDto chatDto)
        {
            var chat = new Chat
            {
                Id = chatDto.Id,
                Title = chatDto.Title,
                CreatorId = chatDto.CreatorId,
            };

            var deleteSuccessfully = await _chatRepository.DeleteChatAsync(chat);

            if (deleteSuccessfully)
            {
                await _chatHubContext.Clients.Group(chat.Id.ToString()).SendAsync("ChatDeleted", chat.Id);
            }

            return deleteSuccessfully;
        }

        public async Task<IEnumerable<ChatDto>> GetAllChats()
        {
            var chats = await _chatRepository.GetAllChatsAsync();

            var chatDtos = chats.Select(chat => new ChatDto
            {
                Id = chat.Id,
                Title = chat.Title,
                CreatorId = chat.CreatorId,
            }).ToList();

            return chatDtos;
        }

        public async Task<ChatDto> GetChatById(int id)
        {
            var chat = await _chatRepository.GetChatByIdAsync(id);

            var chatDto = new ChatDto 
            {
                Id = chat.Id,
                Title = chat.Title,
                CreatorId = chat.CreatorId,
            };

            return chatDto;
        }

        public async Task<bool> RemoveUserFromChat(int chatId, int userId)
        {
            return await _chatRepository.RemoveUserFromChatAsync(chatId, userId);
        }

        public async Task<IEnumerable<ChatDto>> SearchChats(string title)
        {
            var chats = await _chatRepository.SearchChatsAsync(title);

            var chatDtos = chats.Select(chat => new ChatDto
            {
                Id = chat.Id,
                Title = chat.Title,
                CreatorId = chat.CreatorId,
            }).ToList();

            return chatDtos;
        }
    }
}
