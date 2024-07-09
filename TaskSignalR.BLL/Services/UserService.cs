using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSignalR.BLL.DTO;
using TaskSignalR.BLL.IService;
using TaskSignalR.DAL.IRepository;
using TaskSignalR.DAL.Models;

namespace TaskSignalR.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> CreateUser(string name)
        {
            return await _userRepository.CreateUserAsync(name);
        }

        public async Task<bool> DeleteUser(int userId)
        {
            return await _userRepository.DeleteUserAsync(userId);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();

            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.Name,
            }).ToList();

            return userDtos;
        }

        public async Task<UserDto> GetUserById(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
            };

            return userDto;
        }

        public async Task<bool> UpdateUser(UserDto newUserDto)
        {
            var user = new User
            {
                Id = newUserDto.Id,
                Name = newUserDto.Name,
            };

            return await _userRepository.UpdateUserAsync(user);
        }
    }
}
