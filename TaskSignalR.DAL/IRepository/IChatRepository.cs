using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSignalR.DAL.Models;

namespace TaskSignalR.DAL.IRepository
{
    public interface IChatRepository
    {
        Task<IEnumerable<Chat>> GetAllChatsAsync();
        Task<bool> CreateChatAsync(Chat chat);
        Task<Chat> GetChatByIdAsync(int id);
        Task<IEnumerable<Chat>> SearchChatsAsync(string title);
        Task<bool> DeleteChatAsync(Chat chat);
        Task<bool> AddMessageAsync(Message message);
        Task<bool> AddUserToChatAsync(int chatId, int userId);
        Task<bool> RemoveUserFromChatAsync(int chatId, int userId);
    }
}
