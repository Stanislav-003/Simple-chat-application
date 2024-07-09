using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSignalR.BLL.DTO;
using TaskSignalR.DAL.Models;

namespace TaskSignalR.BLL.IService
{
    public interface IChatService
    {
        Task<IEnumerable<ChatDto>> GetAllChats();
        Task<bool> CreateChat(ChatDto chatDto);
        Task<ChatDto> GetChatById(int id);
        Task<IEnumerable<ChatDto>> SearchChats(string title);
        Task<bool> DeleteChat(ChatDto chatDto);
        Task<bool> AddMessage(MessageDto messageDto);
        Task<bool> AddUserToChat(int chatId, int userId);
        Task<bool> RemoveUserFromChat(int chatId, int userId);
    }
}
