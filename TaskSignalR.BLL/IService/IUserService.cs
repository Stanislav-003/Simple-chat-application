using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSignalR.BLL.DTO;
using TaskSignalR.DAL.Models;

namespace TaskSignalR.BLL.IService
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsers();
        Task<UserDto> GetUserById(int userId);
        Task<bool> CreateUser(string name);
        Task<bool> UpdateUser(UserDto newUserDto);
        Task<bool> DeleteUser(int userId);
    }
}
