using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSignalR.DAL.Data;
using TaskSignalR.DAL.Models;
using TaskSignalR;
using TaskSignalR.DAL.IRepository;

namespace TaskSignalR.BLL.Services
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            var chats = await _context.Chats
                .Include(c => c.Creator)
                .Include(c => c.Messages)
                .Include(c => c.ChatUsers)
                    .ThenInclude(cu => cu.User)
                .ToListAsync();

            return chats;
        }

        public async Task<bool> CreateChatAsync(Chat chat)
        {
            var existingChat = await _context.Chats.FirstOrDefaultAsync(c => c.Title == chat.Title && c.CreatorId == chat.CreatorId);
            if (existingChat != null)
            {
                throw new InvalidOperationException();
            }

            var creator = await _context.Users.FindAsync(chat.CreatorId);
            if (creator == null)
            {
                throw new KeyNotFoundException();
            }

            var newChat = new Chat
            {
                Title = chat.Title,
                CreatorId = chat.CreatorId,
                Creator = creator,
                Messages = new List<Message>(),
                ChatUsers = new List<ChatUser>()
            };

            newChat.ChatUsers.Add(new ChatUser { ChatId = newChat.Id, UserId = creator.Id });

            _context.Chats.Add(newChat);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Chat> GetChatByIdAsync(int id)
        {
            var chat =  await _context.Chats
                .Include(c => c.Creator)
                .Include(c => c.Messages)
                .Include(c => c.ChatUsers)
                    .ThenInclude(cu => cu.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chat == null)
            {
                throw new KeyNotFoundException();
            }

            return chat;
        }

        public async Task<IEnumerable<Chat>> SearchChatsAsync(string title)
        {
            var chats = await _context.Chats
                .Include(c => c.Creator)
                .Include(c => c.Messages)
                .Include(c => c.ChatUsers)
                    .ThenInclude(cu => cu.User)
                .Where(c => c.Title.Contains(title)).ToListAsync();

            if (!chats.Any())
            {
                throw new KeyNotFoundException();
            }

            return chats;
        }

        public async Task<bool> DeleteChatAsync(Chat chat)
        {
            var existChat = await _context.Chats
                    .Include(c => c.Creator)
                    .Include(c => c.Messages)
                    .Include(c => c.ChatUsers)
                        .ThenInclude(cu => cu.User)
                    .FirstOrDefaultAsync(c => c.Title == chat.Title && c.CreatorId == chat.CreatorId);

            if (existChat == null)
            {
                throw new KeyNotFoundException();
            }

            if (existChat.CreatorId != chat.CreatorId)
            {
                throw new UnauthorizedAccessException();
            }

            _context.Chats.Remove(existChat);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddMessageAsync(Message message)
        {
            var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id == message.ChatId);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == message.UserId);

            if (chat == null || user == null)
            {
                throw new KeyNotFoundException();
            }

            var isUserInTheChat = await _context.ChatUsers.AnyAsync(cu => cu.ChatId == message.ChatId && cu.UserId == message.UserId);

            if (!isUserInTheChat)
            {
                throw new InvalidOperationException();
            }

            var newMessage = new Message
            {
                ChatId = message.ChatId,
                UserId = message.UserId,
                Content = message.Content,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddUserToChatAsync(int chatId, int userId)
        {
            var chat = await _context.Chats.FindAsync(chatId);
            var user = await _context.Users.FindAsync(userId);

            if (chat == null || user == null)
            {
                throw new KeyNotFoundException();
            }

            if (await _context.ChatUsers.AnyAsync(cu => cu.ChatId == chatId && cu.UserId == userId))
            {
                throw new InvalidOperationException();
            }

            _context.ChatUsers.Add(new ChatUser { ChatId = chat.Id, UserId = user.Id });
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveUserFromChatAsync(int chatId, int userId)
        {
            var chatUser = await _context.ChatUsers.FirstOrDefaultAsync(cu => cu.ChatId == chatId && cu.UserId == userId);

            if (chatUser == null)
            {
                throw new KeyNotFoundException();
            }

            _context.ChatUsers.Remove(chatUser);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
