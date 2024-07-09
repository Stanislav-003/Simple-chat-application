using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TaskSignalR.DAL.Data;
using TaskSignalR.DAL.IRepository;
using TaskSignalR.DAL.Models;

namespace TaskSignalR.BLL.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateUserAsync(string name)
        {
            var user = new User
            {
                Name = name
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.OrderBy(u => u.Id).ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(User newUser)
        {
            var existingUser = await _context.Users.FindAsync(newUser.Id);

            if (existingUser == null)
            {
                return false;
            }

            existingUser.Name = newUser.Name;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return false;
            }

            var chats = await _context.Chats.Where(c => c.CreatorId == userId).ToListAsync();

            _context.Chats.RemoveRange(chats);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
