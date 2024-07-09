using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSignalR.DAL.Models;

namespace TaskSignalR.DAL.IRepository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> CreateUserAsync(string name);
        Task<bool> UpdateUserAsync(User newUser);
        Task<bool> DeleteUserAsync(int userId);
    }
}
